// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.App;
using Zenix.Core;

namespace Zenix.Demos;

/// <summary>
/// Demonstrates the cycle counting capabilities of the Z80 CPU emulator
/// and verifies that it can accurately track more than 10 years of operation at 4MHz.
/// </summary>
public class CycleCountingDemo
{
        public static void RunDemo()
        {
            Console.WriteLine("Z80 CPU Cycle Counting Demonstration");
            Console.WriteLine("====================================");
            Console.WriteLine();

            CalculateCycleRequirements();
            Console.WriteLine();
            DemonstrateCpuCycleTracking();
            Console.WriteLine();
            VerifyLongTermOperation();
        }

        private static void CalculateCycleRequirements()
        {
            Console.WriteLine("Time Period Calculations at 4MHz:");
            Console.WriteLine("---------------------------------");

            const uint clockFreq = Z80CycleTiming.CLOCK_FREQUENCY_HZ;
            
            // Time periods in seconds
            var periods = new[]
            {
                ("1 second", 1.0),
                ("1 minute", 60.0),
                ("1 hour", 3600.0),
                ("1 day", 86400.0),
                ("1 year", 365.25 * 24 * 3600),
                ("10 years", 10 * 365.25 * 24 * 3600),
                ("50 years", 50 * 365.25 * 24 * 3600)
            };

            foreach (var (name, seconds) in periods)
            {
                var cycles = (ulong)(clockFreq * seconds);
                Console.WriteLine($"{name,10}: {cycles,20:N0} cycles");
            }

            Console.WriteLine();
            Console.WriteLine("Counter Capacity Analysis:");
            Console.WriteLine("-------------------------");
            
            var maxUInt32 = uint.MaxValue;
            var maxUInt64 = ulong.MaxValue;
            
            var maxTimeUInt32 = (double)maxUInt32 / clockFreq / (365.25 * 24 * 3600);
            var maxTimeUInt64 = (double)maxUInt64 / clockFreq / (365.25 * 24 * 3600);
            
            Console.WriteLine($"uint32 max value: {maxUInt32,20:N0}");
            Console.WriteLine($"uint32 max years: {maxTimeUInt32,20:F2}");
            Console.WriteLine($"uint64 max value: {maxUInt64,20:N0}");
            Console.WriteLine($"uint64 max years: {maxTimeUInt64,20:F2}");
            
            var cycles10Years = (ulong)(clockFreq * 10 * 365.25 * 24 * 3600);
            var percentage = (double)cycles10Years / maxUInt64 * 100;
            
            Console.WriteLine();
            Console.WriteLine($"10-year requirement: {cycles10Years,20:N0} cycles");
            Console.WriteLine($"Percentage of uint64: {percentage,19:F6}%");
            Console.WriteLine($"Safety margin: {100 - percentage,23:F6}%");
        }

        private static void DemonstrateCpuCycleTracking()
        {
            Console.WriteLine("CPU Cycle Tracking Demo:");
            Console.WriteLine("-----------------------");

            var compositionRoot = new EmulatorCompositionRoot();
            var cpu = compositionRoot.CreateCpu(new Z80CpuOptions { RomSize = 1024, RamSize = 1024 });
            var memory = compositionRoot.GetService<Z80MemoryMap>();

            // Load a simple program
            byte[] program =
            [
                Z80OpCode.NOP,          // 4 cycles
                Z80OpCode.LD_A_n, 0x42, // 7 cycles  
                Z80OpCode.NOP,          // 4 cycles
                Z80OpCode.HALT          // 4 cycles
            ];
            
            memory.LoadRom(program);

            Console.WriteLine("Initial state:");
            Console.WriteLine($"  Total cycles: {cpu.TotalCycles}");
            Console.WriteLine($"  Emulated time: {cpu.EmulatedTimeSeconds:F9} seconds");
            Console.WriteLine();

            // Execute instructions and show cycle progression
            var instructions = new[] { "NOP", "LD A, 0x42", "NOP", "HALT" };
            var expectedCycles = new[] { 4UL, 7UL, 4UL, 4UL };
            
            for (int i = 0; i < instructions.Length; i++)
            {
                var cyclesBefore = cpu.TotalCycles;
                cpu.Step();
                var cyclesAfter = cpu.TotalCycles;
                var cyclesThisInstruction = cyclesAfter - cyclesBefore;
                
                Console.WriteLine($"After {instructions[i]}:");
                Console.WriteLine($"  Cycles this instruction: {cyclesThisInstruction} (expected: {expectedCycles[i]})");
                Console.WriteLine($"  Total cycles: {cyclesAfter}");
                Console.WriteLine($"  Emulated time: {cpu.EmulatedTimeSeconds:F9} seconds");
                Console.WriteLine($"  Effective frequency: {cpu.EffectiveClockFrequency:F0} Hz");
                Console.WriteLine();
            }
        }

        private static void VerifyLongTermOperation()
        {
            Console.WriteLine("Long-term Operation Verification:");
            Console.WriteLine("--------------------------------");

            // Simulate running for a very long time by directly setting a large cycle count
            // (We can't actually run for 10 years in a demo!)
            
            var compositionRoot = new EmulatorCompositionRoot();
            var cpu = compositionRoot.CreateCpu();
            var memory = compositionRoot.GetService<Z80MemoryMap>();
            
            // Calculate cycles for various long periods
            const uint clockFreq = Z80CycleTiming.CLOCK_FREQUENCY_HZ;
            var oneYear = (ulong)(clockFreq * 365.25 * 24 * 3600);
            var tenYears = oneYear * 10;
            
            Console.WriteLine("Theoretical long-term operation:");
            Console.WriteLine($"1 year at 4MHz: {oneYear:N0} cycles");
            Console.WriteLine($"10 years at 4MHz: {tenYears:N0} cycles");
            Console.WriteLine($"Maximum uint64: {ulong.MaxValue:N0}");
            Console.WriteLine($"Can handle 10+ years: {tenYears < ulong.MaxValue}");
            Console.WriteLine();
            
            // Test with a reasonable simulation
            byte[] program = [Z80OpCode.NOP]; // Single instruction program
            memory.LoadRom(program);
            
            const int iterations = 100000; // 100,000 instructions
            var startTime = DateTime.UtcNow;
            
            for (int i = 0; i < iterations; i++)
            {
                cpu.Step();
                // NOP will advance PC, but we'll let it run off the end naturally
                // The CPU will just read 0x00 (NOP) from uninitialized memory
            }
            
            var endTime = DateTime.UtcNow;
            var elapsed = endTime - startTime;
            
            Console.WriteLine($"Performance test results:");
            Console.WriteLine($"Executed {iterations:N0} instructions in {elapsed.TotalMilliseconds:F2} ms");
            Console.WriteLine($"Total cycles tracked: {cpu.TotalCycles:N0}");
            Console.WriteLine($"Instructions per second: {iterations / elapsed.TotalSeconds:F0}");
            if (elapsed.TotalSeconds > 0)
            {
                Console.WriteLine($"Cycles per second: {cpu.TotalCycles / elapsed.TotalSeconds:N0}");
            }
        }
    }
