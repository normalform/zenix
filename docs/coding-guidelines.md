# 🧭 Coding Guidelines – Zenix Emulator

This document defines coding standards, patterns, and best practices for the Zenix MSX emulator project to ensure consistency, maintainability, and high code quality.

> 📝 **Status**: The Zenix codebase has been fully refactored to comply with these guidelines. All 90 tests pass, IDE0130 namespace compliance is enforced, and the project follows modern C# conventions with immutable records, clean architecture, and comprehensive documentation.

---

## 🎯 Core Principles

### 1. **Functional Programming First**
- Prefer immutable data structures over mutable state
- Use pure functions where possible
- Minimize side effects and shared mutable state
- Favor composition over inheritance

### 2. **Domain-Driven Design**
- Keep domain logic separate from infrastructure concerns
- Use clear, domain-specific language in code and naming
- Model real-world concepts accurately
- Apply the Onion Architecture pattern

### 3. **Testability and Reliability**
- Design for dependency injection
- Use interfaces for abstraction
- Write comprehensive unit tests
- Apply defensive programming practices

### 4. **Performance and Efficiency**
- Optimize for the common case
- Use cycle-accurate timing where required
- Minimize memory allocations in hot paths
- Profile and measure performance impact

---

## 📦 Data Types and Immutability

### Immutable Data Structures

**ALWAYS use `record` types for data transfer objects, configuration, and value types:**

```csharp
// ✅ Good: Immutable record
public record Z80CpuOptions(
    double ClockMHz = 3.58,
    int RomSize = 0x10000,
    int RamSize = 0x10000);

// ❌ Avoid: Mutable class with setters
public class Z80CpuOptions
{
    public double ClockMHz { get; set; } = 3.58;
    public int RomSize { get; set; } = 0x10000;
    public int RamSize { get; set; } = 0x10000;
}
```

### When to Use Immutable Records

Use `record` types for:
- **Configuration objects**: `EmulatorConfiguration`, `CpuConfiguration`
- **Data transfer objects**: Inter-component communication
- **Value objects**: Interrupt requests, memory addresses, timing data
- **API request/response models**: External interface contracts
- **Test data**: Test fixtures and expected results

### When to Use Mutable Classes

Use `class` with mutable state only for:
- **Stateful services**: CPU emulator, memory map, interrupt controller
- **Infrastructure**: Logging, file I/O, network communication
- **Performance-critical components**: Where allocation overhead matters
- **Legacy integration**: When interfacing with mutable external APIs

### Configuration-Compatible Records

For records that need to work with Microsoft's Configuration system, use parameterless constructors with `init` properties:

```csharp
// ✅ Good: Configuration-compatible record
public record CpuConfiguration
{
    public int RomSize { get; init; } = 32 * 1024;
    public int RamSize { get; init; } = 64 * 1024;
    public double ClockMHz { get; init; } = 3.58;
    
    public Z80CpuOptions ToZ80CpuOptions() => new(ClockMHz, RomSize, RamSize);
}

// ❌ Avoid: Primary constructor that breaks configuration binding
public record CpuConfiguration(int RomSize = 32 * 1024, int RamSize = 64 * 1024);
```

### Performance-Critical Data Structures

For high-frequency, small data structures in hot paths, prefer immutable structs over records:

```csharp
// ✅ Good: Immutable struct for performance-critical data
public struct Z80InterruptRequest
{
    public Z80InterruptType Type { get; init; }
    public byte Vector { get; init; }
    public int Priority { get; init; }
    public InterruptSourceBase Source { get; init; }
    
    public Z80InterruptRequest(Z80InterruptType type, byte vector = 0, int priority = 0, InterruptSourceBase? source = null)
    {
        Type = type;
        Vector = vector;
        Priority = priority;
        Source = source ?? NullInterruptSource.Instance;
    }
}

// ❌ Avoid: Records for frequently allocated small objects
public record Z80InterruptRequest(Z80InterruptType Type, byte Vector = 0, int Priority = 0);
```

---

## 🎯 Design Patterns

### Null Object Pattern

Use the Null Object pattern to eliminate null checks and provide safe defaults:

```csharp
// ✅ Good: Null Object pattern
using Zenix.Core.Interrupt;

namespace Zenix.Core.Interrupt;

public abstract class InterruptSourceBase
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public virtual bool IsNull => false;
}

public sealed class NullInterruptSource : InterruptSourceBase
{
    public static readonly NullInterruptSource Instance = new();
    private NullInterruptSource() { }
    
    public override string Id => "NULL";
    public override string Name => "No Source";
    public override bool IsNull => true;
}

// Usage with non-nullable parameters
public void RequestInterrupt(InterruptSourceBase source) 
{
    // source is never null, NullInterruptSource.Instance used as default
    if (source.IsNull) 
    {
        // Handle null case gracefully without exceptions
        return; 
    }
    // Process real interrupt source
}
```

