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
