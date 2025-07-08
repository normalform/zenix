# 🧩 Design Description – Zenix Emulator

This document describes the internal design of the Zenix emulator, detailing how each major subsystem is structured and interacts with others.

---

## 🧠 Overview

Zenix is built with a strong separation of concerns using Onion Architecture. Core emulation logic lives in the **Domain layer**, isolated from platform-specific concerns like rendering or telemetry. Configuration, orchestration, and I/O are handled through adapters and services in outer layers.

---

## 🧱 Component Breakdown

### 🧮 Z80 CPU

- **Class:** `Z80Cpu`
- **Responsibilities:**
  - Executes Z80 instructions cycle-accurately
  - Handles interrupt modes (IM 0, 1, 2)
  - Emits trace spans for telemetry if enabled
- **Dependencies:** Memory interface, interrupt controller

---

### 🧠 Memory and Slot Mapper

- **Class:** `MsxMemoryMap`, `SlotManager`
- **Responsibilities:**
  - Emulates 64KB segmented memory with slot mapping
  - Supports cartridge slots, RAM, VRAM, and I/O ports

---

### 🖼 VDP (Video Display Processor)

- **Class:** `Vdp9918A`, `VdpV9938`, `VdpV9958`
- **Responsibilities:**
  - Emulates video output across SCREEN 0–12
  - Renders frames to framebuffer
  - Supports VRAM access and register behavior
- **Output:** Host canvas (WASM) or SDL2 display

---

### 🔊 PSG (Sound Generator)

- **Class:** `Psg8910`
- **Responsibilities:**
  - Emulates AY-3-8910 tone and noise channels
  - Generates PCM audio samples for playback
  - Optional integration with audio buffer pool or DAC

---

### 💾 Disk System

- **Class:** `DiskController`, `DiskImage`
- **Responsibilities:**
  - Supports MSX-DOS compatible `.DSK` images
  - WD2793 emulation for read/write/seek sectors

---

### 🎮 Input Handling

- **Class:** `InputManager`, `GamepadAdapter`
- **Responsibilities:**
  - Maps keyboard and/or gamepad input to MSX joystick and key matrix
  - Cross-platform with WebAssembly and native input support

---

### 📡 Telemetry and Debugging

- **Class:** `TelemetryExporter`, `SpanBuffer`
- **Responsibilities:**
  - Emits OpenTelemetry spans (CPU, VDP, Memory)
  - Optional visualization or export to Jaeger, Zipkin

---

### 🧩 Configuration and Bootstrapping

- **Class:** `EmulatorHost`, `MsxConfiguration`
- **Responsibilities:**
  - Reads model config (MSX1, MSX2, etc.)
  - Instantiates appropriate VDP, PSG, and memory layout
  - Orchestrates the emulation loop and rendering

---

## 🧪 Testability

- Each component has:
  - A pure interface
  - Dependency injection support
  - Test harness (`tests/` folder with xUnit)

---

## 🔌 Optional FPGA/ESP32 Integration

- Enabled via configuration
- Uses bridge abstraction (`IFpgaBridge`)
- Routes memory and CPU cycles externally if detected
- Falls back to default software modules otherwise

---

## 📁 File Structure Summary

```text
src/
├── Core/              # Domain: CPU, VDP, PSG, Memory
├── App/               # Application layer: emulation loop, orchestrators
├── Infrastructure/    # SDL, Blazor, Telemetry, I/O, FPGA
├── CLI/               # CLI frontend
└── Web/               # Blazor WebAssembly frontend
tests/                 # xUnit test projects
```

