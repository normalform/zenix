# 📖 Glossary – Zenix| Term | Definition |
|------|------------|
| DSK | Disk image format used to emulate MSX floppy drives |
| DDD | Domain-Driven Design – A software architecture practice to separate domain logic from infrastructure |
| DI | Disable Interrupts – Z80 instruction that disables maskable interrupts |
| DOTNET | Microsoft's cross-platform development platform used for building Zenix |
| EI | Enable Interrupts – Z80 instruction that enables maskable interrupts (with delay) |
| FPGA | Field-Programmable Gate Array – Optional hardware offload target for CPU/VDP emulation |
| Framebuffer | Memory buffer used for building video frames before output |
| FDC | Floppy Disk Controller (e.g., WD2793) that manages disk image I/O |r

This glossary defines technical terms, abbreviations, and domain-specific language used throughout the Zenix MSX emulator project.

---

## A–C

| Term | Definition |
|------|------------|
| A2DP | Advanced Audio Distribution Profile – Bluetooth protocol for streaming stereo audio |
| AY-3-8910 | Programmable Sound Generator (PSG) used in MSX computers for audio output |
| BIOS | Basic Input/Output System – MSX startup ROM containing essential routines |
| Blazor | A web UI framework in .NET used to compile and run the emulator in browsers (via WASM) |
| CLR | Component-Level Requirement – A low-level, testable implementation detail |
| CLI | Command-Line Interface – Used to launch and configure the emulator via shell/terminal |

---

## D–H

| Term | Definition |
|------|------------|
| DSK | Disk image format used to emulate MSX floppy drives |
| DDD | Domain-Driven Design – A software architecture practice to separate domain logic from infrastructure |
| DOTNET | Microsoft’s cross-platform development platform used for building Zenix |
| FPGA | Field-Programmable Gate Array – Optional hardware offload target for CPU/VDP emulation |
| Framebuffer | Memory buffer used for building video frames before output |
| FDC | Floppy Disk Controller (e.g., WD2793) that manages disk image I/O |

---

## I–O

| Term | Definition |
|------|------------|
| I/O | Input/Output – Refers to MSX port access or emulator host interaction |
| IFF1 | Interrupt Flip-Flop 1 – Z80 master interrupt enable flag |
| IFF2 | Interrupt Flip-Flop 2 – Z80 interrupt enable backup flag |
| IFF | Interrupt Flip-Flop – Z80 CPU flags that control interrupt enable/disable state |
| IM | Interrupt Mode – Z80 has three interrupt modes (0, 1, 2) with different behaviors |
| Interrupt Source | Strongly-typed object representing the origin of an interrupt request |
| InterruptSourceBase | Abstract base class for all interrupt source types in the system |
| IoDeviceInterruptSource | Interrupt source type for I/O device interrupts |
| INT | Maskable interrupt signal on Z80 processor |
| JSON | JavaScript Object Notation – Used for structured emulator configuration files |
| MSX | A standard home computer architecture from the 1980s developed by ASCII & Microsoft |
| MSX-DOS | Disk Operating System for MSX machines, similar to CP/M |
| NFR | Non-Functional Requirement – A quality or performance attribute (e.g., portability, performance) |
| NMI | Non-Maskable Interrupt – High-priority Z80 interrupt that cannot be disabled |
| NmiInterruptSource | Singleton interrupt source instance for non-maskable interrupts |

---

## P–S

| Term | Definition |
|------|------------|
| PCM | Pulse-Code Modulation – Digital representation of analog audio |
| PSG | Programmable Sound Generator – Audio chip used in MSX (AY-3-8910) |
| RAM | Random Access Memory – Volatile storage used by the emulated MSX |
| RETI | Return from Interrupt – Z80 instruction that returns from maskable interrupt |
| RETN | Return from NMI – Z80 instruction that returns from non-maskable interrupt |
| ROM | Read-Only Memory – Cartridge or BIOS images loaded into the emulator |
| RST | Restart instruction – Z80 instruction that jumps to fixed memory addresses |
| SLR | System-Level Requirement – Behavior or operation the system as a whole must fulfill |
| SCREEN Modes | MSX video modes (0–12) controlled via VDP registers |

---

## T–Z

| Term | Definition |
|------|------------|
| TDD | Test-Driven Development – A design practice where tests are written before implementation |
| Telemetry | Emitted performance or trace data for monitoring or analysis |
| TLV | Type-Length-Value – A compact binary packet format used in FPGA protocol |
| T-state | Z80 timing unit – Each clock cycle of the Z80 processor |
| ULR | User-Level Requirement – High-level goal or user expectation |
| VDP | Video Display Processor – TMS9918A/V9938/V9958 chips emulated by Zenix |
| VdpInterruptSource | Interrupt source type for Video Display Processor interrupts |
| Vector | Interrupt vector – Memory address where interrupt service routine is located |
| WASM | WebAssembly – Binary instruction format for running Zenix in the browser |
| Z80 | The 8-bit microprocessor used in MSX computers and emulated by Zenix |
| ZEXALL | A diagnostic ROM used to test Z80 compatibility by executing every instruction |

