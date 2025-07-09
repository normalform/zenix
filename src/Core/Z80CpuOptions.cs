namespace Zenix.Core;

public class Z80CpuOptions
{
    public double ClockMHz { get; set; } = 3.58;
    public int RomSize { get; set; } = 0x10000;
    public int RamSize { get; set; } = 0x10000;
}