### Factory Pattern with DI

```csharp
// ✅ Good: Factory with dependency injection
public interface IEmulatorFactory
{
    Z80Cpu CreateCpu(CpuConfiguration config);
    IZ80Interrupt CreateInterruptController(InterruptConfiguration config);
}

public class EmulatorFactory : IEmulatorFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public EmulatorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public Z80Cpu CreateCpu(CpuConfiguration config)
    {
        var memoryMap = _serviceProvider.GetRequiredService<Z80MemoryMap>();
        var options = config.ToZ80CpuOptions();
        return new Z80Cpu(memoryMap, options);
    }
}
```

### Builder Pattern for Complex Configuration

```csharp
// ✅ Good: Builder for complex scenarios
public class EmulatorConfigurationBuilder
{
    private CpuConfiguration _cpuConfig = new();
    private InterruptConfiguration _interruptConfig = new();
    
    public EmulatorConfigurationBuilder WithCpu(CpuConfiguration config)
    {
        _cpuConfig = config;
        return this;
    }
    
    public EmulatorConfiguration Build() => new(_cpuConfig, _interruptConfig);
}
```

---

## 📁 File Organization and Naming

### File Naming Convention

**File names MUST match the primary class name they contain:**

```
✅ Good Examples:
- InterruptSourceBase.cs contains class InterruptSourceBase
- TimerInterruptSource.cs contains class TimerInterruptSource  
- Z80Cpu.cs contains class Z80Cpu
- IZ80Interrupt.cs contains interface IZ80Interrupt

❌ Bad Examples:
- InterruptSource.cs containing class InterruptSourceBase
- InterruptSources.cs containing multiple classes
- Utils.cs containing class StringHelper
```

### One Class Per File

**Each file SHOULD contain exactly one primary class, interface, or enum:**

```
✅ Good: Single responsibility per file
- TimerInterruptSource.cs (TimerInterruptSource class only)
- VdpInterruptSource.cs (VdpInterruptSource class only)
- InterruptSourceCategory.cs (InterruptSourceCategory enum only)

❌ Avoid: Multiple classes in one file
- InterruptSources.cs containing:
  - TimerInterruptSource
  - VdpInterruptSource  
  - IoDeviceInterruptSource
  - NmiInterruptSource
```

**Exceptions to one-class-per-file:**
- Small, tightly related helper types (e.g., event args with their delegate)
- Private nested classes that are only used by the containing class
- Extension methods grouped in static classes

### Directory Organization

**Group related functionality in logical subdirectories:**

```
src/Core/
├── Z80Cpu.cs
├── Z80MemoryMap.cs
├── Z80CpuOptions.cs
├── Z80CycleTiming.cs
├── Z80OpCode.cs
└── Interrupt/                    # Interrupt-related classes
    ├── IZ80Interrupt.cs         # Interface
    ├── Z80Interrupt.cs          # Main implementation
    ├── AsyncInterruptEmulator.cs # Emulation logic
    ├── InterruptSourceBase.cs   # Base class
    ├── NullInterruptSource.cs   # Null Object pattern implementation
    ├── SystemInterruptSource.cs # System-level interrupts
    ├── TimerInterruptSource.cs  # Specific implementations
    ├── VdpInterruptSource.cs
    ├── IoDeviceInterruptSource.cs
    └── NmiInterruptSource.cs
```

**Benefits of proper organization:**
- **Discoverability**: Easy to find related functionality
- **Maintainability**: Changes are localized to relevant directories
- **Testing**: Test files can mirror the same structure
- **Code reviews**: Reviewers can focus on specific functional areas
- **Refactoring**: Moving/renaming files follows clear patterns

### Test Utilities and Mocks

**Test utilities, mocks, and test-specific classes MUST be placed in the tests directory:**

```
✅ Good: Test utilities in tests directory
tests/Core.Tests/
├── Z80CpuTests.cs
├── Z80InterruptTests.cs
├── MockZ80Interrupt.cs        # Mock implementations for testing
├── TestInterruptSource.cs     # Test-specific interrupt sources
└── Assets/
    └── simple.bin

❌ Bad: Test utilities in source directory
src/Core/Interrupt/
├── Z80Interrupt.cs
├── MockZ80Interrupt.cs        # Should be in tests!
└── TestInterruptSource.cs     # Should be in tests!
```

