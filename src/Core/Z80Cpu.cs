// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.Core.Interrupt;

namespace Zenix.Core;

public class Z80Cpu
{
    private readonly Z80MemoryMap _memory;
    private readonly IZ80Interrupt _interrupt;
    public Z80CpuOptions Options { get; }

    // Register storage accessible for debug tools
    public class Z80Registers
    {
        public byte A;
        public byte F;
        public byte B;
        public byte C;
        public byte D;
        public byte E;
        public byte H;
        public byte L;
        public ushort SP;
        public ushort PC;
    }

    /// <summary>
    /// Public instance exposing the current register values. External tools can
    /// inspect or modify the registers directly for debugging purposes.
    /// </summary>
    public Z80Registers Registers { get; } = new();

    /// <summary>
    /// Hook invoked whenever a byte is read from memory. The ushort parameter is
    /// the address and the byte parameter is the value returned.
    /// </summary>
    public Action<ushort, byte>? MemoryReadHook;

    /// <summary>
    /// Hook invoked whenever a byte is written to memory. The ushort parameter
    /// is the address written and the byte parameter is the value stored.
    /// </summary>
    public Action<ushort, byte>? MemoryWriteHook;

    // Cycle-accurate timing state
    private ulong _totalCycles = 0;
    private DateTime _emulationStartTime = DateTime.UtcNow;

    public Z80Cpu(Z80MemoryMap memory, IZ80Interrupt interrupt, Z80CpuOptions? options = null)
    {
        _memory = memory;
        _interrupt = interrupt;
        Options = options ?? new Z80CpuOptions();
        _memory.Configure(Options.RomSize, Options.RamSize);
        Reset();
    }

    public byte A { get => Registers.A; private set => Registers.A = value; }
    public byte F { get => Registers.F; private set => Registers.F = value; }
    public byte B { get => Registers.B; private set => Registers.B = value; }
    public byte C { get => Registers.C; private set => Registers.C = value; }
    public byte D { get => Registers.D; private set => Registers.D = value; }
    public byte E { get => Registers.E; private set => Registers.E = value; }
    public byte H { get => Registers.H; private set => Registers.H = value; }
    public byte L { get => Registers.L; private set => Registers.L = value; }
    public ushort SP { get => Registers.SP; private set => Registers.SP = value; }
    public ushort PC { get => Registers.PC; private set => Registers.PC = value; }
    public bool Halted { get; private set; }

    /// <summary>
    /// Interrupt controller for handling Z80 interrupts
    /// </summary>
    public IZ80Interrupt Interrupt => _interrupt;

    /// <summary>
    /// Total number of CPU cycles executed since reset.
    /// Can accurately track over 10 years of operation at 4MHz.
    /// </summary>
    public ulong TotalCycles => _totalCycles;

    /// <summary>
    /// Current effective clock frequency based on executed cycles and elapsed time.
    /// </summary>
    public double EffectiveClockFrequency 
    {
        get
        {
            var elapsed = DateTime.UtcNow - _emulationStartTime;
            if (elapsed.TotalSeconds < 0.001)
            {
                return 0.0;
            }
            return _totalCycles / elapsed.TotalSeconds;
        }
    }

    /// <summary>
    /// Emulated time elapsed in seconds based on cycle count and configured clock frequency.
    /// </summary>
    public double EmulatedTimeSeconds =>
        _totalCycles / (Options.ClockMHz * 1_000_000.0);

    public void Reset()
    {
        PC = 0;
        SP = (ushort)(Options.RamSize);
        Halted = false;
        _totalCycles = 0;
        _emulationStartTime = DateTime.UtcNow;
        _interrupt.Reset();
    }

