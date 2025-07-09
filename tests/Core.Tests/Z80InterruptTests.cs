// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Moq;
using Xunit;
using Zenix.Core;
using Zenix.Core.Interrupt;

namespace Zenix.Tests.Core.Tests;

/// <summary>
/// Unit tests specifically for the Z80 interrupt controller and related components
/// </summary>
public class Z80InterruptTests
{
    #region Test Helper Methods

    /// <summary>
    /// Creates a test interrupt source for testing purposes
    /// </summary>
    /// <param name="name">Name of the test source</param>
    /// <returns>Test interrupt source</returns>
    private static TestInterruptSource CreateTestSource(string name = "TestDevice")
    {
        return new TestInterruptSource(name);
    }

    /// <summary>
    /// Creates a test device interrupt source
    /// </summary>
    /// <param name="name">Name of the device</param>
    /// <returns>IO device interrupt source</returns>
    private static IoDeviceInterruptSource CreateDeviceSource(string name = "Device")
    {
        return new IoDeviceInterruptSource(name, name);
    }

    #endregion

    #region Z80Interrupt Core Tests

    [Fact]
    public void Z80Interrupt_InitialState_IsCorrect()
    {
        // Arrange & Act
        var interrupt = new Z80Interrupt();

        // Assert
        Assert.Equal(Z80InterruptMode.Mode0, interrupt.InterruptMode);
        Assert.Equal(0, interrupt.InterruptVector);
        Assert.False(interrupt.InterruptsEnabled);
        Assert.False(interrupt.PreviousInterruptsEnabled);
        Assert.Equal(0, interrupt.PendingInterruptCount);
        Assert.False(interrupt.NmiPending);
    }

    [Fact]
    public void EnableInterrupts_SetsCorrectState()
    {
        // Arrange
        var interrupt = new Z80Interrupt();

        // Act
        interrupt.EnableInterrupts();

        // Assert
        Assert.True(interrupt.InterruptsEnabled);
        Assert.True(interrupt.PreviousInterruptsEnabled);
    }

    [Fact]
    public void DisableInterrupts_SetsCorrectState()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.EnableInterrupts();

        // Act
        interrupt.DisableInterrupts();

