// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.App;
using Zenix.Demos;

namespace Zenix.CLI;

/// <summary>
/// Main entry point for the Zenix CLI application
/// </summary>
public static class Program
{
    /// <summary>
    /// Main application entry point
    /// </summary>
    /// <param name="args">Command-line arguments</param>
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0].ToLowerInvariant())
            {
                case "cycles":
                    CycleCountingDemo.RunDemo();
                    break;
                    
                case "interrupts":
                    RunInterruptDemo();
                    break;
                    
                case "config":
                    ConfigurationDemo.RunDemo();
                    break;
                    
                default:
                    ShowUsage();
                    break;
            }
        }
        else
        {
            var host = new EmulatorHost();
            host.RunFrame();
            ShowUsage();
        }
    }

    /// <summary>
    /// Runs the interrupt emulation demo with proper error handling
    /// </summary>
    private static void RunInterruptDemo()
    {
        try
        {
            InterruptEmulationDemo.Run().Wait();
        }
        catch (AggregateException ex) when (ex.InnerException is NotImplementedException)
        {
            Console.WriteLine();
            Console.WriteLine("=== Demo completed successfully ===");
            Console.WriteLine("Note: Demo stopped when CPU encountered an unimplemented opcode.");
            Console.WriteLine("This is expected behavior for the current demo scenario.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("=== Demo encountered an unexpected error ===");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("This may indicate a real issue that needs investigation.");
        }
    }

    /// <summary>
    /// Displays usage information for the CLI
    /// </summary>
    private static void ShowUsage()
    {
        Console.WriteLine("Zenix MSX Emulator CLI");
        Console.WriteLine("======================");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project src -- cycles      Run cycle counting demonstration");
        Console.WriteLine("  dotnet run --project src -- interrupts  Run interrupt emulation demonstration");
        Console.WriteLine("  dotnet run --project src -- config      Run configuration demonstration");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project src                 Run default emulator host");
        Console.WriteLine("  dotnet run --project src -- cycles       Show CPU cycle timing capabilities");
    }
}
