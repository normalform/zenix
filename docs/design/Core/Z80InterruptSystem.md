# Z80 Interrupt System

This document describes the Z80 interrupt system and how it is implemented in the Zenix emulator, including the different interrupt modes, timing, and behavior.

## Overview

The Z80 processor supports a sophisticated interrupt system with both maskable and non-maskable interrupts, multiple interrupt modes, and precise timing control. This system is essential for MSX computer emulation where various devices need to interrupt the CPU.

## Z80 Interrupt Types

The Z80 processor supports two types of interrupts:

### 1. Maskable Interrupts (INT)
- Can be enabled/disabled using EI/DI instructions
- Controlled by the IFF1 (Interrupt Flip-Flop 1) flag
- Support three different interrupt modes (IM 0, IM 1, IM 2)
- Used for general device interrupts (keyboard, VDP, sound, etc.)

### 2. Non-Maskable Interrupts (NMI)
- Cannot be disabled by software
- Always vectors to address 0x0066
- Used for critical system events (power failure, reset button)
- Higher priority than maskable interrupts

## Interrupt Modes

The Z80 supports three interrupt modes, selected by the IM instruction:

### Interrupt Mode 0 (IM 0)
- Default mode on reset
- Interrupting device places instruction on data bus
- CPU executes the instruction (typically RST n)
- Most flexible but requires external hardware support

### Interrupt Mode 1 (IM 1)
- CPU automatically jumps to address 0x0038
- Simple and commonly used in many systems
- MSX computers typically use this mode

### Interrupt Mode 2 (IM 2)
- Uses interrupt vector table
- I register holds high byte of vector table address
- Interrupting device provides low byte
- Allows up to 128 different interrupt vectors

## Interrupt Processing Sequence

### Maskable Interrupt (INT)
1. External device asserts INT line
2. CPU checks IFF1 flag and EI delay
3. If interrupts enabled, CPU:
   - Disables interrupts (IFF1 = 0, IFF2 = IFF1)
   - Pushes PC onto stack
   - Jumps to interrupt vector based on current IM mode
4. Interrupt service routine executes
5. RETI instruction restores interrupts and returns

### Non-Maskable Interrupt (NMI)
1. External device asserts NMI line
2. CPU immediately:
   - Saves IFF1 to IFF2, clears IFF1
   - Pushes PC onto stack
   - Jumps to 0x0066
3. NMI service routine executes
4. RETN instruction restores IFF1 from IFF2 and returns

## Timing and Priority

### Interrupt Recognition
- Interrupts are sampled at the end of each instruction
- EI instruction has one-instruction delay before taking effect
- NMI has higher priority than INT

### Cycle Timing
- NMI: 11 T-states
- INT Mode 0: 13 T-states (plus instruction execution)
- INT Mode 1: 13 T-states
- INT Mode 2: 19 T-states

## MSX Interrupt Usage

In MSX computers, interrupts are used for:

### VDP (Video Display Processor)
- Generates interrupt at end of each frame (~60Hz)
- Used for vertical blank processing
- Typically uses IM 1 mode

### Keyboard
- Interrupt when key pressed/released
- Lower priority than VDP

### Sound (PSG/FM)
- Timer-based interrupts for music/sound effects

## Implementation in Zenix
The Zenix emulator implements the Z80 interrupt system with the following components:

### Core Components

1. **IZ80Interrupt Interface** - Defines the interrupt controller contract
2. **Z80Interrupt Class** - Concrete implementation of Z80 interrupt behavior
3. **AsyncInterruptEmulator** - Allows asynchronous interrupt scheduling
4. **Z80InterruptRequest** - Represents an interrupt request with strongly-typed source metadata

### Key Features

- **Accurate Timing**: Implements proper T-state counts for all interrupt types
- **EI Delay**: Correctly handles the one-instruction delay after EI
- **Priority Handling**: NMI always has priority over maskable interrupts
- **Mode Support**: Full support for IM 0, IM 1, and IM 2
- **State Management**: Proper IFF1/IFF2 flag handling

### Example Usage

```csharp
// Create CPU with interrupt controller
var interrupt = new Z80Interrupt();
var cpu = new Z80Cpu(memory, interrupt);

// Set interrupt mode (typical for MSX)
interrupt.SetInterruptMode(Z80InterruptMode.Mode1);

// Enable interrupts
cpu.Step(); // EI instruction
cpu.Step(); // Next instruction (EI delay in effect)

// Now interrupts can be processed
interrupt.RequestMaskableInterrupt(0xFF, 0, "VDP");

// CPU will handle interrupt on next Step()
cpu.Step(); // Processes the VDP interrupt
```

