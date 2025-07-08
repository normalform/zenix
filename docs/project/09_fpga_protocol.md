# 🧠 FPGA Protocol – Zenix Emulator

This document defines the communication protocol and responsibilities between the Zenix host system and an optional external FPGA/microcontroller hardware engine.

---

## 🎯 Purpose

To allow optional offloading of emulation tasks such as CPU, VDP, PSG, or audio rendering to a connected FPGA system over USB-C (with ESP32 RISC-V bridge).

---

## ⚙️ High-Level Design

- The host (Zenix) detects and initializes the FPGA system at startup.
- Communication is via a USB-C serial or bulk endpoint, handled by an ESP32 (RISC-V).
- All offload operations are optional. If no FPGA is detected, the emulator defaults to software mode.

---

## 🔌 Connection Layers

| Layer | Role |
|-------|------|
| Host (Zenix) | Sends packets: CPU steps, memory access, configuration |
| ESP32 (RISC-V) | USB-C bridge + Bluetooth HID + firmware loader |
| FPGA | Hardware implementation of Z80, VDP, PSG, HDMI, and Audio DAC |

---

## 📦 Packet Protocol (TLV)

Each command is a binary message with **Type-Length-Value** format:

```text
+--------+--------+------------------+
| TypeID | Length | Payload (bytes)  |
+--------+--------+------------------+
```

| Type ID | Name               | Description |
|---------|--------------------|-------------|
| 0x01    | INIT_CONFIG         | Send model, RAM size, features |
| 0x02    | ROM_UPLOAD          | Transfer ROM image to FPGA memory |
| 0x03    | CPU_STEP            | Run N cycles (Z80) |
| 0x04    | MEMORY_READ         | Read memory address |
| 0x05    | MEMORY_WRITE        | Write memory to address |
| 0x06    | VDP_FRAME_READY     | Notify host of VDP frame completion |
| 0x07    | INPUT_EVENT         | Send gamepad/keyboard input |
| 0x08    | AUDIO_SAMPLE        | (Optional) send audio stream buffer |
| 0x09    | STATUS_REQUEST      | Request FPGA status (connected, idle, active) |
| 0xFF    | RESET               | Soft reset the FPGA system |

---

## 🧠 Command Examples

### 🎮 ROM Upload

- Type: `0x02`
- Length: 0x4000 (for 16KB ROM block)
- Payload: ROM binary chunk

### ⏱ CPU Step

- Type: `0x03`
- Length: 2
- Payload: 16-bit number of Z80 clock cycles to execute

---

## 🔄 Response Packets

The FPGA may send status or trace packets back:

| Type ID | Response Name       | Description |
|---------|---------------------|-------------|
| 0x81    | MEMORY_READ_RESULT  | Return value of memory address |
| 0x82    | CPU_TRACE           | (Optional) execution info for debug |
| 0x83    | INPUT_STATE_ACK     | Confirm input event accepted |
| 0x84    | STATUS_REPLY        | FPGA/ESP32 status report |
| 0x85    | ERROR               | Protocol error (e.g., unknown command) |

---

## 🔐 Security & Synchronization

- All commands are processed **synchronously** with respect to the emulated Z80 clock.
- Packets are ordered and acknowledged (ACK) where needed.
- FPGA should handle timeouts and return errors to keep emulator stable.

---

## 📡 Bluetooth Support

When available, the ESP32 handles:
- Bluetooth **HID** devices: gamepad input routed to MSX joystick ports
- Bluetooth **A2DP** audio: audio DAC output redirected to headset

Host disables local input/audio while Bluetooth is active and confirmed.

---

## 🖥 HDMI and Audio Output

- HDMI output is owned by the FPGA VDP core
- Audio is handled by DAC or Bluetooth; host disables local rendering

---

## 🔄 Fallback Behavior

If no FPGA is detected or any protocol failure occurs:
- Zenix immediately **falls back** to software emulation
- Logs the disconnect event for debugging/telemetry
- Continues operation without user interruption

