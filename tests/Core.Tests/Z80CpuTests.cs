// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Xunit;

namespace Zenix.Core.Tests;

public class Z80CpuTests
{
    private Z80Cpu CreateCpuWithMemory(byte[]? program = null)
    {
        var memory = new MsxMemoryMap();
        var options = new Z80CpuOptions { RomSize = 1024, RamSize = 1024 };
        var cpu = new Z80Cpu(memory, options);
        
        if (program != null)
        {
            memory.LoadRom(program);
        }
        
        return cpu;
    }

    #region Basic Operations Tests

    [Fact]
    public void NOP_DoesNothing()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.NOP }); // NOP
        var initialPC = cpu.PC;
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(initialPC + 1, cpu.PC);
    }

    [Fact]
    public void HALT_StopsExecution()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.HALT }); // HALT
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.True(cpu.Halted);
    }

    #endregion

    #region 8-bit Load Immediate Tests

    [Fact]
    public void LD_A_n_LoadsImmediate()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_A_n, 0x42 }); // LD A, 42h
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x42, cpu.A);
    }

    [Fact]
    public void LD_B_n_LoadsImmediate()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_B_n, 0x33 }); // LD B, 33h
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x33, cpu.B);
    }

    [Fact]
    public void LD_C_n_LoadsImmediate()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_C_n, 0x44 }); // LD C, 44h
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x44, cpu.C);
    }

    [Fact]
    public void LD_D_n_LoadsImmediate()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_D_n, 0x55 }); // LD D, 55h
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x55, cpu.D);
    }

    [Fact]
    public void LD_E_n_LoadsImmediate()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_E_n, 0x66 }); // LD E, 66h
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x66, cpu.E);
    }

    [Fact]
    public void LD_H_n_LoadsImmediate()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_H_n, 0x77 }); // LD H, 77h
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x77, cpu.H);
    }

    [Fact]
    public void LD_L_n_LoadsImmediate()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_L_n, 0x88 }); // LD L, 88h
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x88, cpu.L);
    }

    #endregion

    #region 8-bit Register to Register Load Tests

    [Fact]
    public void LD_A_B_CopiesRegister()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_B_n, 0x42, Z80OpCode.LD_A_B }); // LD B, 42h; LD A, B
        
        // Act
        cpu.Step(); // Load B
        cpu.Step(); // Load A from B
        
        // Assert
        Assert.Equal(0x42, cpu.A);
        Assert.Equal(0x42, cpu.B);
    }

    [Fact]
    public void LD_B_A_CopiesRegister()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_A_n, 0x55, Z80OpCode.LD_B_A }); // LD A, 55h; LD B, A
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Load B from A
        
        // Assert
        Assert.Equal(0x55, cpu.A);
        Assert.Equal(0x55, cpu.B);
    }

    [Fact]
    public void LD_C_D_CopiesRegister()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_D_n, 0x99, Z80OpCode.LD_C_D }); // LD D, 99h; LD C, D
        
        // Act
        cpu.Step(); // Load D
        cpu.Step(); // Load C from D
        
        // Assert
        Assert.Equal(0x99, cpu.C);
        Assert.Equal(0x99, cpu.D);
    }

    #endregion

    #region 16-bit Load Tests

    [Fact]
    public void LD_BC_nn_Loads16Bit()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_BC_nn, 0x34, 0x12 }); // LD BC, 1234h
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x12, cpu.B);
        Assert.Equal(0x34, cpu.C);
    }

    [Fact]
    public void LD_DE_nn_Loads16Bit()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_DE_nn, 0x78, 0x56 }); // LD DE, 5678h
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x56, cpu.D);
        Assert.Equal(0x78, cpu.E);
    }

    [Fact]
    public void LD_HL_nn_Loads16Bit()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_HL_nn, 0xBC, 0x9A }); // LD HL, 9ABCh
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x9A, cpu.H);
        Assert.Equal(0xBC, cpu.L);
    }

    [Fact]
    public void LD_SP_nn_Loads16Bit()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_SP_nn, 0xEF, 0xCD }); // LD SP, CDEFh
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0xCDEF, cpu.SP);
    }

    #endregion

    #region Memory Operation Tests

    [Fact]
    public void LD_A_HL_LoadsFromMemory()
    {
        // Arrange
        var program = new byte[1024];
        program[0] = Z80OpCode.LD_HL_nn; // LD HL, 0x100
        program[1] = 0x00;
        program[2] = 0x01;
        program[3] = Z80OpCode.LD_A_HL; // LD A, (HL)
        program[0x100] = 0xAB; // Data at address 0x100
        
        var cpu = CreateCpuWithMemory(program);
        
        // Act
        cpu.Step(); // Load HL
        cpu.Step(); // Load A from (HL)
        
        // Assert
        Assert.Equal(0xAB, cpu.A);
    }

    [Fact]
    public void LD_HL_A_StoresToMemory()
    {
        // Arrange
        var program = new byte[1024];
        program[0] = Z80OpCode.LD_A_n; // LD A, 0x55
        program[1] = 0x55;
        program[2] = Z80OpCode.LD_HL_nn; // LD HL, 0x400 (RAM area)
        program[3] = 0x00;
        program[4] = 0x04;
        program[5] = Z80OpCode.LD_HL_A; // LD (HL), A
        
        var cpu = CreateCpuWithMemory(program);
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Load HL
        cpu.Step(); // Store A to (HL)
        
        // Assert
        // Verify the memory was written
        var memory = (MsxMemoryMap?)cpu.GetType().GetField("_memory", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(cpu);
        Assert.NotNull(memory);
        Assert.Equal(0x55, memory.ReadByte(0x400));
    }

    #endregion

    #region Arithmetic Tests

    [Fact]
    public void ADD_A_B_AddsRegisters()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_A_n, 0x10, Z80OpCode.LD_B_n, 0x20, Z80OpCode.ADD_A_B }); // LD A, 10h; LD B, 20h; ADD A, B
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Load B
        cpu.Step(); // Add B to A
        
        // Assert
        Assert.Equal(0x30, cpu.A);
    }

    [Fact]
    public void ADD_A_n_AddsImmediate()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0x15, 0xC6, 0x25 }); // LD A, 15h; ADD A, 25h
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Add immediate
        
        // Assert
        Assert.Equal(0x3A, cpu.A);
    }

    [Fact]
    public void ADD_SetsCarryFlag()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0xFF, 0xC6, 0x01 }); // LD A, FFh; ADD A, 01h
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Add (should overflow)
        
        // Assert
        Assert.Equal(0x00, cpu.A);
        // Verify carry flag is set (bit 0 of F register)
        Assert.True((cpu.F & 0x01) != 0);
    }

    [Fact]
    public void ADD_SetsZeroFlag()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0x00, 0xC6, 0x00 }); // LD A, 00h; ADD A, 00h
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Add (result is zero)
        
        // Assert
        Assert.Equal(0x00, cpu.A);
        // Verify zero flag is set (bit 6 of F register)
        Assert.True((cpu.F & 0x40) != 0);
    }

    [Fact]
    public void ADD_OverflowSetsZeroFlag()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0xFF, 0xC6, 0x01 }); // LD A, FFh; ADD A, 01h

        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Add (result wraps to 0)

        // Assert
        Assert.Equal(0x00, cpu.A);
        Assert.True((cpu.F & 0x40) != 0); // Zero flag set
    }

    #endregion

    #region Increment/Decrement Tests

    [Fact]
    public void INC_A_IncrementsRegister()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0x42, 0x3C }); // LD A, 42h; INC A
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Increment A
        
        // Assert
        Assert.Equal(0x43, cpu.A);
    }

    [Fact]
    public void INC_SetsZeroFlag()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0xFF, 0x3C }); // LD A, FFh; INC A
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Increment A (should wrap to 0)
        
        // Assert
        Assert.Equal(0x00, cpu.A);
        Assert.True((cpu.F & 0x40) != 0); // Zero flag set
    }

    [Fact]
    public void DEC_A_DecrementsRegister()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0x42, 0x3D }); // LD A, 42h; DEC A
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Decrement A
        
        // Assert
        Assert.Equal(0x41, cpu.A);
    }

    [Fact]
    public void DEC_SetsZeroFlag()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0x01, 0x3D }); // LD A, 01h; DEC A
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Decrement A (should become 0)
        
        // Assert
        Assert.Equal(0x00, cpu.A);
        Assert.True((cpu.F & 0x40) != 0); // Zero flag set
    }

    #endregion

    #region Jump Tests

    [Fact]
    public void JP_nn_JumpsAbsolute()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0xC3, 0x00, 0x02, 0x00, 0x00 }); // JP 0200h
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(0x0200, cpu.PC);
    }

    [Fact]
    public void JR_e_JumpsRelative()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x18, 0x05 }); // JR +5
        var initialPC = cpu.PC;
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal(initialPC + 2 + 5, cpu.PC); // PC after instruction + offset
    }

    [Fact]
    public void JR_Z_JumpsWhenZeroFlagSet()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0x00, 0x87, 0x28, 0x05 }); // LD A, 0; ADD A, A; JR Z, +5
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Add A to A (sets zero flag)
        var pcBeforeJump = cpu.PC;
        cpu.Step(); // Conditional jump
        
        // Assert
        Assert.Equal(pcBeforeJump + 2 + 5, cpu.PC); // Should jump
    }

    [Fact]
    public void JR_Z_DoesNotJumpWhenZeroFlagClear()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0x01, 0x87, 0x28, 0x05 }); // LD A, 1; ADD A, A; JR Z, +5
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Add A to A (does not set zero flag)
        var pcBeforeJump = cpu.PC;
        cpu.Step(); // Conditional jump
        
        // Assert
        Assert.Equal(pcBeforeJump + 2, cpu.PC); // Should not jump
    }

    [Fact]
    public void JR_NZ_JumpsWhenZeroFlagClear()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0x01, 0x87, 0x20, 0x05 }); // LD A, 1; ADD A, A; JR NZ, +5
        
        // Act
        cpu.Step(); // Load A
        cpu.Step(); // Add A to A (does not set zero flag)
        var pcBeforeJump = cpu.PC;
        cpu.Step(); // Conditional jump
        
        // Assert
        Assert.Equal(pcBeforeJump + 2 + 5, cpu.PC); // Should jump
    }

    #endregion

    #region Stack Operation Tests

    [Fact]
    public void PUSH_POP_BC_WorksCorrectly()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x01, 0x34, 0x12, 0xC5, 0x01, 0x00, 0x00, 0xC1 }); 
        // LD BC, 1234h; PUSH BC; LD BC, 0000h; POP BC
        
        // Act
        cpu.Step(); // Load BC with 1234h
        cpu.Step(); // Push BC
        cpu.Step(); // Load BC with 0000h
        cpu.Step(); // Pop BC
        
        // Assert
        Assert.Equal(0x12, cpu.B);
        Assert.Equal(0x34, cpu.C);
    }

    [Fact]
    public void PUSH_POP_DE_WorksCorrectly()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x11, 0x78, 0x56, 0xD5, 0x11, 0x00, 0x00, 0xD1 }); 
        // LD DE, 5678h; PUSH DE; LD DE, 0000h; POP DE
        
        // Act
        cpu.Step(); // Load DE with 5678h
        cpu.Step(); // Push DE
        cpu.Step(); // Load DE with 0000h
        cpu.Step(); // Pop DE
        
        // Assert
        Assert.Equal(0x56, cpu.D);
        Assert.Equal(0x78, cpu.E);
    }

    [Fact]
    public void PUSH_POP_HL_WorksCorrectly()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x21, 0xBC, 0x9A, 0xE5, 0x21, 0x00, 0x00, 0xE1 }); 
        // LD HL, 9ABCh; PUSH HL; LD HL, 0000h; POP HL
        
        // Act
        cpu.Step(); // Load HL with 9ABCh
        cpu.Step(); // Push HL
        cpu.Step(); // Load HL with 0000h
        cpu.Step(); // Pop HL
        
        // Assert
        Assert.Equal(0x9A, cpu.H);
        Assert.Equal(0xBC, cpu.L);
    }

    [Fact]
    public void PUSH_POP_AF_WorksCorrectly()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 0x3E, 0xAA, 0xF5, 0x3E, 0x00, 0xF1 }); 
        // LD A, AAh; PUSH AF; LD A, 00h; POP AF
        
        // Act
        cpu.Step(); // Load A with AAh
        var originalF = cpu.F;
        cpu.Step(); // Push AF
        cpu.Step(); // Load A with 00h
        cpu.Step(); // Pop AF
        
        // Assert
        Assert.Equal(0xAA, cpu.A);
        Assert.Equal(originalF, cpu.F);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Complex_Program_ExecutesCorrectly()
    {
        // Arrange
        // A simple program that loads values, adds them, and stores result
        var program = new byte[] {
            0x3E, 0x10,       // LD A, 10h
            0x06, 0x20,       // LD B, 20h
            0x80,             // ADD A, B (A = 30h)
            0x21, 0x00, 0x02, // LD HL, 0200h
            0x77,             // LD (HL), A
            0x76              // HALT
        };
        
        var cpu = CreateCpuWithMemory(program);
        
        // Act
        // Execute until halt
        while (!cpu.Halted)
        {
            cpu.Step();
        }
        
        // Assert
        Assert.Equal(0x30, cpu.A);
        Assert.True(cpu.Halted);
        
        // Verify memory was written
        var memory = (MsxMemoryMap?)cpu.GetType().GetField("_memory", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(cpu);
        Assert.NotNull(memory);
        Assert.Equal(0x30, memory.ReadByte(0x200));
    }

    [Fact]
    public void Cpu_Executes_Binary_File()
    {
        // Arrange
        var binPath = Path.Join(AppContext.BaseDirectory, "Core.Tests", "Assets", "simple.bin");
        if (File.Exists(binPath))
        {
            var rom = File.ReadAllBytes(binPath);

            var memory = new MsxMemoryMap();
            var options = new Z80CpuOptions { RomSize = rom.Length, RamSize = 256 };
            var cpu = new Z80Cpu(memory, options);
            memory.LoadRom(rom);

            // Act
            int safety = 100;
            while (!cpu.Halted && safety-- > 0)
            {
                cpu.Step();
            }

            // Assert
            Assert.True(cpu.Halted);        
            Assert.Equal(0x42, cpu.A);
        }
    }

    #endregion

    #region Cycle Counting Tests

    [Fact]
    public void TotalCycles_InitiallyZero()
    {
        // Arrange
        var cpu = CreateCpuWithMemory();
        
        // Act & Assert
        Assert.Equal(0UL, cpu.TotalCycles);
    }

    [Fact]
    public void NOP_ConsumesCorrectCycles()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.NOP });
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal((ulong)Z80CycleTiming.NOP, cpu.TotalCycles);
    }

    [Fact]
    public void HALT_ConsumesCorrectCycles()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.HALT });
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal((ulong)Z80CycleTiming.HALT, cpu.TotalCycles);
        
        // Test that halted state continues to consume cycles
        cpu.Step();
        Assert.Equal((ulong)(Z80CycleTiming.HALT * 2), cpu.TotalCycles);
    }

    [Fact]
    public void LoadImmediate_ConsumesCorrectCycles()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_A_n, 0x42 });
        
        // Act
        cpu.Step();
        
        // Assert
        Assert.Equal((ulong)Z80CycleTiming.LD_r_n, cpu.TotalCycles);
        Assert.Equal(0x42, cpu.A);
    }

    [Fact]
    public void MultipleInstructions_AccumulateCycles()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { 
            Z80OpCode.NOP,          // 4 cycles
            Z80OpCode.LD_A_n, 0x42, // 7 cycles
            Z80OpCode.NOP           // 4 cycles
        });
        
        // Act & Assert
        cpu.Step(); // NOP
        Assert.Equal(4UL, cpu.TotalCycles);
        
        cpu.Step(); // LD A, n
        Assert.Equal(11UL, cpu.TotalCycles);
        
        cpu.Step(); // NOP
        Assert.Equal(15UL, cpu.TotalCycles);
    }

    [Fact]
    public void Reset_ClearsCycleCounter()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.NOP, Z80OpCode.NOP });
        cpu.Step();
        cpu.Step();
        Assert.Equal(8UL, cpu.TotalCycles);
        
        // Act
        cpu.Reset();
        
        // Assert
        Assert.Equal(0UL, cpu.TotalCycles);
    }

    [Fact]
    public void EmulatedTimeSeconds_CalculatesCorrectly()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.NOP });
        
        // Act
        cpu.Step(); // 4 cycles
        
        // Assert - 4 cycles at 4MHz should be 1 microsecond
        var expectedTime = 4.0 / Z80CycleTiming.CLOCK_FREQUENCY_HZ;
        Assert.Equal(expectedTime, cpu.EmulatedTimeSeconds, 10);
    }

    [Fact]
    public void ConditionalJump_ConsumesCorrectCycleCount()
    {
        // Arrange - Test JR Z, e when zero flag is set (using DEC to set zero flag)
        var cpu = CreateCpuWithMemory(new byte[] { 
            Z80OpCode.LD_A_n, 0x01, // Load 1 into A
            Z80OpCode.DEC_A,        // Decrement A to 0 (sets zero flag)
            Z80OpCode.JR_Z_e, 0x02  // Jump forward 2 bytes if zero
        });
        
        // Act
        cpu.Step(); // LD A, n
        cpu.Step(); // DEC A (sets zero flag)
        var cyclesAfterDec = cpu.TotalCycles;
        
        cpu.Step(); // JR Z, e (should take taken branch)
        var cyclesAfterJump = cpu.TotalCycles;
        
        // Assert
        var jumpCycles = cyclesAfterJump - cyclesAfterDec;
        Assert.Equal((ulong)Z80CycleTiming.JR_cc_e_taken, jumpCycles);
    }

    [Fact]
    public void ConditionalJump_NotTaken_ConsumesCorrectCycles()
    {
        // Arrange - Test JR Z, e when zero flag is NOT set
        var cpu = CreateCpuWithMemory(new byte[] { 
            Z80OpCode.LD_A_n, 0x01, // Load non-zero value (clears zero flag)
            Z80OpCode.JR_Z_e, 0x02  // Jump forward 2 bytes if zero (won't jump)
        });
        
        // Act
        cpu.Step(); // LD A, n (clears zero flag)
        var cyclesAfterLoad = cpu.TotalCycles;
        
        cpu.Step(); // JR Z, e (should not take branch)
        var cyclesAfterJump = cpu.TotalCycles;
        
        // Assert
        var jumpCycles = cyclesAfterJump - cyclesAfterLoad;
        Assert.Equal((ulong)Z80CycleTiming.JR_cc_e_not_taken, jumpCycles);
    }

    [Fact]
    public void LongRunningEmulation_CycleCounterDoesNotOverflow()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.NOP });
        
        // Act - Simulate a large number of cycles (but not so many that test takes forever)
        const int iterations = 1000000;
        for (int i = 0; i < iterations; i++)
        {
            cpu.Reset();
            cpu.Step();
        }
        
        // Assert - Should be able to handle large cycle counts
        Assert.Equal((ulong)Z80CycleTiming.NOP, cpu.TotalCycles);
        Assert.True(cpu.TotalCycles < Z80CycleTiming.MAX_CYCLE_COUNT);
    }

    #endregion

    #region Self-Assignment Register Load Tests

    [Fact]
    public void LD_A_A_SelfAssignment_PreservesValueAndConsumesCycles()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_A_n, 0x99, Z80OpCode.LD_A_A }); // LD A, 99h; LD A, A
        
        // Act
        cpu.Step(); // Load A with 0x99
        var cyclesBefore = cpu.TotalCycles;
        cpu.Step(); // LD A, A (self-assignment)
        var cyclesAfter = cpu.TotalCycles;
        
        // Assert
        Assert.Equal(0x99, cpu.A); // Value should be preserved
        Assert.Equal((ulong)Z80CycleTiming.LD_r_r, cyclesAfter - cyclesBefore); // Should consume correct cycles
    }

    [Fact]
    public void LD_B_B_SelfAssignment_PreservesValueAndConsumesCycles()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_B_n, 0x77, Z80OpCode.LD_B_B }); // LD B, 77h; LD B, B
        
        // Act
        cpu.Step(); // Load B with 0x77
        var cyclesBefore = cpu.TotalCycles;
        cpu.Step(); // LD B, B (self-assignment)
        var cyclesAfter = cpu.TotalCycles;
        
        // Assert
        Assert.Equal(0x77, cpu.B); // Value should be preserved
        Assert.Equal((ulong)Z80CycleTiming.LD_r_r, cyclesAfter - cyclesBefore); // Should consume correct cycles
    }

    [Fact]
    public void LD_C_C_SelfAssignment_PreservesValueAndConsumesCycles()
    {
        // Arrange
        var cpu = CreateCpuWithMemory(new byte[] { Z80OpCode.LD_C_n, 0x33, Z80OpCode.LD_C_C }); // LD C, 33h; LD C, C
        
        // Act
        cpu.Step(); // Load C with 0x33
        var cyclesBefore = cpu.TotalCycles;
        cpu.Step(); // LD C, C (self-assignment)
        var cyclesAfter = cpu.TotalCycles;
        
        // Assert
        Assert.Equal(0x33, cpu.C); // Value should be preserved
        Assert.Equal((ulong)Z80CycleTiming.LD_r_r, cyclesAfter - cyclesBefore); // Should consume correct cycles
    }

    #endregion
}