## Interrupt Source System

### Overview

The Z80 interrupt system uses strongly-typed interrupt sources to provide better type safety, debugging capabilities, and system organization. Instead of using simple strings to identify interrupt sources, the system uses an abstract `InterruptSourceBase` hierarchy.

### InterruptSourceBase Hierarchy

```csharp
public abstract class InterruptSourceBase
{
    public abstract string Id { get; }           // Unique identifier
    public abstract string Name { get; }         // Human-readable name  
    public virtual string Description => Name;   // Detailed description
    public abstract InterruptSourceCategory Category { get; }
}
```

### Built-in Interrupt Sources

#### CPU-Level Sources
- **NmiInterruptSource.Instance** - Non-maskable interrupt from CPU

#### Timer Sources  
- **TimerInterruptSource** - Timer-based interrupts with frequency tracking
```csharp
var timer = new TimerInterruptSource("VBLANK", "VBlank Timer", 60.0);
```

#### Video Display Processor Sources
- **VdpInterruptSource.VerticalBlank** - VDP vertical blank interrupt
- **VdpInterruptSource.HorizontalBlank** - VDP horizontal blank interrupt  
- **VdpInterruptSource.SpriteCollision** - VDP sprite collision interrupt

#### I/O Device Sources
- **IoDeviceInterruptSource** - Generic I/O device interrupts
```csharp
var keyboard = new IoDeviceInterruptSource("KEYBOARD", "Keyboard", 0x40);
```

#### Test Sources
- **TestInterruptSource** - For unit testing and debugging (located in test project)
```csharp
var test = new TestInterruptSource("MOCK", "Mock device for testing");
```

### Updated Example Usage

```csharp
// Create CPU with interrupt controller
var interrupt = new Z80Interrupt();
var cpu = new Z80Cpu(memory, interrupt);

// Set interrupt mode (typical for MSX)
interrupt.SetInterruptMode(Z80InterruptMode.Mode1);

// Enable interrupts
cpu.Step(); // EI instruction
cpu.Step(); // Next instruction (EI delay in effect)

// Request interrupts with typed sources
interrupt.RequestMaskableInterrupt(0xFF, 5, VdpInterruptSource.VerticalBlank);
interrupt.RequestMaskableInterrupt(0xC7, 10, new IoDeviceInterruptSource("PSG", "Sound Chip"));
interrupt.RequestNonMaskableInterrupt(NmiInterruptSource.Instance);

// CPU will handle interrupts on next Step() calls
cpu.Step(); // Processes highest priority interrupt
```

### Benefits of Typed Sources

1. **Type Safety**: Compile-time checking prevents string typos
2. **IntelliSense**: Better IDE support with autocompletion
3. **Categorization**: Sources are grouped by category for better organization
4. **Debugging**: Rich metadata helps with troubleshooting
5. **Extensibility**: Easy to add new source types for specific systems

## Instruction Reference

### Interrupt Control Instructions

| Instruction | Description | Cycles |
|------------|-------------|---------|
| EI | Enable interrupts (IFF1 = IFF2 = 1) | 4 |
| DI | Disable interrupts (IFF1 = IFF2 = 0) | 4 |
| IM 0 | Set interrupt mode 0 | 8 |
| IM 1 | Set interrupt mode 1 | 8 |
| IM 2 | Set interrupt mode 2 | 8 |
| RETI | Return from interrupt | 14 |
| RETN | Return from NMI | 14 |

### RST Instructions (Used in IM 0)

| Instruction | Vector | Cycles |
|------------|--------|---------|
| RST 00H | 0x0000 | 11 |
| RST 08H | 0x0008 | 11 |
| RST 10H | 0x0010 | 11 |
| RST 18H | 0x0018 | 11 |
| RST 20H | 0x0020 | 11 |
| RST 28H | 0x0028 | 11 |
| RST 30H | 0x0030 | 11 |
| RST 38H | 0x0038 | 11 |

## References

- Z80 CPU User Manual (Zilog)
- MSX Technical Data Book
- "Programming the Z80" by Rodnay Zaks