**Test utility naming:**
- Use `Mock` prefix for mock implementations: `MockZ80Interrupt`
- Use `Test` prefix for test-specific utilities: `TestInterruptSource`
- Use `Fake` prefix for fake implementations: `FakeMemoryMap`

### Namespace Guidelines

**Namespaces MUST follow the directory structure (IDE0130 rule):**

```csharp
// ✅ Good: Namespace matches directory structure exactly
namespace Zenix.Core;                    // src/Core/Z80Cpu.cs
namespace Zenix.Core.Interrupt;         // src/Core/Interrupt/Z80Interrupt.cs
namespace Zenix.App.Configuration;      // src/App/Configuration/EmulatorConfiguration.cs
namespace Zenix.Tests.Core;             // tests/Core.Tests/TestInterruptSource.cs

// ❌ Bad: Namespace doesn't match location
namespace Zenix.Core;                   // In src/Core/Interrupt/Z80Interrupt.cs (should be Zenix.Core.Interrupt)
namespace Zenix.Utilities;             // In src/Core/Interrupt/Helper.cs
namespace Zenix.Core.Utils.Timing;     // In src/Core/Z80CycleTiming.cs
```

**Using Directives:**
When referencing types from other namespaces, add explicit using statements:

```csharp
// ✅ Good: Explicit using statements
using Zenix.Core;
using Zenix.Core.Interrupt;

namespace Zenix.App;

public class EmulatorFactory
{
    public IZ80Interrupt CreateInterrupt() => new Z80Interrupt();
    public InterruptSourceBase CreateSource() => NullInterruptSource.Instance;
}

// ❌ Avoid: Fully qualified names everywhere (reduces readability)
namespace Zenix.App;

public class EmulatorFactory  
{
    public Zenix.Core.Interrupt.IZ80Interrupt CreateInterrupt() => new Zenix.Core.Interrupt.Z80Interrupt();
}
```

---

## 🏗️ Architecture Guidelines

### Dependency Injection

**ALWAYS use constructor injection for dependencies:**

```csharp
// ✅ Good: Constructor injection
public class Z80Cpu
{
    private readonly Z80MemoryMap _memory;
    private readonly IZ80Interrupt _interrupt;
    
    public Z80Cpu(Z80MemoryMap memory, IZ80Interrupt interrupt, Z80CpuOptions options)
    {
        _memory = memory;
        _interrupt = interrupt;
        Options = options;
    }
}

// ❌ Avoid: Service locator or static dependencies
public class Z80Cpu
{
    private readonly Z80MemoryMap _memory = ServiceLocator.Get<Z80MemoryMap>();
}
```

### Interface Segregation

Keep interfaces focused and cohesive:

```csharp
// ✅ Good: Focused interfaces
public interface IMemoryReader
{
    byte ReadByte(ushort address);
    ushort ReadWord(ushort address);
}

public interface IMemoryWriter
{
    void WriteByte(ushort address, byte value);
    void WriteWord(ushort address, ushort value);
}

// ❌ Avoid: Large, unfocused interfaces
public interface IMemoryEverything
{
    byte ReadByte(ushort address);
    void WriteByte(ushort address, byte value);
    void LoadRom(byte[] data);
    void SaveState(Stream stream);
    void Configure(MemoryConfiguration config);
    // ... 20 more methods
}
```

---

## 📝 Naming Conventions

### General Rules

- Use **PascalCase** for public members, types, and namespaces
- Use **camelCase** for private fields, parameters, and local variables
- Use **UPPER_CASE** for constants
- Prefix private fields with underscore: `_fieldName`

### Domain-Specific Naming

```csharp
// ✅ Good: Clear, domain-specific names
public record VdpInterruptSource() : InterruptSourceBase("VDP");
public record NmiInterruptSource() : InterruptSourceBase("NMI");

public class Z80Cpu
{
    private readonly Z80MemoryMap _memory;
    private ulong _totalCycles;
    
    public void ExecuteInstruction() { }
    public bool IsHalted => Halted;
}

// ❌ Avoid: Generic or unclear names
public record Source1() : SourceBase("S1");
public class Processor
{
    private readonly Map _m;
    private ulong _c;
    
    public void Do() { }
    public bool Flag => _f;
}
```

### Z80-Specific Conventions

Follow Z80 assembly language conventions for register and instruction names:

```csharp
// Registers: Uppercase as in Z80 documentation
public byte A { get; private set; }
public ushort BC => (ushort)((B << 8) | C);

// Instructions: Match Z80 mnemonics
private void ExecuteLD_A_n() { }
private void ExecuteJP_nn() { }
private void ExecuteADD_A_r() { }

// Flags: Use Z80 flag names
private bool GetZeroFlag() => (F & FLAG_ZERO) != 0;
private bool GetCarryFlag() => (F & FLAG_CARRY) != 0;
```

---

## 🧪 Testing Guidelines

### Test Structure

Use the **Arrange-Act-Assert** pattern:

```csharp
[Fact]
public void ExecuteLD_A_n_ShouldLoadImmediateToAccumulator()
{
    // Arrange
    var memoryMap = new Z80MemoryMap();
    var options = new Z80CpuOptions(ClockMHz: 4.0);
    var cpu = new Z80Cpu(memoryMap, options);
    
    memoryMap.WriteByte(0x0000, Z80OpCode.LD_A_n);
    memoryMap.WriteByte(0x0001, 0x42);
    
    // Act
    cpu.Step();
    
    // Assert
    Assert.Equal(0x42, cpu.A);
    Assert.Equal(0x0002, cpu.PC);
    Assert.Equal(7UL, cpu.TotalCycles);
}
```

### Mock Usage

Prefer concrete test objects over complex mocks when possible:

```csharp
// ✅ Good: Simple test double
public class TestInterruptSource : InterruptSourceBase
{
    public TestInterruptSource(string name = "Test") : base(name) { }
}

// ✅ Good: Use Moq for complex behavior
var mockMemory = new Mock<IMemoryMap>();
mockMemory.Setup(m => m.ReadByte(0x1000)).Returns(0x42);
```

### Test Data with Records

```csharp
// ✅ Good: Immutable test data
public record TestScenario(
    string Name,
    byte[] Program,
    Z80CpuOptions Options,
    byte ExpectedA,
    ushort ExpectedPC);

public static readonly TestScenario[] LoadInstructionTests = 
{
    new("LD A,42", [Z80OpCode.LD_A_n, 0x42], new(), 0x42, 0x0002),
    new("LD B,100", [Z80OpCode.LD_B_n, 0x64], new(), 0x00, 0x0002)
};
```

---

## ⚡ Performance Guidelines

### Hot Path Optimization

```csharp
// ✅ Good: Minimize allocations in instruction execution
public void Step()
{
    byte opcode = _memory.ReadByte(PC++);
    int cycles = ExecuteInstruction(opcode); // Returns value type
    _totalCycles += (ulong)cycles;
}

// ❌ Avoid: Allocations in hot paths
public void Step()
{
    var instruction = new Instruction(_memory.ReadByte(PC++)); // Allocation!
    var result = instruction.Execute(); // Allocation!
    _totalCycles += result.Cycles;
}
```

### Memory-Efficient Data Structures

```csharp
// ✅ Good: Use appropriate collection types
private readonly Dictionary<byte, int> _instructionCycles = new(256);

// ✅ Good: Prefer arrays for fixed-size data
private readonly byte[] _memory = new byte[0x10000];

// ❌ Avoid: Oversized collections
private readonly List<byte> _memory = new(0x10000);
```

---

## 📚 Documentation Standards

### XML Documentation

Document all public APIs:

```csharp
/// <summary>
/// Executes a single Z80 instruction and updates CPU state.
/// </summary>
/// <remarks>
/// This method reads the next instruction from memory at PC, executes it,
/// and updates the program counter and cycle count accordingly.
/// </remarks>
/// <exception cref="InvalidOperationException">
/// Thrown when CPU is in halted state and no interrupt is pending.
/// </exception>
public void Step()
{
    // Implementation
}
```

### Code Comments

Focus on **why**, not **what**:

```csharp
// ✅ Good: Explains the reasoning
// Use 16-bit addition to detect carry from bit 15
private void AddToHL(ushort value)
{
    int result = H16 + value;
    SetCarryFlag(result > 0xFFFF); // Carry occurred
    H16 = (ushort)(result & 0xFFFF);
}

// ❌ Avoid: States the obvious
// Add value to HL register
private void AddToHL(ushort value)
{
    // Add the value
    int result = H16 + value;
    // Set carry flag
    SetCarryFlag(result > 0xFFFF);
    // Store result
    H16 = (ushort)(result & 0xFFFF);
}
```

---

## 🔧 Configuration Management