        // Assert
        Assert.False(interrupt.InterruptsEnabled);
        Assert.False(interrupt.PreviousInterruptsEnabled);
    }

    [Fact]
    public void SetInterruptMode_UpdatesMode()
    {
        // Arrange
        var interrupt = new Z80Interrupt();

        // Act & Assert
        interrupt.SetInterruptMode(Z80InterruptMode.Mode1);
        Assert.Equal(Z80InterruptMode.Mode1, interrupt.InterruptMode);

        interrupt.SetInterruptMode(Z80InterruptMode.Mode2);
        Assert.Equal(Z80InterruptMode.Mode2, interrupt.InterruptMode);

        interrupt.SetInterruptMode(Z80InterruptMode.Mode0);
        Assert.Equal(Z80InterruptMode.Mode0, interrupt.InterruptMode);
    }

    [Fact]
    public void InterruptVector_CanBeSetAndRetrieved()
    {
        // Arrange
        var interrupt = new Z80Interrupt();

        // Act
        interrupt.InterruptVector = 0xAB;

        // Assert
        Assert.Equal(0xAB, interrupt.InterruptVector);
    }

    [Fact]
    public void RequestMaskableInterrupt_IncreasesPendingCount()
    {
        // Arrange
        var interrupt = new Z80Interrupt();

        // Act
        interrupt.RequestMaskableInterrupt();
        interrupt.RequestMaskableInterrupt(0xC7, 5, CreateTestSource("Device2"));

        // Assert
        Assert.Equal(2, interrupt.PendingInterruptCount);
    }

    [Fact]
    public void RequestNonMaskableInterrupt_SetsNmiPending()
    {
        // Arrange
        var interrupt = new Z80Interrupt();

        // Act
        interrupt.RequestNonMaskableInterrupt();

        // Assert
        Assert.True(interrupt.NmiPending);
    }

    [Fact]
    public void ClearMaskableInterrupts_ClearsPendingInterrupts()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.RequestMaskableInterrupt();
        interrupt.RequestMaskableInterrupt();

        // Act
        interrupt.ClearMaskableInterrupts();

        // Assert
        Assert.Equal(0, interrupt.PendingInterruptCount);
    }

    [Fact]
    public void BeforeInstruction_HandlesEIDelay()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.EnableInterrupts();
        interrupt.RequestMaskableInterrupt();

        // Act & Assert - First instruction after EI should not process interrupt
        Assert.False(interrupt.ShouldProcessInterrupt(out _));
        
        interrupt.BeforeInstruction(); // Decrements EI delay
        
        // Second instruction should process interrupt
        Assert.True(interrupt.ShouldProcessInterrupt(out _));
    }

    [Fact]
    public void ShouldProcessInterrupt_NMI_HasHighestPriority()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.EnableInterrupts();
        interrupt.BeforeInstruction(); // Clear EI delay
        interrupt.RequestMaskableInterrupt();
        interrupt.RequestNonMaskableInterrupt();

        // Act
        bool hasInterrupt = interrupt.ShouldProcessInterrupt(out Z80InterruptRequest request);

        // Assert
        Assert.True(hasInterrupt);
        Assert.Equal(Z80InterruptType.NonMaskable, request.Type);
    }

    [Fact]
    public void ShouldProcessInterrupt_MaskableInterrupt_WhenEnabled()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        var testSource = CreateTestSource("TestDevice");
        interrupt.EnableInterrupts();
        interrupt.BeforeInstruction(); // Clear EI delay
        interrupt.RequestMaskableInterrupt(0xC7, 5, testSource);

        // Act
        bool hasInterrupt = interrupt.ShouldProcessInterrupt(out Z80InterruptRequest request);

        // Assert
        Assert.True(hasInterrupt);
        Assert.Equal(Z80InterruptType.Maskable, request.Type);
        Assert.Equal(0xC7, request.Vector);
        Assert.Equal(5, request.Priority);
        Assert.Equal(testSource.Id, request.Source.Id);
    }

    [Fact]
    public void ShouldProcessInterrupt_MaskableInterrupt_NotWhenDisabled()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.RequestMaskableInterrupt();

        // Act
        bool hasInterrupt = interrupt.ShouldProcessInterrupt(out _);

        // Assert
        Assert.False(hasInterrupt);
    }

    [Fact]
    public void ProcessInterrupt_NMI_ReturnsCorrectVector()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.EnableInterrupts();
        var nmiRequest = new Z80InterruptRequest(Z80InterruptType.NonMaskable, 0, 0, NmiInterruptSource.Instance);

        // Act
        byte instruction = interrupt.ProcessInterrupt(nmiRequest, out ushort vector);

        // Assert
        Assert.Equal(0, instruction);
        Assert.Equal((ushort)0x0066, vector);
        Assert.False(interrupt.InterruptsEnabled); // Should disable interrupts
    }

    [Fact]
    public void ProcessInterrupt_IM0_ReturnsInstruction()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        var testSource = CreateDeviceSource("Device");
        interrupt.EnableInterrupts();
        interrupt.SetInterruptMode(Z80InterruptMode.Mode0);
        var request = new Z80InterruptRequest(Z80InterruptType.Maskable, 0xC7, 0, testSource);

        // Act
        byte instruction = interrupt.ProcessInterrupt(request, out ushort vector);

        // Assert
        Assert.Equal(0xC7, instruction);
        Assert.Equal((ushort)0, vector);
        Assert.False(interrupt.InterruptsEnabled); // Should disable interrupts
    }

    [Fact]
    public void ProcessInterrupt_IM1_ReturnsCorrectVector()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        var testSource = CreateDeviceSource("Device");
        interrupt.EnableInterrupts();
        interrupt.SetInterruptMode(Z80InterruptMode.Mode1);
        var request = new Z80InterruptRequest(Z80InterruptType.Maskable, 0xFF, 0, testSource);

        // Act
        byte instruction = interrupt.ProcessInterrupt(request, out ushort vector);

        // Assert
        Assert.Equal(0, instruction);
        Assert.Equal((ushort)0x0038, vector);
        Assert.False(interrupt.InterruptsEnabled); // Should disable interrupts
    }

    [Fact]
    public void ProcessInterrupt_IM2_ReturnsCorrectVector()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        var testSource = CreateDeviceSource("Device");
        interrupt.EnableInterrupts();
        interrupt.SetInterruptMode(Z80InterruptMode.Mode2);
        interrupt.InterruptVector = 0xAB; // I register
        var request = new Z80InterruptRequest(Z80InterruptType.Maskable, 0xCD, 0, testSource);

        // Act
        byte instruction = interrupt.ProcessInterrupt(request, out ushort vector);

        // Assert
        Assert.Equal(0, instruction);
        Assert.Equal((ushort)0xABCD, vector); // (I << 8) | vector
        Assert.False(interrupt.InterruptsEnabled); // Should disable interrupts
    }

    [Fact]
    public void ReturnFromInterrupt_RestoresInterruptState()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        var testSource = CreateDeviceSource("Device");
        interrupt.EnableInterrupts();
        var request = new Z80InterruptRequest(Z80InterruptType.Maskable, 0xFF, 0, testSource);
        interrupt.ProcessInterrupt(request, out _); // Disables interrupts, saves state

        // Act
        interrupt.ReturnFromInterrupt();

        // Assert
        Assert.True(interrupt.InterruptsEnabled); // Should restore state
    }

    [Fact]
    public void ReturnFromNonMaskableInterrupt_RestoresInterruptState()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.EnableInterrupts();
        var nmiRequest = new Z80InterruptRequest(Z80InterruptType.NonMaskable, 0, 0, NmiInterruptSource.Instance);
        interrupt.ProcessInterrupt(nmiRequest, out _); // Disables interrupts, saves state

        // Act
        interrupt.ReturnFromNonMaskableInterrupt();

        // Assert
        Assert.True(interrupt.InterruptsEnabled); // Should restore state
    }

    [Fact]
    public void Reset_ClearsAllState()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.EnableInterrupts();
        interrupt.SetInterruptMode(Z80InterruptMode.Mode2);
        interrupt.InterruptVector = 0xFF;
        interrupt.RequestMaskableInterrupt();
        interrupt.RequestNonMaskableInterrupt();

        // Act
        interrupt.Reset();

        // Assert
        Assert.False(interrupt.InterruptsEnabled);
        Assert.False(interrupt.PreviousInterruptsEnabled);
        Assert.Equal(Z80InterruptMode.Mode0, interrupt.InterruptMode);
        Assert.Equal(0, interrupt.InterruptVector);
        Assert.Equal(0, interrupt.PendingInterruptCount);
        Assert.False(interrupt.NmiPending);
    }

    [Fact]
    public void GetState_ReturnsFormattedString()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.EnableInterrupts();
        interrupt.SetInterruptMode(Z80InterruptMode.Mode1);
        interrupt.InterruptVector = 0xAB;

        // Act
        string state = interrupt.GetState();

        // Assert
        Assert.Contains("IFF1=True", state);
        Assert.Contains("IM=Mode1", state);
        Assert.Contains("I=0xAB", state);
    }

    #endregion

    #region Mock Interrupt Tests

    [Fact]
    public void MockInterrupt_RecordsMethodCalls()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        var testSource = CreateTestSource("TestDevice");
        mockInterrupt.SetupProperty(x => x.InterruptMode);

        // Act
        mockInterrupt.Object.EnableInterrupts();
        mockInterrupt.Object.SetInterruptMode(Z80InterruptMode.Mode1);
        mockInterrupt.Object.RequestMaskableInterrupt(0xC7, 5, testSource);

        // Assert
        mockInterrupt.Verify(x => x.EnableInterrupts(), Times.Once);
        mockInterrupt.Verify(x => x.SetInterruptMode(Z80InterruptMode.Mode1), Times.Once);
        mockInterrupt.Verify(x => x.RequestMaskableInterrupt(0xC7, 5, testSource), Times.Once);
    }

    [Fact]
    public void MockInterrupt_TracksRequestHistory()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        var requestHistory = new List<Z80InterruptRequest>();
        var device1Source = CreateDeviceSource("Device1");
        var device2Source = CreateDeviceSource("Device2");
        
        // Set up the mock to capture interrupt requests
        mockInterrupt.Setup(x => x.RequestInterrupt(It.IsAny<Z80InterruptRequest>()))
                    .Callback<Z80InterruptRequest>(request => requestHistory.Add(request));

        // Act
        var request1 = new Z80InterruptRequest(Z80InterruptType.Maskable, 0xC7, 5, device1Source);
        var request2 = new Z80InterruptRequest(Z80InterruptType.NonMaskable, 0, 0, device2Source);
        
        mockInterrupt.Object.RequestInterrupt(request1);
        mockInterrupt.Object.RequestInterrupt(request2);

        // Assert
        Assert.Equal(2, requestHistory.Count);
        
        var first = requestHistory[0];
        Assert.Equal(Z80InterruptType.Maskable, first.Type);
        Assert.Equal(0xC7, first.Vector);
        Assert.Equal(5, first.Priority);
        Assert.Equal(device1Source.Id, first.Source.Id);

        var second = requestHistory[1];
        Assert.Equal(Z80InterruptType.NonMaskable, second.Type);
        Assert.Equal(device2Source.Id, second.Source.Id);
    }

    [Fact]
    public void MockInterrupt_ShouldProcessInterrupt_Works()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        var mockSource = CreateTestSource("MockDevice");
        var testInterrupt = new Z80InterruptRequest(Z80InterruptType.Maskable, 0xD7, 10, mockSource);
        
        mockInterrupt.Setup(x => x.InterruptsEnabled).Returns(true);
        mockInterrupt.Setup(x => x.ShouldProcessInterrupt(out It.Ref<Z80InterruptRequest>.IsAny))
                    .Returns((out Z80InterruptRequest interrupt) =>
                    {
                        interrupt = testInterrupt;
                        return true;
                    });

        // Act
        bool hasInterrupt = mockInterrupt.Object.ShouldProcessInterrupt(out Z80InterruptRequest result);

        // Assert
        Assert.True(hasInterrupt);
        Assert.Equal(testInterrupt.Type, result.Type);
        Assert.Equal(testInterrupt.Vector, result.Vector);
        Assert.Equal(testInterrupt.Priority, result.Priority);
        Assert.Equal(testInterrupt.Source, result.Source);
    }

    [Fact]
    public void MockInterrupt_PropertySetup_Works()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        mockInterrupt.SetupProperty(x => x.InterruptMode);
        mockInterrupt.SetupProperty(x => x.InterruptVector);
        mockInterrupt.Setup(x => x.InterruptsEnabled).Returns(true);
        mockInterrupt.Setup(x => x.PreviousInterruptsEnabled).Returns(false);
        mockInterrupt.Setup(x => x.PendingInterruptCount).Returns(3);
        mockInterrupt.Setup(x => x.NmiPending).Returns(true);

        // Act & Assert - Test read-only properties
        Assert.True(mockInterrupt.Object.InterruptsEnabled);
        Assert.False(mockInterrupt.Object.PreviousInterruptsEnabled);
        Assert.Equal(3, mockInterrupt.Object.PendingInterruptCount);
        Assert.True(mockInterrupt.Object.NmiPending);
        
        // Test read-write properties
        mockInterrupt.Object.InterruptMode = Z80InterruptMode.Mode2;
        Assert.Equal(Z80InterruptMode.Mode2, mockInterrupt.Object.InterruptMode);
        
        mockInterrupt.Object.InterruptVector = 0xAB;
        Assert.Equal(0xAB, mockInterrupt.Object.InterruptVector);
    }

    [Fact]
    public void MockInterrupt_GetState_ReturnsFormattedString()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        mockInterrupt.Setup(x => x.GetState()).Returns("MOCK: IFF1=True, IM=Mode1, I=0xAB");

        // Act
        string state = mockInterrupt.Object.GetState();

        // Assert
        Assert.Equal("MOCK: IFF1=True, IM=Mode1, I=0xAB", state);
        mockInterrupt.Verify(x => x.GetState(), Times.Once);
    }

    #endregion

    #region AsyncInterruptEmulator Tests

    [Fact]
    public async Task AsyncInterruptEmulator_SchedulesMaskableInterrupt()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        var capturedRequests = new List<Z80InterruptRequest>();
        var testSource = CreateTestSource("TestDevice");
        
        mockInterrupt.Setup(x => x.RequestInterrupt(It.IsAny<Z80InterruptRequest>()))
                    .Callback<Z80InterruptRequest>(request => capturedRequests.Add(request));
        
        using var emulator = new AsyncInterruptEmulator(mockInterrupt.Object);
        
        var interrupted = false;
        emulator.InterruptEmulated += (_, _) => interrupted = true;

        // Act
        emulator.ScheduleMaskableInterrupt(TimeSpan.FromMilliseconds(10), 0xC7, 5, testSource);
        
        // Wait for interrupt to be processed
        await Task.Delay(50);

        // Assert
        Assert.True(interrupted);
        Assert.Contains(capturedRequests, r => 
            r.Type == Z80InterruptType.Maskable && 
            r.Vector == 0xC7 && 
            r.Priority == 5 && 
            r.Source.Id == testSource.Id);
    }

    [Fact] 
    public async Task AsyncInterruptEmulator_SchedulesNMI()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        var capturedRequests = new List<Z80InterruptRequest>();
        var testSource = CreateTestSource("TestNMI");
        
        mockInterrupt.Setup(x => x.RequestInterrupt(It.IsAny<Z80InterruptRequest>()))
                    .Callback<Z80InterruptRequest>(request => capturedRequests.Add(request));
        
        using var emulator = new AsyncInterruptEmulator(mockInterrupt.Object);
        
        var interruptEmulated = false;
        Z80InterruptRequest? capturedRequest = null;
        emulator.InterruptEmulated += (_, args) => 
        {
            interruptEmulated = true;
            capturedRequest = args.InterruptRequest;
        };

        // Act
        emulator.ScheduleNonMaskableInterrupt(TimeSpan.FromMilliseconds(10), testSource);
        
        // Wait for interrupt to be processed
        await Task.Delay(50);

        // Assert
        Assert.True(interruptEmulated);
        Assert.NotNull(capturedRequest);
        Assert.Equal(Z80InterruptType.NonMaskable, capturedRequest.Value.Type);
        Assert.Equal(testSource.Id, capturedRequest.Value.Source.Id);
    }

    [Fact]
    public async Task AsyncInterruptEmulator_HandlesRepeatingInterrupts()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        var capturedRequests = new List<Z80InterruptRequest>();
        var timerSource = new TimerInterruptSource("Timer", "Test Timer", 50.0);
        
        mockInterrupt.Setup(x => x.RequestInterrupt(It.IsAny<Z80InterruptRequest>()))
                    .Callback<Z80InterruptRequest>(request => capturedRequests.Add(request));
        
        using var emulator = new AsyncInterruptEmulator(mockInterrupt.Object);
        
        var interruptCount = 0;
        emulator.InterruptEmulated += (_, _) => Interlocked.Increment(ref interruptCount);

        // Act
        emulator.ScheduleRepeatingInterrupt(
            TimeSpan.FromMilliseconds(10), 
            TimeSpan.FromMilliseconds(20), 
            0xFF,
            0,
            timerSource);
        
        // Wait for multiple interrupts
        await Task.Delay(100);

        // Assert
        Assert.True(interruptCount >= 3); // Should have fired at least 3 times
        Assert.True(capturedRequests.Count >= 3);
        Assert.All(capturedRequests, r => Assert.Contains("TIMER", r.Source.Id));
    }

    [Fact]
    public void AsyncInterruptEmulator_EmulatesCommonDevices()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        var capturedRequests = new List<Z80InterruptRequest>();
        var requestReceived = new ManualResetEventSlim();
        
        mockInterrupt.Setup(x => x.RequestInterrupt(It.IsAny<Z80InterruptRequest>()))
                    .Callback<Z80InterruptRequest>(request => 
                    {
                        capturedRequests.Add(request);
                        if (capturedRequests.Count >= 3) // We expect 3 interrupts
                        {
                            requestReceived.Set();
                        }
                    });
        
        using var emulator = new AsyncInterruptEmulator(mockInterrupt.Object);

        // Act
        emulator.EmulateKeyboardInterrupt();
        emulator.EmulateVdpInterrupt();
        emulator.EmulateSystemReset();

        // Wait for all interrupts to be processed (with timeout)
        var success = requestReceived.Wait(TimeSpan.FromSeconds(2));
        Assert.True(success, "Timeout waiting for interrupts to be processed");

        // Assert
        Assert.Contains(capturedRequests, r => r.Source.Id.Contains("KEYBOARD"));
        Assert.Contains(capturedRequests, r => r.Source.Id.Contains("VDP"));
        Assert.Contains(capturedRequests, r => r.Source.Id.Contains("SYSTEM") && r.Type == Z80InterruptType.NonMaskable);
    }

    [Fact]
    public void AsyncInterruptEmulator_ClearPendingRequests_Works()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        using var emulator = new AsyncInterruptEmulator(mockInterrupt.Object);

        // Act
        emulator.ScheduleMaskableInterrupt(TimeSpan.FromSeconds(10)); // Long delay
        var countBefore = emulator.PendingRequestCount;
        emulator.ClearPendingRequests();
        var countAfter = emulator.PendingRequestCount;

        // Assert
        Assert.True(countBefore > 0);
        Assert.Equal(0, countAfter);
    }

    [Fact]
    public void AsyncInterruptEmulator_RequiresInterruptController()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AsyncInterruptEmulator(null!));
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void InterruptPriority_OrderingWorks()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        var lowPrioritySource = CreateTestSource("LowPriority");
        var mediumPrioritySource = CreateTestSource("MediumPriority");
        var highPrioritySource = CreateTestSource("HighPriority");
        
        interrupt.EnableInterrupts();
        interrupt.BeforeInstruction(); // Clear EI delay

        // Add interrupts in reverse priority order
        interrupt.RequestMaskableInterrupt(0xC7, 10, lowPrioritySource);
        interrupt.RequestMaskableInterrupt(0xCF, 5, mediumPrioritySource);
        interrupt.RequestMaskableInterrupt(0xD7, 1, highPrioritySource);

        // Act & Assert - Should process highest priority first
        Assert.True(interrupt.ShouldProcessInterrupt(out Z80InterruptRequest first));
        Assert.Equal(highPrioritySource.Id, first.Source.Id);
        Assert.Equal(1, first.Priority);

        Assert.True(interrupt.ShouldProcessInterrupt(out Z80InterruptRequest second));
        Assert.Equal(mediumPrioritySource.Id, second.Source.Id);
        Assert.Equal(5, second.Priority);

        Assert.True(interrupt.ShouldProcessInterrupt(out Z80InterruptRequest third));
        Assert.Equal(lowPrioritySource.Id, third.Source.Id);
        Assert.Equal(10, third.Priority);
    }

    [Fact]
    public void InterruptStateTransitions_WorkCorrectly()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        var testSource = CreateTestSource("Test");

        // Test EI -> interrupt -> RETI cycle
        interrupt.EnableInterrupts();
        Assert.True(interrupt.InterruptsEnabled);
        Assert.True(interrupt.PreviousInterruptsEnabled);

        // Process interrupt (should save and disable interrupts)
        var request = new Z80InterruptRequest(Z80InterruptType.Maskable, 0xFF, 0, testSource);
        interrupt.ProcessInterrupt(request, out _);
        Assert.False(interrupt.InterruptsEnabled);
        Assert.True(interrupt.PreviousInterruptsEnabled); // Should be saved

        // Return from interrupt (should restore)
        interrupt.ReturnFromInterrupt();
        Assert.True(interrupt.InterruptsEnabled); // Should be restored
    }

    [Fact]
    public void NMI_CanInterruptEvenWhenDisabled()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.DisableInterrupts();
        interrupt.RequestNonMaskableInterrupt();

        // Act
        bool hasInterrupt = interrupt.ShouldProcessInterrupt(out Z80InterruptRequest request);

        // Assert
        Assert.True(hasInterrupt);
        Assert.Equal(Z80InterruptType.NonMaskable, request.Type);
    }

    [Fact]
    public void MultipleNMI_OnlyProcessedOnce()
    {
        // Arrange
        var interrupt = new Z80Interrupt();
        interrupt.RequestNonMaskableInterrupt();
        interrupt.RequestNonMaskableInterrupt(); // Second NMI

        // Act
        bool firstNmi = interrupt.ShouldProcessInterrupt(out _);
        bool secondNmi = interrupt.ShouldProcessInterrupt(out _);

        // Assert
        Assert.True(firstNmi);
        Assert.False(secondNmi); // Second NMI should not be processed until first is complete
    }

    #endregion

    #region CPU-Interrupt Integration Tests

    private Z80Cpu CreateCpuWithMemory(byte[]? program = null, IZ80Interrupt? interrupt = null)
    {
        var memory = new Z80MemoryMap();
        var options = new Z80CpuOptions { RomSize = 1024, RamSize = 1024 };
        var cpu = new Z80Cpu(memory, interrupt ?? new Z80Interrupt(), options);
        
        if (program != null)
        {
            memory.LoadRom(program);
        }
        
        return cpu;
    }

    [Fact]
    public void CPU_ExecutesInterruptInstructions()
    {
        // Test that CPU can execute interrupt-related opcodes
        var cpu = CreateCpuWithMemory([Z80OpCode.EI, Z80OpCode.DI]);
        
        // Act
        cpu.Step(); // EI
        Assert.True(cpu.Interrupt.InterruptsEnabled);
        
        cpu.Step(); // DI
        Assert.False(cpu.Interrupt.InterruptsEnabled);
    }

    [Fact]
    public void CPU_HandlesInterruptDuringExecution()
    {
        // Arrange
        var cpu = CreateCpuWithMemory([Z80OpCode.EI, Z80OpCode.NOP, Z80OpCode.NOP]);
        cpu.Interrupt.SetInterruptMode(Z80InterruptMode.Mode1);
        cpu.Step(); // EI
        cpu.Step(); // NOP (EI delay)
        var originalSP = cpu.SP;
        
        // Act
        cpu.Interrupt.RequestMaskableInterrupt();
        cpu.Step(); // Should handle interrupt instead of executing NOP
        
        // Assert
        Assert.Equal((ushort)0x0038, cpu.PC); // IM1 vector
        Assert.Equal((ushort)(originalSP - 2), cpu.SP); // PC should be pushed
        Assert.False(cpu.Interrupt.InterruptsEnabled); // IFF1 should be disabled
    }

    [Fact]
    public void CPU_HALTWakesUpOnInterrupt()
    {
        // Arrange
        var cpu = CreateCpuWithMemory([Z80OpCode.EI, Z80OpCode.HALT]);
        cpu.Step(); // EI
        cpu.Step(); // HALT
        Assert.True(cpu.Halted);
        
        // Act
        cpu.Interrupt.RequestMaskableInterrupt();
        cpu.Step(); // Should wake up from HALT and handle interrupt
        
        // Assert
        Assert.False(cpu.Halted);
        Assert.Equal((ushort)0x0038, cpu.PC); // Interrupt handled
    }

    [Fact]
    public void CPU_CanUseCustomInterruptController()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        mockInterrupt.Setup(x => x.Reset()).Verifiable();
        
        var memory = new Z80MemoryMap();
        var cpu = new Z80Cpu(memory, mockInterrupt.Object, new Z80CpuOptions { RomSize = 1024, RamSize = 1024 });

        // Act
        cpu.Reset();

        // Assert
        Assert.Same(mockInterrupt.Object, cpu.Interrupt);
        // Reset is called once during CPU construction and once when we call cpu.Reset()
        mockInterrupt.Verify(x => x.Reset(), Times.AtLeast(1));
    }

    [Fact]
    public void CPU_WorksWithMockInterrupt()
    {
        // Arrange
        var mockInterrupt = new Mock<IZ80Interrupt>();
        var testSource = CreateTestSource("TestSource");
        
        // Set up the mock to return the expected interrupt when requested
        var pendingInterrupt = new Z80InterruptRequest(Z80InterruptType.Maskable, 0xFF, 0, testSource);
        var shouldProcessCalls = 0;
        
        mockInterrupt.Setup(x => x.EnableInterrupts()).Verifiable();
        mockInterrupt.Setup(x => x.BeforeInstruction()).Verifiable();
        mockInterrupt.Setup(x => x.ShouldProcessInterrupt(out It.Ref<Z80InterruptRequest>.IsAny))
                    .Returns((out Z80InterruptRequest request) =>
                    {
                        shouldProcessCalls++;
                        if (shouldProcessCalls == 2) // Second call (after EI delay)
                        {
                            request = pendingInterrupt;
                            return true;
                        }
                        request = default;
                        return false;
                    });
        
        var cpu = CreateCpuWithMemory([Z80OpCode.EI, Z80OpCode.NOP], mockInterrupt.Object);

        // Act
        cpu.Step(); // EI
        cpu.Step(); // NOP - but should process interrupt instead

        // Assert
        mockInterrupt.Verify(x => x.EnableInterrupts(), Times.Once);
        mockInterrupt.Verify(x => x.BeforeInstruction(), Times.AtLeastOnce);
        mockInterrupt.Verify(x => x.ShouldProcessInterrupt(out It.Ref<Z80InterruptRequest>.IsAny), Times.AtLeastOnce);
    }

    #endregion
}
