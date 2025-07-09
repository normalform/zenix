// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core;

public class Z80Cpu
{
    private readonly MsxMemoryMap _memory;
    public Z80CpuOptions Options { get; }

    public Z80Cpu(MsxMemoryMap memory, Z80CpuOptions? options = null)
    {
        _memory = memory;
        Options = options ?? new Z80CpuOptions();
        _memory.Configure(Options.RomSize, Options.RamSize);
        Reset();
    }

    public byte A { get; private set; }
    public byte F { get; private set; }
    public byte B { get; private set; }
    public byte C { get; private set; }
    public byte D { get; private set; }
    public byte E { get; private set; }
    public byte H { get; private set; }
    public byte L { get; private set; }
    public ushort SP { get; private set; }
    public ushort PC { get; private set; }
    public bool Halted { get; private set; }

    public void Reset()
    {
        PC = 0;
        SP = (ushort)(Options.RamSize);
        Halted = false;
    }

    public void Step()
    {
        if (Halted) return;

        byte opcode = _memory.ReadByte(PC++);
        switch (opcode)
        {
            case 0x00: // NOP
                break;
            case 0x3E: // LD A, n
                A = _memory.ReadByte(PC++);
                break;
            case 0x76: // HALT
                Halted = true;
                break;
            case 0xC3: // JP nn
                ushort low = _memory.ReadByte(PC++);
                ushort high = _memory.ReadByte(PC++);
                PC = (ushort)(low | (high << 8));
                break;
            default:
                throw new NotImplementedException($"Opcode 0x{opcode:X2} not implemented");
        }
    }
}
