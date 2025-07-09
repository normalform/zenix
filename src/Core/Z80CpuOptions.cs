namespace Zenix.Core;

/// <summary>
/// Immutable configuration options for Z80 CPU emulation
/// </summary>
/// <param name="ClockMHz">CPU clock frequency in MHz (default: 3.58)</param>
/// <param name="RomSize">ROM size in bytes (default: 64KB)</param>
/// <param name="RamSize">RAM size in bytes (default: 64KB)</param>
public record Z80CpuOptions(
    double ClockMHz = 3.58,
    int RomSize = 0x10000,
    int RamSize = 0x10000);
