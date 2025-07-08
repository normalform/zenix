# 📋 Use Case Analysis – Zenix Emulator

This document outlines the core user-driven use cases for Zenix, a modern MSulator for MSX1/2/2+ systems. These use cases cover gameplay, configuration, development, testing, and optional hardware integration — all aligned with the project’s architecture and observability goals.

---

## 🧑‍💻 Actors

| Actor | Description |
|-------|-------------|
| **Player** | User who plays MSX games using the emulator |
| **Developer** | Contributor or maintainer building/extending Zenix |
| **Tester** | User verifying emulator accuracy or regression behavior |
| **Educator** | Uses emulator + trace/visualization features to teach MSX concepts |
| **Hardware Integrator** | (Optional) Developer exploring hardware acceleration via FPGA or ESP32 |

---

## ✅ Use Cases

### 🎮 UC-01: Play MSX Game
- **Actor:** Player  
- **Goal:** Load and play a ROM or disk image  
- **Preconditions:** A valid image file is available  
- **Flow:** Launch emulator → Load image → Use input → View output  
- **Postconditions:** Game runs successfully with real-time interaction  
- **Variants:** CLI or Blazor WASM frontend, software or optional hardware backend

---

### 💾 UC-02: Load and Configure Emulator
- **Actors:** Player, Developer  
- **Goal:** Select MSX model, RAM size, ROM paths, disk image  
- **Flow:** Pass options via CLI, JSON config, or UI presets  
- **Postconditions:** Emulator starts with accurate machine configuration

---

### 🖼 UC-03: Visualize Internal State
- **Actors:** Tester, Educator  
- **Goal:** Display internal CPU, memory, and VDP state for analysis or learning  
- **Preconditions:** Debug/visual mode enabled  
- **Flow:** Run emulator → Open viewer panel → Inspect memory/trace/VRAM  
- **Postconditions:** Visualization assists learning or debugging

---

### 📡 UC-04: Export Emulator Telemetry
- **Actors:** Developer, Tester  
- **Goal:** Emit telemetry spans for observability (e.g., CPU ticks, frames)  
- **Flow:** Enable telemetry → Run emulator → Capture via console or OTLP collector  
- **Postconditions:** Span data exported to OpenTelemetry-compatible tooling

---

### 🧪 UC-05: Run Compatibility Test ROM
- **Actor:** Tester  
- **Goal:** Validate CPU and hardware behavior with test ROMs (e.g., ZEXALL, MSX-DOS)  
- **Flow:** Load ROM → Observe output → Compare against expected result  
- **Postconditions:** Test passes or failure is logged for debugging

---

### 🧑‍🔧 UC-06: Extend Emulator Components
- **Actor:** Developer  
- **Goal:** Add support for new hardware components (e.g., MSX-MUSIC, turbo Z80, mappers)  
- **Flow:** Implement domain-level module → Plug into configuration and factory layer  
- **Postconditions:** Emulator builds with new feature enabled via config

---

### 🔌 UC-07: Enable Optional Hardware Acceleration
- **Actor:** Hardware Integrator  
- **Goal:** Offload Z80/VDP/audio to external hardware (FPGA/ESP32) via USB-C  
- **Preconditions:** Hardware detected and compatible firmware loaded  
- **Flow:** Host detects optional hardware → Routes operations accordingly  
- **Postconditions:** Host uses hardware for execution if available, software fallback remains default

---

## 🔗 Use Case to Requirement Mapping

| Use Case | Related Functional Areas |
|----------|--------------------------|
| UC-01 | ROM loading, CPU/VDP/audio execution, input/output |
| UC-02 | Config loader, model presets, RAM/VRAM selection |
| UC-03 | Memory tracing, VDP viewer, instruction logging |
| UC-04 | OpenTelemetry instrumentation |
| UC-05 | Instruction set compliance, test ROM validation |
| UC-06 | Plug-in architecture, DDD layering, feature flags |
| UC-07 | Optional hardware bridge, USB detection, fallback logic