### Immutable Configuration

```csharp
// ✅ Good: Immutable configuration with validation
public record EmulatorConfiguration(
    CpuConfiguration Cpu = null,
    VideoConfiguration Video = null,
    AudioConfiguration Audio = null)
{
    public CpuConfiguration Cpu { get; init; } = Cpu ?? new();
    public VideoConfiguration Video { get; init; } = Video ?? new();
    public AudioConfiguration Audio { get; init; } = Audio ?? new();
}

// ✅ Good: Conversion methods for legacy compatibility
public static class ConfigurationExtensions
{
    public static Z80CpuOptions ToZ80CpuOptions(this CpuConfiguration config) =>
        new(config.ClockMHz, config.RomSize, config.RamSize);
}
```

### Environment-Specific Configurations

```csharp
// ✅ Good: Environment-aware configuration
public static class ConfigurationProfiles
{
    public static EmulatorConfiguration Development => new(
        Cpu: new(ClockMHz: 4.0, EnableTelemetry: true),
        Video: new(EnableDebugOverlay: true));
        
    public static EmulatorConfiguration Production => new(
        Cpu: new(ClockMHz: 3.58, EnableTelemetry: false),
        Video: new(EnableDebugOverlay: false));
}
```

---

## 📋 Change Log

| Version | Date | Changes |
|---------|------|---------|
| 1.1 | 2025-01-15 | Updated namespace structure section with current implementation |
| | | Added complete IDE0130 compliance information |
| | | Documented refactoring completion and verification |
| 1.0 | 2025-01-15 | Initial coding guidelines |
| | | Immutable data structures with records |
| | | Functional programming principles |
| | | Testing and performance guidelines |
| | | Architecture patterns and DI guidelines |

---

*This document is part of the Zenix Emulator project. For implementation examples, see the source code and test suite.*

---

## 📂 Namespace Structure

**Current Project Namespace Structure:**
```
Zenix.Core                     # Core CPU and memory components
├── Z80Cpu.cs                  # Main CPU implementation
├── Z80MemoryMap.cs           # Memory management
├── Z80CpuOptions.cs          # CPU configuration
├── Z80CycleTiming.cs         # Timing calculations
└── Z80OpCode.cs              # Opcode definitions

Zenix.Core.Interrupt          # All interrupt-related components
├── InterruptSourceBase.cs    # Base class for interrupt sources
├── NullInterruptSource.cs    # Null Object pattern implementation
├── SystemInterruptSource.cs  # System-level interrupts
├── TimerInterruptSource.cs   # Timer-based interrupts
├── VdpInterruptSource.cs     # Video display processor interrupts
├── IoDeviceInterruptSource.cs # I/O device interrupts
├── NmiInterruptSource.cs     # Non-maskable interrupts
├── IZ80Interrupt.cs          # Interrupt controller interface
├── Z80Interrupt.cs           # Main interrupt controller
└── AsyncInterruptEmulator.cs # Async interrupt emulation

Zenix.App                     # Application layer
├── EmulatorFactory.cs        # Factory for creating components
├── EmulatorCompositionRoot.cs # Dependency injection setup
├── EmulatorHost.cs           # Host for emulator operations
└── IEmulatorFactory.cs       # Factory interface

Zenix.App.Configuration       # Configuration classes
└── EmulatorConfiguration.cs  # Main configuration records

Zenix.CLI                     # Command-line interface
└── Program.cs                # Main entry point

Zenix.Infrastructure          # Infrastructure services
└── ConsoleRenderer.cs        # Console output utilities

Zenix.Web                     # Web-related components (placeholder)
└── WebPlaceholder.cs         # Future web interface

Zenix.Demos                   # Demonstration programs
├── CycleCountingDemo.cs      # CPU cycle timing demonstrations
├── InterruptEmulationDemo.cs # Interrupt system demonstrations
└── ConfigurationDemo.cs     # Configuration system demonstrations

Zenix.Tests.Core.Tests        # Unit tests
├── Z80CpuTests.cs           # CPU unit tests
├── Z80InterruptTests.cs     # Interrupt system unit tests
└── TestInterruptSource.cs   # Test utility classes
```

**IDE0130 Compliance Benefits:**
- **Improved Navigation**: IDE can accurately navigate between files and namespaces
- **Better IntelliSense**: Autocomplete works more reliably
- **Consistent Organization**: Clear logical separation of concerns
- **Easier Refactoring**: Moving files automatically suggests correct namespace updates
- **Team Productivity**: Developers can quickly locate functionality
