// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.Core;
using Zenix.Core.Interrupt;

namespace Zenix.App.Configuration;

/// <summary>
/// Root configuration record for the Zenix emulator
/// Provides parameterless constructor for configuration binding compatibility
/// </summary>
public record EmulatorConfiguration
{
    /// <summary>
    /// Default CPU configuration options
    /// </summary>
    public CpuConfiguration DefaultCpuOptions { get; init; } = new();
    
    /// <summary>
    /// MSX-specific configuration
    /// </summary>
    public MsxConfiguration MsxConfiguration { get; init; } = new();
    
    /// <summary>
    /// Benchmark/performance testing configuration
    /// </summary>
    public BenchmarkConfiguration BenchmarkConfiguration { get; init; } = new();
}

/// <summary>
/// Immutable CPU configuration options
/// Provides parameterless constructor for configuration binding compatibility
/// </summary>
public record CpuConfiguration
{
    /// <summary>
    /// ROM size in bytes (default: 32KB)
    /// </summary>
    public int RomSize { get; init; } = 32 * 1024; // 32KB

    /// <summary>
    /// RAM size in bytes (default: 64KB)
    /// </summary>
    public int RamSize { get; init; } = 64 * 1024; // 64KB

    /// <summary>
    /// CPU clock frequency in MHz (default: 3.58)
    /// </summary>
    public double ClockMHz { get; init; } = 3.58;

    /// <summary>
    /// Convert to Z80CpuOptions
    /// </summary>
    public Z80CpuOptions ToZ80CpuOptions() => new(ClockMHz, RomSize, RamSize);
}

/// <summary>
/// Immutable MSX-specific configuration
/// Provides parameterless constructor for configuration binding compatibility
/// </summary>
public record MsxConfiguration : CpuConfiguration
{
    /// <summary>
    /// Z80 interrupt mode for MSX emulation (default: "Mode1")
    /// </summary>
    public string InterruptMode { get; init; } = "Mode1";

    /// <summary>
    /// Whether to enable VDP interrupts (default: true)
    /// </summary>
    public bool EnableVdpInterrupts { get; init; } = true;

    /// <summary>
    /// VDP interrupt frequency in Hz (default: 60.0)
    /// </summary>
    public double VdpInterruptFrequency { get; init; } = 60.0;

    /// <summary>
    /// Convert interrupt mode string to enum
    /// </summary>
    public Z80InterruptMode GetInterruptMode() => InterruptMode.ToLowerInvariant() switch
    {
        "mode0" => Z80InterruptMode.Mode0,
        "mode1" => Z80InterruptMode.Mode1,
        "mode2" => Z80InterruptMode.Mode2,
        _ => Z80InterruptMode.Mode1 // Default for MSX
    };
}

/// <summary>
/// Immutable benchmark configuration for performance testing
/// Provides parameterless constructor for configuration binding compatibility
/// </summary>
public record BenchmarkConfiguration : CpuConfiguration
{
    /// <summary>
    /// Whether to enable interrupt emulation (default: false)
    /// </summary>
    public bool EnableInterruptEmulation { get; init; } = false;

    /// <summary>
    /// Override with larger memory for benchmark scenarios
    /// </summary>
    public new int RomSize { get; init; } = 64 * 1024; // 64KB

    /// <summary>
    /// Override with larger memory for benchmark scenarios
    /// </summary>
    public new int RamSize { get; init; } = 128 * 1024; // 128KB
}