    public void Step()
    {
        // Handle interrupt timing
        _interrupt.BeforeInstruction();
        
        // Check for interrupts before instruction execution
        if (_interrupt.ShouldProcessInterrupt(out Z80InterruptRequest interruptRequest))
        {
            HandleInterrupt(interruptRequest);
            return;
        }
        
        if (Halted) 
        {
            // HALT state still consumes cycles
            _totalCycles += Z80CycleTiming.HALT;
            return;
        }

        byte opcode = ReadByte(PC++);
        byte cycles = 0;

        switch (opcode)
        {
            // 8-bit Load instructions
            case Z80OpCode.NOP: // NOP
                cycles = Z80CycleTiming.NOP;
                break;
            case Z80OpCode.LD_A_n: // LD A, n
                A = ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_B_n: // LD B, n
                B = ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_C_n: // LD C, n
                C = ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_D_n: // LD D, n
                D = ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_E_n: // LD E, n
                E = ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_H_n: // LD H, n
                H = ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_L_n: // LD L, n
                L = ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            
            // 8-bit register to register loads
            case Z80OpCode.LD_A_A: cycles = Z80CycleTiming.LD_r_r; break; // LD A, A
            case Z80OpCode.LD_A_B: A = B; cycles = Z80CycleTiming.LD_r_r; break; // LD A, B
            case Z80OpCode.LD_A_C: A = C; cycles = Z80CycleTiming.LD_r_r; break; // LD A, C
            case Z80OpCode.LD_A_D: A = D; cycles = Z80CycleTiming.LD_r_r; break; // LD A, D
            case Z80OpCode.LD_A_E: A = E; cycles = Z80CycleTiming.LD_r_r; break; // LD A, E
            case Z80OpCode.LD_A_H: A = H; cycles = Z80CycleTiming.LD_r_r; break; // LD A, H
            case Z80OpCode.LD_A_L: A = L; cycles = Z80CycleTiming.LD_r_r; break; // LD A, L
            
            case Z80OpCode.LD_B_A: B = A; cycles = Z80CycleTiming.LD_r_r; break; // LD B, A
            case Z80OpCode.LD_B_B: cycles = Z80CycleTiming.LD_r_r; break; // LD B, B
            case Z80OpCode.LD_B_C: B = C; cycles = Z80CycleTiming.LD_r_r; break; // LD B, C
            case Z80OpCode.LD_B_D: B = D; cycles = Z80CycleTiming.LD_r_r; break; // LD B, D
            case Z80OpCode.LD_B_E: B = E; cycles = Z80CycleTiming.LD_r_r; break; // LD B, E
            case Z80OpCode.LD_B_H: B = H; cycles = Z80CycleTiming.LD_r_r; break; // LD B, H
            case Z80OpCode.LD_B_L: B = L; cycles = Z80CycleTiming.LD_r_r; break; // LD B, L
            
            case Z80OpCode.LD_C_A: C = A; cycles = Z80CycleTiming.LD_r_r; break; // LD C, A
            case Z80OpCode.LD_C_B: C = B; cycles = Z80CycleTiming.LD_r_r; break; // LD C, B
            case Z80OpCode.LD_C_C: cycles = Z80CycleTiming.LD_r_r; break; // LD C, C
            case Z80OpCode.LD_C_D: C = D; cycles = Z80CycleTiming.LD_r_r; break; // LD C, D
            case Z80OpCode.LD_C_E: C = E; cycles = Z80CycleTiming.LD_r_r; break; // LD C, E
            case Z80OpCode.LD_C_H: C = H; cycles = Z80CycleTiming.LD_r_r; break; // LD C, H
            case Z80OpCode.LD_C_L: C = L; cycles = Z80CycleTiming.LD_r_r; break; // LD C, L
            
            // 16-bit Load instructions
            case Z80OpCode.LD_BC_nn: // LD BC, nn
                C = ReadByte(PC++);
                B = ReadByte(PC++);
                cycles = Z80CycleTiming.LD_dd_nn;
                break;
            case Z80OpCode.LD_DE_nn: // LD DE, nn
                E = ReadByte(PC++);
                D = ReadByte(PC++);
                cycles = Z80CycleTiming.LD_dd_nn;
                break;
            case Z80OpCode.LD_HL_nn: // LD HL, nn
                L = ReadByte(PC++);
                H = ReadByte(PC++);
                cycles = Z80CycleTiming.LD_dd_nn;
                break;
            case Z80OpCode.LD_SP_nn: // LD SP, nn
                ushort spLow = ReadByte(PC++);
                ushort spHigh = ReadByte(PC++);
                SP = (ushort)(spLow | (spHigh << 8));
                cycles = Z80CycleTiming.LD_dd_nn;
                break;
            
            // Memory operations
            case Z80OpCode.LD_nn_A: // LD (nn), A
                ushort addrLow = ReadByte(PC++);
                ushort addrHigh = ReadByte(PC++);
                ushort addr = (ushort)(addrLow | (addrHigh << 8));
                WriteByte(addr, A);
                cycles = Z80CycleTiming.LD_nn_A;
                break;
            case Z80OpCode.LD_A_nn: // LD A, (nn)
                ushort loadAddrLow = ReadByte(PC++);
                ushort loadAddrHigh = ReadByte(PC++);
                ushort loadAddr = (ushort)(loadAddrLow | (loadAddrHigh << 8));
                A = ReadByte(loadAddr);
                cycles = Z80CycleTiming.LD_A_nn;
                break;
            case Z80OpCode.LD_HL_A: // LD (HL), A
                WriteByte(GetHL(), A);
                cycles = Z80CycleTiming.LD_HL_r;
                break;
            case Z80OpCode.LD_A_HL: // LD A, (HL)
                A = ReadByte(GetHL());
                cycles = Z80CycleTiming.LD_r_HL;
                break;
            
            // Arithmetic operations
            case Z80OpCode.ADD_A_A: // ADD A, A
                AddToA(A);
                cycles = Z80CycleTiming.ADD_A_r;
                break;
            case Z80OpCode.ADD_A_B: // ADD A, B
                AddToA(B);
                cycles = Z80CycleTiming.ADD_A_r;
                break;
            case Z80OpCode.ADD_A_C: // ADD A, C
                AddToA(C);
                cycles = Z80CycleTiming.ADD_A_r;
                break;
            case Z80OpCode.ADD_A_D: // ADD A, D
                AddToA(D);
                cycles = Z80CycleTiming.ADD_A_r;
                break;
            case Z80OpCode.ADD_A_E: // ADD A, E
                AddToA(E);
                cycles = Z80CycleTiming.ADD_A_r;
                break;
            case Z80OpCode.ADD_A_H: // ADD A, H
                AddToA(H);
                cycles = Z80CycleTiming.ADD_A_r;
                break;
            case Z80OpCode.ADD_A_L: // ADD A, L
                AddToA(L);
                cycles = Z80CycleTiming.ADD_A_r;
                break;
            case Z80OpCode.ADD_A_n: // ADD A, n
                AddToA(ReadByte(PC++));
                cycles = Z80CycleTiming.ADD_A_n;
                break;
            
            // Increment/Decrement
            case Z80OpCode.INC_A: // INC A
                A = IncByte(A);
                cycles = Z80CycleTiming.INC_r;
                break;
            case Z80OpCode.INC_B: // INC B
                B = IncByte(B);
                cycles = Z80CycleTiming.INC_r;
                break;
            case Z80OpCode.INC_C: // INC C
                C = IncByte(C);
                cycles = Z80CycleTiming.INC_r;
                break;
            case Z80OpCode.INC_D: // INC D
                D = IncByte(D);
                cycles = Z80CycleTiming.INC_r;
                break;
            case Z80OpCode.INC_E: // INC E
                E = IncByte(E);
                cycles = Z80CycleTiming.INC_r;
                break;
            case Z80OpCode.INC_H: // INC H
                H = IncByte(H);
                cycles = Z80CycleTiming.INC_r;
                break;
            case Z80OpCode.INC_L: // INC L
                L = IncByte(L);
                cycles = Z80CycleTiming.INC_r;
                break;
            
            case Z80OpCode.DEC_A: // DEC A
                A = DecByte(A);
                cycles = Z80CycleTiming.DEC_r;
                break;
            case Z80OpCode.DEC_B: // DEC B
                B = DecByte(B);
                cycles = Z80CycleTiming.DEC_r;
                break;
            case Z80OpCode.DEC_C: // DEC C
                C = DecByte(C);
                cycles = Z80CycleTiming.DEC_r;
                break;
            case Z80OpCode.DEC_D: // DEC D
                D = DecByte(D);
                cycles = Z80CycleTiming.DEC_r;
                break;
            case Z80OpCode.DEC_E: // DEC E
                E = DecByte(E);
                cycles = Z80CycleTiming.DEC_r;
                break;
            case Z80OpCode.DEC_H: // DEC H
                H = DecByte(H);
                cycles = Z80CycleTiming.DEC_r;
                break;
            case Z80OpCode.DEC_L: // DEC L
                L = DecByte(L);
                cycles = Z80CycleTiming.DEC_r;
                break;
            
            // Jump instructions
            case Z80OpCode.JP_nn: // JP nn
                ushort low = ReadByte(PC++);
                ushort high = ReadByte(PC++);
                PC = (ushort)(low | (high << 8));
                cycles = Z80CycleTiming.JP_nn;
                break;
            case Z80OpCode.JR_e: // JR e (relative jump)
                sbyte offset = (sbyte)ReadByte(PC++);
                PC = (ushort)(PC + offset);
                cycles = Z80CycleTiming.JR_e_taken;
                break;
            case Z80OpCode.JR_NZ_e: // JR NZ, e
                sbyte nzOffset = (sbyte)ReadByte(PC++);
                if (!GetZeroFlag())
                {
                    PC = (ushort)(PC + nzOffset);
                    cycles = Z80CycleTiming.JR_cc_e_taken;
                }
                else
                {
                    cycles = Z80CycleTiming.JR_cc_e_not_taken;
                }
                break;
            case Z80OpCode.JR_Z_e: // JR Z, e
                sbyte zOffset = (sbyte)ReadByte(PC++);
                if (GetZeroFlag())
                {
                    PC = (ushort)(PC + zOffset);
                    cycles = Z80CycleTiming.JR_cc_e_taken;
                }
                else
                {
                    cycles = Z80CycleTiming.JR_cc_e_not_taken;
                }
                break;
            
            // Stack operations
            case Z80OpCode.PUSH_BC: // PUSH BC
                PushWord(GetBC());
                cycles = Z80CycleTiming.PUSH_qq;
                break;
            case Z80OpCode.PUSH_DE: // PUSH DE
                PushWord(GetDE());
                cycles = Z80CycleTiming.PUSH_qq;
                break;
            case Z80OpCode.PUSH_HL: // PUSH HL
                PushWord(GetHL());
                cycles = Z80CycleTiming.PUSH_qq;
                break;
            case Z80OpCode.PUSH_AF: // PUSH AF
                PushWord(GetAF());
                cycles = Z80CycleTiming.PUSH_qq;
                break;
            
            case Z80OpCode.POP_BC: // POP BC
                SetBC(PopWord());
                cycles = Z80CycleTiming.POP_qq;
                break;
            case Z80OpCode.POP_DE: // POP DE
                SetDE(PopWord());
                cycles = Z80CycleTiming.POP_qq;
                break;
            case Z80OpCode.POP_HL: // POP HL
                SetHL(PopWord());
                cycles = Z80CycleTiming.POP_qq;
                break;
            case Z80OpCode.POP_AF: // POP AF
                SetAF(PopWord());
                cycles = Z80CycleTiming.POP_qq;
                break;
            
            // Interrupt instructions
            case Z80OpCode.EI: // EI - Enable interrupts
                _interrupt.EnableInterrupts();
                cycles = Z80CycleTiming.EI;
                break;
            case Z80OpCode.DI: // DI - Disable interrupts
                _interrupt.DisableInterrupts();
                cycles = Z80CycleTiming.DI;
                break;
            
            // Extended instructions (ED prefix)
            case Z80OpCode.ED_PREFIX: // ED prefix
                byte extendedOpcode = _memory.ReadByte(PC++);
                switch (extendedOpcode)
                {
                    case Z80OpCode.IM_0: // IM 0
                        _interrupt.SetInterruptMode(Z80InterruptMode.Mode0);
                        cycles = Z80CycleTiming.IM_0;
                        break;
                    case Z80OpCode.IM_1: // IM 1
                        _interrupt.SetInterruptMode(Z80InterruptMode.Mode1);
                        cycles = Z80CycleTiming.IM_1;
                        break;
                    case Z80OpCode.IM_2: // IM 2
                        _interrupt.SetInterruptMode(Z80InterruptMode.Mode2);
                        cycles = Z80CycleTiming.IM_2;
                        break;
                    case Z80OpCode.RETI: // RETI
                        PC = PopWord();
                        _interrupt.ReturnFromInterrupt();
                        cycles = Z80CycleTiming.RETI;
                        break;
                    case Z80OpCode.RETN: // RETN
                        PC = PopWord();
                        _interrupt.ReturnFromNonMaskableInterrupt();
                        cycles = Z80CycleTiming.RETN;
                        break;
                    default:
                        throw new NotImplementedException($"Extended opcode 0xED{extendedOpcode:X2} not implemented");
                }
                break;
            
            // RST instructions (used by interrupt mode 0)
            case Z80OpCode.RST_00: PushWord(PC); PC = 0x0000; cycles = Z80CycleTiming.PUSH_qq; break; // RST 00h
            case Z80OpCode.RST_08: PushWord(PC); PC = 0x0008; cycles = Z80CycleTiming.PUSH_qq; break; // RST 08h
            case Z80OpCode.RST_10: PushWord(PC); PC = 0x0010; cycles = Z80CycleTiming.PUSH_qq; break; // RST 10h
            case Z80OpCode.RST_18: PushWord(PC); PC = 0x0018; cycles = Z80CycleTiming.PUSH_qq; break; // RST 18h
            case Z80OpCode.RST_20: PushWord(PC); PC = 0x0020; cycles = Z80CycleTiming.PUSH_qq; break; // RST 20h
            case Z80OpCode.RST_28: PushWord(PC); PC = 0x0028; cycles = Z80CycleTiming.PUSH_qq; break; // RST 28h
            case Z80OpCode.RST_30: PushWord(PC); PC = 0x0030; cycles = Z80CycleTiming.PUSH_qq; break; // RST 30h
            case Z80OpCode.RST_38: PushWord(PC); PC = 0x0038; cycles = Z80CycleTiming.PUSH_qq; break; // RST 38h
            
            // System
            case Z80OpCode.HALT: // HALT
                Halted = true;
                cycles = Z80CycleTiming.HALT;
                break;
            
            default:
                throw new NotImplementedException($"Opcode 0x{opcode:X2} not implemented");
        }

        // Add the cycles for this instruction to the total count
        _totalCycles += cycles;
    }
    
