// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using Zenix.Core;
using Zenix.Core.Interrupt;

namespace Zenix.App;

/// <summary>
/// Factory implementation for creating emulator components with dependency injection
/// </summary>
public class EmulatorFactory : IEmulatorFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initialize the factory with the DI service provider
    /// </summary>
    /// <param name="serviceProvider">Service provider for dependency resolution</param>
    public EmulatorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Create a Z80 CPU with the specified options
    /// </summary>
    /// <param name="options">CPU configuration options</param>
    /// <returns>Configured Z80 CPU instance</returns>
    public Z80Cpu CreateCpu(Z80CpuOptions options)
    {
        var interrupt = _serviceProvider.GetRequiredService<IZ80Interrupt>();
        var memoryMap = _serviceProvider.GetRequiredService<Z80MemoryMap>();
        
        return new Z80Cpu(memoryMap, interrupt, options);
    }

    /// <summary>
    /// Create a Z80 CPU with default options
    /// </summary>
    /// <returns>Z80 CPU with default configuration</returns>
    public Z80Cpu CreateCpu()
    {
        var defaultOptions = new Z80CpuOptions();
        return CreateCpu(defaultOptions);
    }

    /// <summary>
    /// Create an async interrupt emulator for the specified interrupt controller
    /// </summary>
    /// <param name="interrupt">Interrupt controller to emulate</param>
    /// <returns>Async interrupt emulator instance</returns>
    public AsyncInterruptEmulator CreateAsyncInterruptEmulator(IZ80Interrupt interrupt)
    {
        return new AsyncInterruptEmulator(interrupt);
    }
}
