# Z80 Interrupt Interface Design

This document describes the design and implementation of the Z80 interrupt interface (`IZ80Interrupt`) in the Zenix emulator.

---

## 📋 Overview

The `IZ80Interrupt` interface defines the contract for interrupt handling in the Z80 CPU emulation. It provides a clean abstraction layer that separates interrupt logic from CPU implementation, enabling testability and modularity.

---

## 🏗️ Interface Design

### Core Interface Definition

```csharp
namespace Zenix.Core.Interrupt;

/// <summary>
/// Interface for Z80 interrupt controller functionality
/// </summary>
public interface IZ80Interrupt
{
    /// <summary>
    /// Gets or sets whether interrupts are enabled (IFF1 flag)
    /// </summary>
    bool InterruptsEnabled { get; set; }

    /// <summary>
    /// Gets whether an interrupt is currently pending
    /// </summary>
    bool InterruptPending { get; }

    /// <summary>
    /// Gets the current interrupt mode (0, 1, or 2)
    /// </summary>
    int InterruptMode { get; set; }

    /// <summary>
    /// Gets or sets the interrupt page register (I register)
    /// </summary>
    byte InterruptPage { get; set; }

    /// <summary>
    /// Checks for and services pending interrupts
    /// </summary>
    /// <returns>The interrupt vector address, or null if no interrupt</returns>
    ushort? ServiceInterrupt();

    /// <summary>
    /// Triggers a non-maskable interrupt
    /// </summary>
    void TriggerNmi();

    /// <summary>
    /// Requests an interrupt from an external source
    /// </summary>
    /// <param name="source">The interrupt source</param>
    void RequestInterrupt(InterruptSourceBase source);

    /// <summary>
    /// Clears pending interrupts from a source
    /// </summary>
    /// <param name="source">The interrupt source to clear</param>
    void ClearInterrupt(InterruptSourceBase source);
}
```

---

## 🔧 Implementation Architecture

### Primary Implementation: Z80Interrupt

The main implementation `Z80Interrupt` provides:

- **State Management**: Tracks IFF1, IFF2, and interrupt mode
- **Source Management**: Handles multiple interrupt sources with priorities
- **Mode Handling**: Supports all three Z80 interrupt modes (IM 0, IM 1, IM 2)
- **NMI Support**: Non-maskable interrupt processing
- **Thread Safety**: Safe for use in multithreaded environments

### Key Design Principles

1. **Separation of Concerns**: Interrupt logic is separate from CPU implementation
2. **Testability**: Interface allows for easy mocking and unit testing
3. **Extensibility**: Support for multiple interrupt sources and custom behaviors
4. **Performance**: Efficient interrupt checking and processing
5. **Accuracy**: Faithful Z80 interrupt timing and behavior

---

## 🎯 Interrupt Source Management

### Interrupt Source Types

The interface works with various interrupt source types:

- **SystemInterruptSource**: System-level interrupts (reset, etc.)
- **VdpInterruptSource**: Video display processor interrupts
- **TimerInterruptSource**: Timer-based interrupts
- **IoDeviceInterruptSource**: I/O device interrupts
- **NmiInterruptSource**: Non-maskable interrupts
- **NullInterruptSource**: Null object pattern implementation

### Priority Handling

Interrupt sources are prioritized:
1. Non-maskable interrupts (highest priority)
2. System interrupts
3. VDP interrupts
4. Timer interrupts
5. I/O device interrupts (lowest priority)

---

## ⚡ Interrupt Processing Flow

### Maskable Interrupt Processing

1. **Check Conditions**: Verify IFF1 flag and interrupt mode
2. **Source Selection**: Find highest priority pending source
3. **Vector Calculation**: Calculate interrupt vector based on mode
4. **State Update**: Disable interrupts (IFF1 = 0, IFF2 = IFF1)
5. **Return Vector**: Provide vector address to CPU

### Non-Maskable Interrupt Processing

1. **Immediate Processing**: Cannot be disabled
2. **State Preservation**: Save IFF1 to IFF2, clear IFF1
3. **Fixed Vector**: Always vectors to 0x0066
4. **Priority**: Overrides any pending maskable interrupts

---

## 🧪 Testing Strategy

### Interface Testing Benefits

The interface design enables comprehensive testing:

- **Mock Implementations**: Easy to create test doubles
- **Behavior Verification**: Test interrupt timing and priority
- **State Testing**: Verify flag handling and mode changes
- **Integration Testing**: Test with real CPU implementation

### Test Coverage Areas

1. **Mode Switching**: IM 0, IM 1, IM 2 behavior
2. **Enable/Disable**: EI/DI instruction effects
3. **Priority Handling**: Multiple source interactions
4. **NMI Processing**: Non-maskable interrupt behavior
5. **Edge Cases**: Invalid states and error conditions

---

## 📊 Performance Considerations

### Optimization Strategies

- **Fast Path**: Quick check for no pending interrupts
- **Lazy Evaluation**: Defer expensive operations when possible
- **Efficient Collections**: Use appropriate data structures for sources
- **Minimal Allocations**: Avoid memory allocations in hot paths

### Timing Accuracy

The interface supports cycle-accurate timing:
- Interrupt recognition timing
- Mode-specific processing cycles
- EI instruction delay behavior
- NMI processing overhead

---

## 🔄 Usage Examples

### Basic Interrupt Setup

```csharp
// Create interrupt controller
IZ80Interrupt interrupt = new Z80Interrupt();

// Configure interrupt mode
interrupt.InterruptMode = 1;  // IM 1
interrupt.InterruptsEnabled = true;

// Add interrupt sources
var vdpSource = new VdpInterruptSource();
interrupt.RequestInterrupt(vdpSource);
```

### Interrupt Processing in CPU

```csharp
public void ProcessInterrupts()
{
    if (_interrupt.InterruptPending && _interrupt.InterruptsEnabled)
    {
        var vector = _interrupt.ServiceInterrupt();
        if (vector.HasValue)
        {
            // Push PC and jump to interrupt vector
            PushWord(PC);
            PC = vector.Value;
        }
    }
}
```

---

## 🚀 Future Enhancements

### Planned Improvements

1. **Advanced Timing**: More precise cycle counting
2. **Debug Support**: Interrupt tracing and logging
3. **Custom Sources**: Plugin architecture for interrupt sources
4. **State Snapshots**: Save/restore interrupt state
5. **Performance Metrics**: Interrupt frequency and timing analysis

---

## 📚 References

- **Z80 CPU User Manual**: Official interrupt specifications
- **MSX Technical Data Book**: MSX-specific interrupt usage
- **Z80 Family CPU User Manual**: Detailed timing information
- **Zenix Architecture Document**: Overall system design context

---

*This document is part of the Zenix Emulator detailed design documentation. For implementation details, see the source code in `src/Core/Interrupt/`.*
