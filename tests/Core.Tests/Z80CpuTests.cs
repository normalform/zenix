// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Xunit;
using Zenix.Core;

namespace Zenix.Core.Tests;

public class Z80CpuTests
{
    [Fact]
    public void Cpu_Can_Step()
    {
        var memory = new MsxMemoryMap();
        var cpu = new Z80Cpu(memory);
        cpu.Step();
        Assert.NotNull(cpu);
    }

    [Fact]
    public void Cpu_Executes_Binary_File()
    {
        var binPath = Path.Combine(AppContext.BaseDirectory, "Core.Tests", "Assets", "simple.bin");
        var rom = File.ReadAllBytes(binPath);

        var memory = new MsxMemoryMap();
        var options = new Z80CpuOptions { RomSize = rom.Length, RamSize = 256 };
        var cpu = new Z80Cpu(memory, options);
        memory.LoadRom(rom);

        int safety = 10;
        while (!cpu.Halted && safety-- > 0)
        {
            cpu.Step();
        }

        Assert.True(cpu.Halted);        
        Assert.Equal(0x42, cpu.A);
    }
}
