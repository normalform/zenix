using System.Text;
using System.Text.Json;
using Zenix.Core;

namespace Zenix.Tools.Z80DebugServer;

public class DebugServer
{
    private readonly Z80Cpu _cpu;
    private readonly Z80MemoryMap _memory;
    private readonly List<ushort> _breakpoints = new();

    public DebugServer(Z80Cpu cpu, Z80MemoryMap memory)
    {
        _cpu = cpu;
        _memory = memory;
    }

    public void SetBreakpoints(IEnumerable<ushort> breakpoints)
    {
        _breakpoints.Clear();
        _breakpoints.AddRange(breakpoints);
    }

    public void Continue()
    {
        while (true)
        {
            if (_breakpoints.Contains(_cpu.PC))
            {
                break;
            }

            _cpu.Step();

            if (_cpu.Halted)
            {
                break;
            }
        }
    }

    public void Step() => _cpu.Step();

    public byte[] ReadMemory(ushort address, int count)
    {
        var result = new byte[count];
        for (var i = 0; i < count; i++)
        {
            result[i] = _memory.ReadByte((ushort)(address + i));
        }

        return result;
    }

    public void WriteMemory(ushort address, byte[] data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            _memory.WriteByte((ushort)(address + i), data[i]);
        }
    }

    public Z80Cpu.Z80Registers ReadRegisters() => _cpu.Registers;

    public void WriteRegisters(Z80Cpu.Z80Registers registers)
    {
        _cpu.Registers.A = registers.A;
        _cpu.Registers.F = registers.F;
        _cpu.Registers.B = registers.B;
        _cpu.Registers.C = registers.C;
        _cpu.Registers.D = registers.D;
        _cpu.Registers.E = registers.E;
        _cpu.Registers.H = registers.H;
        _cpu.Registers.L = registers.L;
        _cpu.Registers.SP = registers.SP;
        _cpu.Registers.PC = registers.PC;
    }

    // Minimal DAP protocol implementation
    private record DapRequest(int seq, string command, JsonElement? arguments);
    private record DapResponse(int request_seq, bool success, string command, object? body = null);
    private record DapEvent(string @event, object? body = null);

    public async Task RunAsync(Stream input, Stream output, CancellationToken cancellationToken)
    {
        var reader = new StreamReader(input, Encoding.UTF8);
        var writer = new StreamWriter(output, Encoding.UTF8) { AutoFlush = true };

        while (!cancellationToken.IsCancellationRequested)
        {
            var request = await ReadRequestAsync(reader, cancellationToken);
            if (request is null)
            {
                break;
            }

            var response = await HandleRequestAsync(request.Value, writer, cancellationToken);
            if (response is not null)
            {
                await WriteMessageAsync(writer, response, cancellationToken);
            }
        }
    }

    private async Task<DapRequest?> ReadRequestAsync(StreamReader reader, CancellationToken ct)
    {
        string? line;
        int length = 0;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrEmpty(line))
            {
                break;
            }

            if (line.StartsWith("Content-Length:"))
            {
                length = int.Parse(line[15..].Trim());
            }
        }

        if (length == 0)
        {
            return null;
        }

        var buffer = new char[length];
        var read = await reader.ReadBlockAsync(buffer, 0, length);
        var json = new string(buffer, 0, read);
        var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var seq = root.GetProperty("seq").GetInt32();
        var cmd = root.GetProperty("command").GetString() ?? string.Empty;
        JsonElement? args = null;
        if (root.TryGetProperty("arguments", out var arg))
        {
            args = arg.Clone();
        }

        return new DapRequest(seq, cmd, args);
    }

    private async Task HandleStoppedAsync(StreamWriter writer, string reason, CancellationToken ct)
    {
        var stopped = new DapEvent("stopped", new { reason });
        await WriteMessageAsync(writer, stopped, ct);
    }

    private async Task<object?> HandleRequestAsync(DapRequest request, StreamWriter writer, CancellationToken ct)
    {
        switch (request.command)
        {
            case "initialize":
                await WriteMessageAsync(writer, new DapEvent("initialized"), ct);
                return new DapResponse(request.seq, true, request.command, new { supportsConfigurationDoneRequest = true });

            case "setBreakpoints":
                var bps = new List<ushort>();
                if (request.arguments is { } arg && arg.TryGetProperty("breakpoints", out var arr))
                {
                    foreach (var bp in arr.EnumerateArray())
                    {
                        if (bp.TryGetProperty("address", out var addressProp))
                        {
                            bps.Add((ushort)addressProp.GetInt32());
                        }
                    }
                }
                SetBreakpoints(bps);
                return new DapResponse(request.seq, true, request.command);

            case "configurationDone":
                return new DapResponse(request.seq, true, request.command);

            case "continue":
                Continue();
                await HandleStoppedAsync(writer, _cpu.Halted ? "halted" : "breakpoint", ct);
                return new DapResponse(request.seq, true, request.command);

            case "next":
                Step();
                await HandleStoppedAsync(writer, "step", ct);
                return new DapResponse(request.seq, true, request.command);

            case "readMemory":
                if (request.arguments is { } memArgs)
                {
                    var addr = (ushort)memArgs.GetProperty("address").GetInt32();
                    var count = memArgs.GetProperty("count").GetInt32();
                    var data = ReadMemory(addr, count);
                    return new DapResponse(request.seq, true, request.command, new { data });
                }
                break;

            case "writeMemory":
                if (request.arguments is { } writeArgs)
                {
                    var addr = (ushort)writeArgs.GetProperty("address").GetInt32();
                    var data = writeArgs.GetProperty("data").EnumerateArray().Select(e => (byte)e.GetInt32()).ToArray();
                    WriteMemory(addr, data);
                    return new DapResponse(request.seq, true, request.command);
                }
                break;

            case "zenix.readRegisters":
                var regs = ReadRegisters();
                return new DapResponse(request.seq, true, request.command, regs);

            case "zenix.writeRegisters":
                if (request.arguments is { } regArgs)
                {
                    var newRegs = JsonSerializer.Deserialize<Z80Cpu.Z80Registers>(regArgs.GetRawText());
                    if (newRegs is not null)
                    {
                        WriteRegisters(newRegs);
                    }
                }
                return new DapResponse(request.seq, true, request.command);

            case "disconnect":
                return null;
        }

        return new DapResponse(request.seq, false, request.command);
    }

    private static async Task WriteMessageAsync(StreamWriter writer, object message, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        await writer.WriteAsync($"Content-Length: {bytes.Length}\r\n\r\n");
        await writer.FlushAsync();
        await writer.BaseStream.WriteAsync(bytes, 0, bytes.Length, ct);
        await writer.BaseStream.FlushAsync(ct);
    }
}
