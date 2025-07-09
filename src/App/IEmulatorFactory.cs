// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.Core;
using Zenix.Core.Interrupt;

namespace Zenix.App;

/// <summary>
/// Factory interface for creating emulator components with dependency injection
/// </summary>
public interface IEmulatorFactory
{
    /// <summary>
    /// Create a Z80 CPU with the specified options
    /// </summary>
    /// <param name="options">CPU configuration options</param>
    /// <returns>Configured Z80 CPU instance</returns>
    Z80Cpu CreateCpu(Z80CpuOptions options);

    /// <summary>
    /// Create a Z80 CPU with default options
    /// </summary>
    /// <returns>Z80 CPU with default configuration</returns>
    Z80Cpu CreateCpu();

    /// <summary>
    /// Create an async interrupt emulator for the specified interrupt controller
    /// </summary>
    /// <param name="interrupt">Interrupt controller to emulate</param>
    /// <returns>Async interrupt emulator instance</returns>
    AsyncInterruptEmulator CreateAsyncInterruptEmulator(IZ80Interrupt interrupt);
}
