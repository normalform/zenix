// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Zenix.App.Configuration;
using Zenix.Core;
using Zenix.Core.Interrupt;

namespace Zenix.App;

/// <summary>
/// Composition root for dependency injection in the Zenix emulator
/// </summary>
public class EmulatorCompositionRoot
{
    private readonly IServiceProvider _serviceProvider;
    private readonly EmulatorConfiguration _configuration;

    /// <summary>
    /// Initialize the composition root with dependency injection
    /// </summary>
    /// <param name="configurationPath">Path to configuration file (optional)</param>
    public EmulatorCompositionRoot(string? configurationPath = null)
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(configurationPath);
        
        ConfigureServices(services, configuration);
        _serviceProvider = services.BuildServiceProvider();
        _configuration = _serviceProvider.GetRequiredService<IOptions<EmulatorConfiguration>>().Value;
    }

    /// <summary>
    /// Build configuration from JSON file and environment
    /// </summary>
    private static IConfiguration BuildConfiguration(string? configurationPath)
    {
        var builder = new ConfigurationBuilder();
        
        // Add default configuration file
        string defaultConfigPath = configurationPath ?? "appsettings.json";
        if (File.Exists(defaultConfigPath))
        {
            builder.AddJsonFile(defaultConfigPath, optional: false, reloadOnChange: true);
        }
        
        // Add environment-specific config if exists
        string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
        string envConfigPath = $"appsettings.{environment}.json";
        if (File.Exists(envConfigPath))
        {
            builder.AddJsonFile(envConfigPath, optional: true, reloadOnChange: true);
        }
        
        // Environment variables override
        builder.AddEnvironmentVariables("ZENIX_");
        
        return builder.Build();
    }

    /// <summary>
    /// Configure dependency injection services
    /// </summary>
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.Configure<EmulatorConfiguration>(
            configuration.GetSection("EmulatorConfiguration"));

        // Core services
        services.AddSingleton<Z80MemoryMap>();
        services.AddTransient<IZ80Interrupt, Z80Interrupt>();
        
        // Factory services
        services.AddSingleton<IEmulatorFactory, EmulatorFactory>();
    }

    /// <summary>
    /// Create a fully configured Z80 CPU with default options
    /// </summary>
    public Z80Cpu CreateCpu()
    {
        var factory = _serviceProvider.GetRequiredService<IEmulatorFactory>();
        return factory.CreateCpu(_configuration.DefaultCpuOptions.ToZ80CpuOptions());
    }

    /// <summary>
    /// Create a CPU with custom options
    /// </summary>
    public Z80Cpu CreateCpu(Z80CpuOptions options)
    {
        var factory = _serviceProvider.GetRequiredService<IEmulatorFactory>();
        return factory.CreateCpu(options);
    }

    /// <summary>
    /// Create a CPU configured for MSX emulation
    /// </summary>
    public Z80Cpu CreateMsxCpu()
    {
        var factory = _serviceProvider.GetRequiredService<IEmulatorFactory>();
        var cpu = factory.CreateCpu(_configuration.MsxConfiguration.ToZ80CpuOptions());
        
        // Configure for MSX-like operation
        cpu.Interrupt.SetInterruptMode(_configuration.MsxConfiguration.GetInterruptMode());
        
        return cpu;
    }

    /// <summary>
    /// Create a CPU with asynchronous interrupt emulation for MSX devices
    /// </summary>
    public (Z80Cpu Cpu, AsyncInterruptEmulator InterruptEmulator) CreateMsxWithInterruptEmulation()
    {
        var cpu = CreateMsxCpu();
        var interruptEmulator = new AsyncInterruptEmulator(cpu.Interrupt);

        var msxConfig = _configuration.MsxConfiguration;
        
        // Set up VDP interrupts if enabled
        if (msxConfig.EnableVdpInterrupts)
        {
            var intervalMs = 1000.0 / msxConfig.VdpInterruptFrequency;
            interruptEmulator.ScheduleRepeatingInterrupt(
                TimeSpan.FromMilliseconds(intervalMs), // Start immediately
                TimeSpan.FromMilliseconds(intervalMs), // Repeat every frame
                vector: 0xFF,
                priority: 5,
                source: VdpInterruptSource.VerticalBlank,
                description: "MSX VDP vertical blank interrupt"
            );
        }

        return (cpu, interruptEmulator);
    }

    /// <summary>
    /// Create a high-performance CPU configuration for benchmarking
    /// </summary>
    public Z80Cpu CreateBenchmarkCpu()
    {
        var benchmarkConfig = _configuration.BenchmarkConfiguration;
        return CreateCpu(benchmarkConfig.ToZ80CpuOptions());
    }

    /// <summary>
    /// Get a service from the DI container
    /// </summary>
    public T GetService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Dispose of resources
    /// </summary>
    public void Dispose()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
