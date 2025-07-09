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
  - Executes Z80 instructions cycle-accurately with precise timing constants
  - Maintains a 64-bit cycle counter capable of tracking 10+ years of continuous operation at 4MHz
  - Handles interrupt modes (IM 0, 1, 2)
  - Emits trace spans for telemetry if enabled
  - Provides emulated time calculation and effective frequency monitoring
- **Dependencies:** Memory interface, interrupt controller
- **Cycle Accuracy:** Each instruction consumes the exact number of cycles as specified in the Z80 technical documentation, enabling precise timing for MSX hardware compatibility
- **Detailed Design:** See [Z80 CPU Core Design](../design/Core/Z80Cpu.md) for comprehensive implementation details

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

## ⏱️ Timing and Performance

### Cycle-Accurate Emulation

The Zenix emulator implements cycle-accurate Z80 CPU emulation with the following characteristics:

- **Precision:** Each instruction consumes exactly the same number of cycles as the real Z80 hardware
- **Long-term Accuracy:** Uses a 64-bit cycle counter that can accurately track over 146,000 years of continuous operation at 4MHz
- **Performance Monitoring:** Provides real-time effective frequency calculation and emulated time tracking
- **MSX Compatibility:** Precise timing ensures accurate emulation of MSX hardware timing dependencies

### Timing Constants

All instruction timing is defined in `Z80CycleTiming.cs` with constants based on official Z80 documentation:

- **Basic Operations:** NOP (4 cycles), HALT (4 cycles)
- **Load Instructions:** Immediate loads (7 cycles), register-to-register (4 cycles)
- **Arithmetic:** Register operations (4 cycles), immediate operations (7 cycles)
- **Jumps:** Conditional jumps (7-12 cycles depending on condition)
- **Stack Operations:** PUSH (11 cycles), POP (10 cycles)

### 10-Year Operation Capability

At 4MHz operation:
- **1 year:** ~126 trillion cycles
- **10 years:** ~1.26 quadrillion cycles
- **Safety margin:** 99.99% of uint64 capacity remaining

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
Demos/                 # Demonstration programs and examples
docs/
├── project/           # High-level project documentation
└── design/            # Detailed component design specifications
    └── Core/          # Core emulation component designs
        └── Z80Cpu.md  # Z80 CPU detailed design document
```

