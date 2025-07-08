# ✅ Verification and Validation – Zenix Emulator

This document outlines how each level of Zenix requirements is verified and validated across testing phases.

---

## 🧪 User-Level Verification

| ULR | Method |
|-----|--------|
| ULR-01 | Manual image loading tests (CLI & browser), expected boot to BASIC or game |
| ULR-02 | ZEXALL + known ROMs + real-time input/output testing |
| ULR-03 | Configuration coverage test matrix |
| ULR-04 | Debug tools activated, verify memory, registers, VDP state output |
| ULR-05 | OpenTelemetry pipeline test using OTLP collector |
| ULR-06 | Build/run tests for desktop and browser targets |
| ULR-07 | Enable FPGA in config; disconnect to verify fallback |

---

## ⚙️ System-Level Verification

| SLR | Method |
|-----|--------|
| SLR-01 | Z80 opcode test coverage and instruction timing log |
| SLR-02 | SCREEN mode tests with known VRAM writes and pattern match |
| SLR-03 | Audio output waveform capture and tone test comparison |
| SLR-04 | Load `.DSK`, mount FAT12 partition, run MSX-DOS |
| SLR-05 | Gamepad → MSX joystick port map verification |
| SLR-06 | Span log export to Jaeger/Zipkin validated via span count and labels |
| SLR-07 | 60Hz output frame telemetry with tolerance threshold |
| SLR-08 | CLI and config file parsing and emulator init consistency |
| SLR-09 | FPGA detection handler + simulated failover logging |

---

## 🔧 Component-Level Verification

| CLR | Method |
|-----|--------|
| CLR-01 | Unit test for each Z80 opcode, flags, and illegal instruction trap |
| CLR-02 | Slot test: bank switch + mirrored RAM area validation |
| CLR-03 | VDP test: write pattern to VRAM, capture rendered framebuffer |
| CLR-04 | PSG tone output buffer diff with golden reference waveform |
| CLR-05 | Emulate WD2793 commands: seek, read sector, format |
| CLR-06 | Frame timing profiler → average time per frame < 16.67ms |
| CLR-07 | WebAssembly test suite: rendering, canvas, gamepad |
| CLR-08 | Check audio buffer timing under load |
| CLR-09 | Press key/gamepad → MSX register state reflects input |
| CLR-10 | Disconnect FPGA, verify emulation continues via software backend |
