// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.Core;

namespace Zenix.App;

public class EmulatorHost
{
    private readonly MsxMemoryMap _memory = new();
    private readonly Z80Cpu _cpu;

    public EmulatorHost()
    {
        _cpu = new Z80Cpu(_memory);

        // Attach debug hooks to log memory accesses
        _cpu.MemoryReadHook = (addr, val) =>
            Console.WriteLine($"READ  {addr:X4}: {val:X2}");
        _cpu.MemoryWriteHook = (addr, val) =>
            Console.WriteLine($"WRITE {addr:X4}: {val:X2}");
    }

    public void Step()
    {
        _cpu.Step();
        DumpState();
    }

    private void DumpState()
    {
        Console.WriteLine($"PC:{_cpu.PC:X4} AF:{_cpu.A:X2}{_cpu.F:X2} BC:{_cpu.B:X2}{_cpu.C:X2} DE:{_cpu.D:X2}{_cpu.E:X2} HL:{_cpu.H:X2}{_cpu.L:X2} SP:{_cpu.SP:X4}");

        Console.Write("MEM[0000-000F]:");
        for (ushort i = 0; i < 0x10; i++)
        {
            Console.Write($" {_memory.ReadByte(i):X2}");
        }
        Console.WriteLine();
    }
}