    // Handle interrupt processing
    private void HandleInterrupt(Z80InterruptRequest interruptRequest)
    {
        // Process the interrupt and get vector/instruction
        byte instruction = _interrupt.ProcessInterrupt(interruptRequest, out ushort vector);
        
        if (interruptRequest.Type == Z80InterruptType.NonMaskable)
        {
            // NMI: Push PC and jump to 0x0066
            PushWord(PC);
            PC = vector;
            _totalCycles += Z80CycleTiming.NMI_CYCLES;
            Halted = false; // NMI wakes up from HALT
        }
        else
        {
            // Maskable interrupt
            PushWord(PC);
            
            switch (_interrupt.InterruptMode)
            {
                case Z80InterruptMode.Mode0:
                    // Execute instruction placed on bus by interrupting device
                    // This is typically a RST instruction
                    ExecuteInterruptInstruction(instruction);
                    _totalCycles += Z80CycleTiming.INT_MODE0_CYCLES;
                    break;
                
                case Z80InterruptMode.Mode1:
                    // Jump to fixed vector 0x0038
                    PC = vector;
                    _totalCycles += Z80CycleTiming.INT_MODE1_CYCLES;
                    break;
                
                case Z80InterruptMode.Mode2:
                    // Vector table lookup
                    ushort vectorLow = _memory.ReadByte(vector);
                    ushort vectorHigh = _memory.ReadByte((ushort)(vector + 1));
                    PC = (ushort)(vectorLow | (vectorHigh << 8));
                    _totalCycles += Z80CycleTiming.INT_MODE2_CYCLES;
                    break;
            }
            
            Halted = false; // Interrupts wake up from HALT
        }
    }
    
