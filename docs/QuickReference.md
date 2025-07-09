# 📚 Documentation Quick Reference

This document provides quick links to all Zenix emulator documentation, organized by purpose and audience.

---

## 🎯 By Audience

### 👨‍💻 Developers

**Getting Started:**
- [Project README](../README.md) - Overview and quick start
- [Contributing Guide](../CONTRIBUTING.md) - Development guidelines
- [Coding Guidelines](coding-guidelines.md) - Code style, patterns, and best practices

**Architecture & Design:**
- [Architecture Overview](project/05_architecture.md) - High-level system design
- [Design Description](project/06_design_description.md) - Component overview
- [Z80 CPU Design](design/Core/Z80Cpu.md) - Detailed CPU implementation
- [Z80 Interrupt System](design/Core/Z80InterruptSystem.md) - Interrupt system design

**Implementation:**
- [Requirements](project/04_requirements.md) - What needs to be built
- [Verification & Validation](project/07_verification_validation.md) - Testing strategy
- [DevOps Documentation](devops.md) - CI/CD and automation

### 🎮 Users

**Usage:**
- [Project README](../README.md) - Getting started guide
- [Use Cases](project/02_usecase_analysis.md) - What the emulator can do

### 📋 Project Management

**Planning:**
- [Requirements](project/04_requirements.md) - Feature specifications
- [Risk Analysis](project/03_risk_analysis.md) - Project risks and mitigation
- [Traceability Matrix](project/08_traceability_matrix.md) - Requirements tracking

## 🏗️ By Component

### 🧮 Z80 CPU
- **Overview:** [Design Description - Z80 CPU](project/06_design_description.md#🧮-z80-cpu)
- **Detailed Design:** [Z80 CPU Core Design](design/Core/Z80Cpu.md)
- **Testing:** [Z80 CPU Tests](../tests/Core.Tests/Z80CpuTests.cs)
- **Demo:** [Cycle Counting Demo](../Demos/CycleCountingDemo.cs)

### ⚡ Z80 Interrupt System
- **Overview:** [Design Description - Interrupt System](project/06_design_description.md#⚡-z80-interrupt-system)
- **Detailed Design:** [Z80 Interrupt System Design](design/Core/Z80InterruptSystem.md)
- **Interrupt Sources:** Strongly-typed interrupt source hierarchy for type safety
- **Testing:** [Z80 Interrupt Tests](../tests/Core.Tests/Z80InterruptTests.cs)
- **Demo:** [Interrupt Emulation Demo](../Demos/InterruptEmulationDemo.cs)

### 🧠 Memory System
- **Overview:** [Design Description - Memory](project/06_design_description.md#🧠-memory-and-slot-mapper)
- **Implementation:** [Z80MemoryMap](../src/Core/Z80MemoryMap.cs)

### 🖼️ Video Display Processor (VDP)
- **Overview:** [Design Description - VDP](project/06_design_description.md#🖼-vdp-video-display-processor)
- **Status:** 📋 Planned

### 🔊 Sound System (PSG)
- **Overview:** [Design Description - PSG](project/06_design_description.md#🔊-psg-sound-generator)
- **Status:** 📋 Planned

### 💾 Disk System
- **Overview:** [Design Description - Disk](project/06_design_description.md#💾-disk-system)
- **Status:** 📋 Planned

## 📝 By Document Type

### 📋 Requirements & Planning
1. [Introduction](project/01_introduction.md) - Project vision and goals
2. [Use Case Analysis](project/02_usecase_analysis.md) - User scenarios
3. [Risk Analysis](project/03_risk_analysis.md) - Project risks and mitigation
4. [Requirements](project/04_requirements.md) - Functional and non-functional requirements

### 🏗️ Architecture & Design
5. [Architecture](project/05_architecture.md) - System architecture overview
6. [Design Description](project/06_design_description.md) - Component design overview
7. [Detailed Design Index](design/README.md) - Navigation for detailed designs
8. [Z80 CPU Design](design/Core/Z80Cpu.md) - Complete CPU implementation details
9. [Z80 Interrupt System](design/Core/Z80InterruptSystem.md) - Interrupt system design

### ✅ Quality & Validation
10. [Verification & Validation](project/07_verification_validation.md) - Testing strategy
11. [Traceability Matrix](project/08_traceability_matrix.md) - Requirements tracking

### 🔧 Implementation & Operations
12. [FPGA Protocol](project/09_fpga_protocol.md) - Hardware integration
13. [DevOps Documentation](devops.md) - CI/CD processes
14. [Glossary](project/10_glossary.md) - Technical terms and definitions

## 🚀 Quick Navigation

### 🆕 New to the Project?
Start here: [README](../README.md) → [Introduction](project/01_introduction.md) → [Architecture](project/05_architecture.md)

### 🛠️ Want to Contribute?
Read: [Contributing](../CONTRIBUTING.md) → [Requirements](project/04_requirements.md) → [Design Description](project/06_design_description.md)

### 🧮 Working on Z80 CPU?
Go to: [Z80 CPU Design](design/Core/Z80Cpu.md) → [Z80 Tests](../tests/Core.Tests/Z80CpuTests.cs) → [Cycle Demo](../Demos/CycleCountingDemo.cs)

### ⚡ Working on Z80 Interrupts?
Go to: [Z80 Interrupt System](design/Core/Z80InterruptSystem.md) → [Interrupt Tests](../tests/Core.Tests/Z80InterruptTests.cs) → [Interrupt Demo](../Demos/InterruptEmulationDemo.cs)

### 🔍 Looking for Specific Information?
Use: [Glossary](project/10_glossary.md) for terms, [Traceability Matrix](project/08_traceability_matrix.md) for requirements

---

## 📈 Documentation Status

| Component | Overview | Detailed Design | Tests | Demo |
|-----------|----------|-----------------|-------|------|
| Z80 CPU | ✅ Complete | ✅ Complete | ✅ Complete | ✅ Complete |
| Z80 Interrupts | ✅ Complete | ✅ Complete | ✅ Complete | ✅ Complete |
| Coding Guidelines | ✅ Complete | ✅ Complete | N/A | N/A |
| Memory | ✅ Complete | 📋 Planned | ✅ Partial | ❌ None |
| VDP | ✅ Complete | 📋 Planned | ❌ None | ❌ None |
| PSG | ✅ Complete | 📋 Planned | ❌ None | ❌ None |
| Disk | ✅ Complete | 📋 Planned | ❌ None | ❌ None |
| Input | ✅ Complete | 📋 Planned | ❌ None | ❌ None |

**Legend:**
- ✅ Complete - Fully documented and implemented
- ✅ Partial - Basic implementation, needs expansion  
- 📋 Planned - Documented but not yet implemented
- ❌ None - Not yet documented or implemented

---

*This quick reference is part of the Zenix Emulator project documentation. Last updated: 2025-01-15*
