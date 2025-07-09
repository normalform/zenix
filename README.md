<p align="left">
  <img src="docs/icons/github_readme.png" alt="Zenix Project Icon" />
</p>

# Zenix: MSX2+ Emulator

**Zenix** is a modern, high-fidelity emulator for the MSX1, MSX2, and MSX2+ home computer systems.  
It is built in **C# with .NET 8**, featuring clean architecture, full observability, WebAssembly support, and a modular design inspired by Domain-Driven Design and Test-Driven Development.

> 💡 This project was collaboratively designed and iterated with **ChatGPT**.

---

## 🎮 Features

- 🎯 Cycle-accurate **Z80** CPU emulation
- 🖼️ Full **VDP** emulation (TMS9918A / V9938 / V9958)
- 🔊 AY-3-8910 **PSG** sound generator
- 💾 **WD2793** floppy controller and MSX-DOS disk support
- 🎮 Gamepad input (USB and browser-compatible)
- 🌐 WebAssembly frontend (Blazor WASM)
- 🧠 OpenTelemetry instrumentation (CPU, memory, video tracing)
- 🧪 Designed for testability, extensibility, and learning
- ⚙️ Optional hardware offload via USB-C (FPGA + ESP32) — modular and non-required

---

## 🚀 Getting Started

### 🖥️ Desktop (CLI)

The project currently includes several demonstration programs to showcase different aspects of the emulator:

```bash
# Run cycle counting demonstration (CPU timing accuracy)
dotnet run --project src -- cycles

# Run interrupt emulation demonstration (interrupt system)
dotnet run --project src -- interrupts

# Run configuration demonstration (dependency injection & config)
dotnet run --project src -- config

# Run all tests
dotnet test tests/Zenix.Tests.csproj
```

### 🌐 Browser (WASM)
*WebAssembly frontend is planned for future development*

### 🧪 Current Implementation Status

✅ **Completed:**
- **Z80 CPU Core**: Full instruction set with cycle-accurate timing
- **Interrupt System**: Complete Z80 interrupt implementation with multiple source types
- **Memory Management**: ROM/RAM management with configurable sizes  
- **Configuration System**: Immutable records with dependency injection
- **Clean Architecture**: Proper namespace structure following IDE0130 compliance
- **Test Suite**: Comprehensive coverage with 90 passing tests
- **Demonstrations**: Working examples of CPU timing, interrupts, and configuration
- **Documentation**: Complete coding guidelines and project documentation

🚧 **In Development:**
- VDP (Video Display Processor) emulation
- PSG (Programmable Sound Generator) 
- Floppy disk controller (WD2793)
- WebAssembly frontend (Blazor WASM)

---

## 📚 Project Documentation

### High-Level Documentation
All core project documents are located in the [`/docs/project`](docs/project) folder:

1. [📘 Introduction](docs/project/01_introduction.md)
2. [📋 Use Case Analysis](docs/project/02_usecase_analysis.md)
3. [⚠️ Risk Analysis with Mitigation](docs/project/03_risk_analysis.md)
4. [📌 Requirements](docs/project/04_requirements.md)
5. [🧱 Architecture](docs/project/05_architecture.md)
6. [🧩 Design Description](docs/project/06_design_description.md)
7. [✅ Verification and Validation](docs/project/07_verification_validation.md)
8. [🔗 Traceability Matrix](docs/project/08_traceability_matrix.md)
9. [🧠 FPGA Protocol](docs/project/09_fpga_protocol.md)
10. [📖 Glossary](docs/project/10_glossary.md)

### Detailed Design Documentation
Comprehensive component design specifications are in [`/docs/design`](docs/design):

- [🏗️ Design Index](docs/design/README.md) - Navigation and overview
- [🧮 Z80 CPU Core](docs/design/Core/Z80Cpu.md) - Complete CPU implementation details
- Additional component designs (planned)

---

## 📁 Project Structure

```
Zenix/
├── src/                           # Main source code
│   ├── CLI/                      # Command-line interface (Zenix.CLI)
│   ├── Core/                     # CPU and memory components (Zenix.Core)
│   │   └── Interrupt/           # Interrupt system (Zenix.Core.Interrupt)
│   ├── App/                     # Application layer (Zenix.App)
│   │   └── Configuration/       # Configuration system (Zenix.App.Configuration)
│   ├── Infrastructure/          # Infrastructure services (Zenix.Infrastructure)
│   └── Web/                     # Web components (Zenix.Web)
├── tests/                       # Unit tests
│   └── Core.Tests/             # Core component tests (Zenix.Tests.Core.Tests)
├── Demos/                       # Demonstration programs (Zenix.Demos)
├── docs/                        # Documentation
│   ├── project/                # High-level project documentation
│   ├── design/                 # Detailed design specifications
│   └── coding-guidelines.md    # Development standards and conventions
└── README.md                    # This file
```

### Key Components

- **🧮 Z80 CPU**: Cycle-accurate instruction execution with comprehensive timing verification
- **⚡ Interrupt System**: Complete Z80 interrupt implementation (IM 0/1/2, NMI, multiple sources)
- **🧠 Memory Management**: Configurable ROM/RAM with proper slot management
- **⚙️ Configuration**: Immutable record-based configuration with DI container support
- **🧪 Test Suite**: 90 comprehensive unit tests covering all components
- **🎯 Demonstrations**: Working examples showcasing CPU timing, interrupts, and configuration
- **📁 Clean Architecture**: IDE0130-compliant namespace structure with proper separation of concerns

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🤝 Contributing

Zenix is open to contributors — whether you're into emulation, .NET, HDL, Blazor, or observability, your help is welcome!

Please read `CONTRIBUTING.md` (coming soon) for guidelines.

---

## 🙏 Credits

- Designed and developed with the help of **ChatGPT**
- Inspired by the MSX, Z80, and retro computing communities
- Open-source emulator references: openMSX, BlueMSX, Fuse, MiSTer

---

## 🌟 Support the Project

If you enjoy Zenix, consider ⭐ starring the repo, opening issues, or sharing feedback!