    /// <summary>
    /// Execute instruction placed on bus during IM0 interrupt
    /// </summary>
    private void ExecuteInterruptInstruction(byte instruction)
    {
        // Most interrupt devices place RST instructions on the bus
        // RST n: Push PC, jump to n*8
        if ((instruction & 0xC7) == 0xC7)
        {
            // RST instruction: bits 7,6,0 = 1,1,1 and bits 5,4,3 = restart address
            byte rstAddress = (byte)((instruction & 0x38) >> 3);
            PC = (ushort)(rstAddress * 8);
        }
        else
        {
            // For non-RST instructions, we would need to decode and execute
            // For now, treat as NOP with warning
            PC = 0x0038; // Default to IM1 vector as fallback
        }
    }

    // Helper methods for register pairs
    private ushort GetBC() => (ushort)((B << 8) | C);
    private ushort GetDE() => (ushort)((D << 8) | E);
    private ushort GetHL() => (ushort)((H << 8) | L);
    private ushort GetAF() => (ushort)((A << 8) | F);
    
    private void SetBC(ushort value)
    {
        B = (byte)(value >> 8);
        C = (byte)(value & Z80OpCode.BYTE_MASK);
    }
    
    private void SetDE(ushort value)
    {
        D = (byte)(value >> 8);
        E = (byte)(value & Z80OpCode.BYTE_MASK);
    }
    
