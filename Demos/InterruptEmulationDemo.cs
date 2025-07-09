// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.App;
using Zenix.Core;
using Zenix.Core.Interrupt;

namespace Zenix.Demos;

/// <summary>
/// Demonstrates the use of dependency injection and asynchronous interrupt emulation
/// </summary>
public static class InterruptEmulationDemo
{
    /// <summary>
    /// Main demo entry point
    /// </summary>
    public static async Task Run()
    {
        Console.WriteLine("=== Z80 Interrupt Emulation Demo ===");
        Console.WriteLine();
        
        BasicInterruptDemo();
        await AsyncInterruptDemo();
        DependencyInjectionDemo();
        
        Console.WriteLine("\n=== Demo Complete ===");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Demonstrates basic interrupt functionality
    /// </summary>
    public static void BasicInterruptDemo()
    {
        Console.WriteLine("=== Basic Interrupt Demo ===");
        Console.WriteLine();

        // Create CPU using DI composition root
        var compositionRoot = new EmulatorCompositionRoot();
        var cpu = compositionRoot.CreateCpu(new Z80CpuOptions { RomSize = 1024, RamSize = 1024 });
        var memory = compositionRoot.GetService<Z80MemoryMap>();
        var interruptController = cpu.Interrupt;

        // Set up simple program
        var program = new byte[]
        {
            Z80OpCode.EI,           // Enable interrupts
            Z80OpCode.NOP,          // EI delay
            Z80OpCode.JR_e, 0xFD,   // Jump back 3 bytes (infinite loop)
        };
        memory.LoadRom(program);

        // Set up async interrupt emulation
        using var interruptEmulator = new AsyncInterruptEmulator(interruptController);
        
        // Schedule some interrupts
        interruptEmulator.ScheduleMaskableInterrupt(TimeSpan.FromMilliseconds(100), 0xC7, 0, new TimerInterruptSource("DEMO_TIMER", "Demo Timer", 10));
        interruptEmulator.ScheduleMaskableInterrupt(TimeSpan.FromMilliseconds(200), 0xCF, 1, new IoDeviceInterruptSource("SERIAL", "Serial Port"));
        interruptEmulator.ScheduleNonMaskableInterrupt(TimeSpan.FromMilliseconds(300), new SystemInterruptSource("POWER_FAIL", "Power Failure"));

        Console.WriteLine("Running CPU with scheduled interrupts...");
        Console.WriteLine("Interrupt mode: " + interruptController.InterruptMode);
        
        // Run for a short period
        var startTime = DateTime.UtcNow;
        var cyclesBefore = cpu.TotalCycles;
        
        while ((DateTime.UtcNow - startTime).TotalMilliseconds < 500)
        {
            cpu.Step();
            
            // Show state occasionally
            if (cpu.TotalCycles % 1000 == 0)
            {
                Console.WriteLine($"PC: 0x{cpu.PC:X4}, Cycles: {cpu.TotalCycles}, " +
                                 $"Interrupts: {interruptController.InterruptsEnabled}");
            }
        }
        
        var cyclesAfter = cpu.TotalCycles;
        var elapsedMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
        
        Console.WriteLine();
        Console.WriteLine($"Executed {cyclesAfter - cyclesBefore} cycles in {elapsedMs:F1}ms");
        Console.WriteLine($"Effective frequency: {(cyclesAfter - cyclesBefore) / elapsedMs * 1000 / 1000:F1} kHz");
        Console.WriteLine($"Final interrupt state: {interruptController.GetState()}");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates asynchronous interrupt emulation with real-time timing
    /// </summary>
    public static async Task AsyncInterruptDemo()
    {
        Console.WriteLine("=== Async Interrupt Demo ===");
        Console.WriteLine();

        var compositionRoot = new EmulatorCompositionRoot();
        var cpu = compositionRoot.CreateMsxCpu();
        var memory = compositionRoot.GetService<Z80MemoryMap>();
        var interruptController = cpu.Interrupt;

        // Set up interrupt mode 1 and enable interrupts
        interruptController.SetInterruptMode(Z80InterruptMode.Mode1);
        
        var program = new byte[]
        {
            Z80OpCode.EI,           // Enable interrupts
            Z80OpCode.NOP,          // EI delay
            Z80OpCode.JR_e, 0xFD,   // Jump back 3 bytes (infinite loop)
        };
        memory.LoadRom(program);

        using var interruptEmulator = new AsyncInterruptEmulator(interruptController);
        
        var interruptCount = 0;
        interruptEmulator.InterruptEmulated += (sender, args) =>
        {
            interruptCount++;
            Console.WriteLine($"Interrupt #{interruptCount}: {args.InterruptRequest.Type} from {args.InterruptRequest.Source}");
        };

        // Schedule various interrupt patterns
        interruptEmulator.ScheduleRepeatingInterrupt(
            TimeSpan.FromMilliseconds(100), 
            TimeSpan.FromMilliseconds(200), 
            0xFF,
            0,
            new TimerInterruptSource("TIMER_50HZ", "50Hz Timer", 50),
            "50Hz timer interrupt");
            
        interruptEmulator.EmulateVdpInterrupt();
        interruptEmulator.EmulateKeyboardInterrupt();

        Console.WriteLine("Running CPU with async interrupt emulation...");
        Console.WriteLine("Press any key to stop or wait 5 seconds...");

        // Run CPU asynchronously
        var cpuTask = Task.Run(async () =>
        {
            while (!Console.KeyAvailable)
            {
                cpu.Step();
                
                // Periodic status update
                if (cpu.TotalCycles % 5000 == 0)
                {
                    Console.WriteLine($"Cycles: {cpu.TotalCycles}, PC: 0x{cpu.PC:X4}, " +
                                     $"Interrupts={interruptController.InterruptsEnabled}, " +
                                     $"Mode={interruptController.InterruptMode}");
                }

                // Small delay to make output readable
                await Task.Delay(1);
            }
        });

        // Wait for user input or timeout
        var timeoutTask = Task.Delay(5000); // 5 second timeout
        var completedTask = await Task.WhenAny(
            Task.Run(() => Console.ReadKey(true)),
            timeoutTask,
            cpuTask
        );

        if (completedTask == timeoutTask)
        {
            Console.WriteLine("\nDemo timed out after 5 seconds.");
        }
        else
        {
            Console.WriteLine("\nDemo stopped by user.");
        }

        Console.WriteLine();
        Console.WriteLine("=== Demo Summary ===");
        Console.WriteLine($"Total interrupts emulated: {interruptCount}");
        Console.WriteLine($"CPU total cycles: {cpu.TotalCycles}");
        Console.WriteLine($"Effective frequency: {cpu.EffectiveClockFrequency / 1000:F1} kHz");
        Console.WriteLine($"Interrupt controller state: {interruptController.GetState()}");
    }

    /// <summary>
    /// Demonstrates dependency injection with custom interrupt controller
    /// </summary>
    public static void DependencyInjectionDemo()
    {
        Console.WriteLine("\n=== Dependency Injection Demo ===");
        Console.WriteLine();

        // Use the composition root to create a CPU
        var compositionRoot = new EmulatorCompositionRoot();
        var cpu = compositionRoot.CreateCpu(new Z80CpuOptions { RomSize = 1024, RamSize = 1024 });
        var memory = compositionRoot.GetService<Z80MemoryMap>();
        var customInterrupt = cpu.Interrupt;
        
        // Configure the interrupt controller
        customInterrupt.SetInterruptMode(Z80InterruptMode.Mode2);
        customInterrupt.InterruptVector = 0xFE; // Custom vector table base

        // Set up vector table for IM2
        var vectorTableBase = (ushort)0xFE00;
        memory.WriteByte(vectorTableBase, 0x00); // Low byte of handler address
        memory.WriteByte((ushort)(vectorTableBase + 1), 0x10); // High byte of handler address

        // Set up simple interrupt handler at 0x1000
        memory.WriteByte(0x1000, Z80OpCode.EI);     // Re-enable interrupts
        memory.WriteByte(0x1001, Z80OpCode.RETI);   // Return from interrupt

        var program = new byte[]
        {
            Z80OpCode.EI,           // Enable interrupts
            Z80OpCode.NOP,          // EI delay
            Z80OpCode.HALT,         // HALT
        };
        memory.LoadRom(program);

        Console.WriteLine("Using custom interrupt controller with IM2...");
        Console.WriteLine($"Initial state: {customInterrupt.GetState()}");

        // Execute initial instructions
        cpu.Step(); // EI
        cpu.Step(); // NOP
        
        Console.WriteLine($"After EI: {customInterrupt.GetState()}");
        
        // Request an interrupt
        customInterrupt.RequestMaskableInterrupt(0x00, 0, new IoDeviceInterruptSource("CUSTOM", "Custom Device"));
        
        Console.WriteLine($"Pending interrupt count: {customInterrupt.PendingInterruptCount}");
        
        // Execute HALT - should be interrupted
        var pcBefore = cpu.PC;
        cpu.Step(); // Should handle interrupt instead of HALT
        
        Console.WriteLine($"PC before interrupt: 0x{pcBefore:X4}");
        Console.WriteLine($"PC after interrupt: 0x{cpu.PC:X4}");
        Console.WriteLine($"Interrupt state: {customInterrupt.GetState()}");
        Console.WriteLine($"CPU is halted: {cpu.Halted}");
        
        Console.WriteLine();
        Console.WriteLine("Dependency injection allows flexible interrupt controller configuration!");
        Console.WriteLine();
    }
}
