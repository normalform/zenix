// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.App;
using Zenix.App.Configuration;

namespace Zenix.Demos;

/// <summary>
/// Demonstrates configuration and dependency injection capabilities
/// </summary>
public static class ConfigurationDemo
{
    /// <summary>
    /// Main demo entry point
    /// </summary>
    public static void RunDemo()
    {
        Console.WriteLine("Configuration Demo");
        Console.WriteLine("=================");
        
        // Create composition root and configure services
        var compositionRoot = new EmulatorCompositionRoot();
        
        // Create default configuration
        var config = new EmulatorConfiguration();
        
        Console.WriteLine($"CPU Clock: {config.DefaultCpuOptions.ClockMHz} MHz");
        Console.WriteLine($"ROM Size: {config.DefaultCpuOptions.RomSize} bytes");
        Console.WriteLine($"RAM Size: {config.DefaultCpuOptions.RamSize} bytes");
        Console.WriteLine($"VDP Interrupts: {config.MsxConfiguration.EnableVdpInterrupts}");
        
        Console.WriteLine("\nConfiguration demo completed successfully.");
    }
}