    private void SetHL(ushort value)
    {
        H = (byte)(value >> 8);
        L = (byte)(value & Z80OpCode.BYTE_MASK);
    }
    
    private void SetAF(ushort value)
    {
        A = (byte)(value >> 8);
        F = (byte)(value & Z80OpCode.BYTE_MASK);
    }
    
    // Arithmetic helper methods
    private void AddToA(byte value)
    {
        int result = A + value;
        SetZeroFlag((result & 0xFF) == 0);
        SetCarryFlag(result > 0xFF);
        A = (byte)result;
    }
    
    private byte IncByte(byte value)
    {
        byte result = (byte)((value + 1) & Z80OpCode.BYTE_MASK);
        SetZeroFlag(result == 0);
        return result;
    }
    
    private byte DecByte(byte value)
    {
        byte result = (byte)((value - 1) & Z80OpCode.BYTE_MASK);
        SetZeroFlag(result == 0);
        return result;
    }

    // Memory access wrappers that invoke debug hooks
    private byte ReadByte(ushort address)
    {
        var value = _memory.ReadByte(address);
        MemoryReadHook?.Invoke(address, value);
        return value;
    }

    private void WriteByte(ushort address, byte value)
    {
        _memory.WriteByte(address, value);
        MemoryWriteHook?.Invoke(address, value);
    }
    
    // Stack operations
    private void PushWord(ushort value)
    {
        WriteByte(--SP, (byte)(value >> 8));
        WriteByte(--SP, (byte)(value & Z80OpCode.BYTE_MASK));
    }
    
    private ushort PopWord()
    {
        byte low = ReadByte(SP++);
        byte high = ReadByte(SP++);
        return (ushort)((high << 8) | low);
    }
    
    // Flag operations (Z80 flags are in F register)
    private bool GetZeroFlag() => (F & Z80OpCode.FLAG_ZERO) != 0;
    private bool GetCarryFlag() => (F & Z80OpCode.FLAG_CARRY) != 0;
    
    private void SetZeroFlag(bool value)
    {
        if (value)
        {
            F |= Z80OpCode.FLAG_ZERO;
        }
        else
        {
            F &= Z80OpCode.FLAG_ZERO_MASK;
        }
    }
    
    private void SetCarryFlag(bool value)
    {
        if (value)
        {
            F |= Z80OpCode.FLAG_CARRY;
        }
        else
        {
            F &= Z80OpCode.FLAG_CARRY_MASK;
        }
    }
}
