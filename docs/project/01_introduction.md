# 📘 Introduction

## Project Name: Zenix – MSX2+ Emulator

**Zenix** is a modern, open-source emulator for the MSX1, MSX2, and MSX2+ computer systems.  
It is designed with an emphasis on modularity, accuracy, and observability, while remaining accessible to users across platforms including desktop and the web.

This project aims to provide both an accurate MSX experience for retro computing enthusiasts and a transparent system for developers, educators, and emulator researchers.

---

## ✨ Key Characteristics

- **Language:** C# (.NET 8)
- **Architecture:** Domain-Driven Design (DDD), Onion Architecture
- **Frontend Support:** CLI, Blazor WebAssembly
- **Testing Strategy:** Test-Driven Development (TDD)
- **Documentation:** Markdown-based with Mermaid diagrams
- **Telemetry:** Integrated OpenTelemetry for performance and trace observability
- **Optional Offload:** Designed to support optional hardware acceleration via FPGA and microcontroller over USB-C
- **Distribution:** Portable, cross-platform, and embeddable

---

## 📌 Goals

- Emulate MSX1/2/2+ systems with **cycle-accurate** Z80 and VDP behavior
- Provide **visual and traceable** debugging tools for developers and educators
- Allow flexible **configuration** of system models, RAM, VRAM, slots, and peripherals
- Support **OpenTelemetry** and modern instrumentation tooling
- Be easily **extendable**, **testable**, and **platform-independent**
- Keep the emulator fully functional in **software-only mode**, with optional hardware integration

---

## 🧠 About This Document

This introduction outlines the vision, goals, and design values of the Zenix project and serves as the entry point for deeper architectural and design documentation.

For a full overview, see the rest of the documentation in the `/docs` folder.

