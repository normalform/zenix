// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.Core;

namespace Zenix.App;

/// <summary>
/// Main emulator host that manages the lifecycle and execution of the MSX emulator
/// </summary>
public class EmulatorHost
{
    private readonly EmulatorCompositionRoot _compositionRoot;
    private readonly Z80Cpu _cpu;

    /// <summary>
    /// Initialize the emulator host with default configuration
    /// </summary>
    public EmulatorHost() : this(null)
    {
    }

    /// <summary>
    /// Initialize the emulator host with custom configuration
    /// </summary>
    /// <param name="configurationPath">Path to configuration file (optional)</param>
    public EmulatorHost(string? configurationPath)
    {
        _compositionRoot = new EmulatorCompositionRoot(configurationPath);
        _cpu = _compositionRoot.CreateCpu();
    }

    /// <summary>
    /// Run a single CPU step/frame
    /// </summary>
    public void RunFrame()
    {
        _cpu.Step();
    }

    /// <summary>
    /// Get the CPU instance for direct access
    /// </summary>
    public Z80Cpu Cpu => _cpu;

    /// <summary>
    /// Get the composition root for accessing other services
    /// </summary>
    public EmulatorCompositionRoot CompositionRoot => _compositionRoot;

    /// <summary>
    /// Dispose of resources
    /// </summary>
    public void Dispose()
    {
        _compositionRoot.Dispose();
    }
}
