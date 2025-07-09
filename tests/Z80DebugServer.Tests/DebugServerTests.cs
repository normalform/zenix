using Xunit;
using Zenix.Core;
using Zenix.Tools.Z80DebugServer;

namespace Zenix.Tests.DebugServer;

public class DebugServerTests
{
    private static (DebugServer server, Z80Cpu cpu, Z80MemoryMap memory) CreateServer(byte[]? program = null)
    {
        var memory = new Z80MemoryMap();
        var cpu = new Z80Cpu(memory, new Zenix.Core.Interrupt.Z80Interrupt(), new Z80CpuOptions { RomSize = 1024, RamSize = 1024 });
        if (program is not null)
        {
            memory.LoadRom(program);
        }
        var server = new DebugServer(cpu, memory);
        return (server, cpu, memory);
    }

    [Fact]
    public void Continue_StopsAtBreakpoint()
    {
        var (server, cpu, _) = CreateServer([Z80OpCode.NOP, Z80OpCode.NOP, Z80OpCode.HALT]);
        server.SetBreakpoints(new ushort[] { 1 });

        server.Continue();

        Assert.Equal((ushort)1, cpu.PC);
    }

    [Fact]
    public void MemoryReadWrite_Works()
    {
        var (server, _, memory) = CreateServer();
        server.WriteMemory(0x10, [0x42, 0x43]);
        var data = server.ReadMemory(0x10, 2);
        Assert.Equal(new byte[] { 0x42, 0x43 }, data);
    }

    [Fact]
    public void RegisterReadWrite_Works()
    {
        var (server, cpu, _) = CreateServer();
        var regs = cpu.Registers;
        regs.A = 0x11;
        regs.F = 0xFF;
        regs.PC = 0x1234;
        server.WriteRegisters(regs);

        var read = server.ReadRegisters();
        Assert.Equal((byte)0x11, read.A);
        Assert.Equal((byte)0xFF, read.F);
        Assert.Equal((ushort)0x1234, read.PC);
    }
}

