# 🔗 Traceability Matrix – Zenix Emulator

This document links user, system, and component-level requirements across implementation and testing.

---

## 🔁 ULR → SLR Mapping

| ULR ID | Description | Supported By (SLRs) |
|--------|-------------|----------------------|
| ULR-01 | Load and run MSX images | SLR-01, SLR-02, SLR-04, SLR-07 |
| ULR-02 | Accurate, real-time emulation | SLR-01, SLR-02, SLR-03 |
| ULR-03 | Configure system model and RAM | SLR-08, SLR-09 |
| ULR-04 | Debugging and inspection tools | SLR-05, SLR-06 |
| ULR-05 | Runtime telemetry output | SLR-06 |
| ULR-06 | Multi-platform support | SLR-07 |
| ULR-07 | Optional HW acceleration | SLR-09 |

---

## 🔁 SLR → CLR Mapping

| SLR ID | Description | Supported By (CLRs) |
|--------|-------------|----------------------|
| SLR-01 | Z80 instruction set emulation | CLR-01 |
| SLR-02 | MSX video chip emulation | CLR-03 |
| SLR-03 | PSG sound | CLR-04 |
| SLR-04 | Load images (ROM/DSK) | CLR-05 |
| SLR-05 | Input handling | CLR-09 |
| SLR-06 | Telemetry export | CLR-08 |
| SLR-07 | 60Hz rendering | CLR-06, CLR-07 |
| SLR-08 | Config parsing | CLR-02 |
| SLR-09 | HW offload integration | CLR-10 |
