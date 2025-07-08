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

```bash
dotnet run --project src/Zenix --rom path/to/game.rom --model MSX2+
```

### 🌐 Browser (WASM)

1. Open the WebAssembly build in your browser
2. Drag and drop a `.ROM` file into the UI
3. Use keyboard or gamepad to play!

---

## 📚 Project Documentation

All core documents are located in the [`/docs/project`](docs/project) folder:

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
