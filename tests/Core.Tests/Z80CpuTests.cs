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
        var cpu = new Z80Cpu();
        cpu.Step();
        Assert.NotNull(cpu);
    }
}
