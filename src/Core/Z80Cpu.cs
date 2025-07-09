// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core;

public class Z80Cpu
{
    private readonly MsxMemoryMap _memory;
    public Z80CpuOptions Options { get; }

    // Cycle-accurate timing state
    private ulong _totalCycles = 0;
    private DateTime _emulationStartTime = DateTime.UtcNow;

    public Z80Cpu(MsxMemoryMap memory, Z80CpuOptions? options = null)
    {
        _memory = memory;
        Options = options ?? new Z80CpuOptions();
        _memory.Configure(Options.RomSize, Options.RamSize);
        Reset();
    }

    public byte A { get; private set; }
    public byte F { get; private set; }
    public byte B { get; private set; }
    public byte C { get; private set; }
    public byte D { get; private set; }
    public byte E { get; private set; }
    public byte H { get; private set; }
    public byte L { get; private set; }
    public ushort SP { get; private set; }
    public ushort PC { get; private set; }
    public bool Halted { get; private set; }

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
            if (elapsed.TotalSeconds < 0.001) return 0.0;
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
    }

    public void Step()
    {
        if (Halted) 
        {
            // HALT state still consumes cycles
            _totalCycles += Z80CycleTiming.HALT;
            return;
        }

        byte opcode = _memory.ReadByte(PC++);
        byte cycles = 0;

        switch (opcode)
        {
            // 8-bit Load instructions
            case Z80OpCode.NOP: // NOP
                cycles = Z80CycleTiming.NOP;
                break;
            case Z80OpCode.LD_A_n: // LD A, n
                A = _memory.ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_B_n: // LD B, n
                B = _memory.ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_C_n: // LD C, n
                C = _memory.ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_D_n: // LD D, n
                D = _memory.ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_E_n: // LD E, n
                E = _memory.ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_H_n: // LD H, n
                H = _memory.ReadByte(PC++);
                cycles = Z80CycleTiming.LD_r_n;
                break;
            case Z80OpCode.LD_L_n: // LD L, n
                L = _memory.ReadByte(PC++);
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
                C = _memory.ReadByte(PC++);
                B = _memory.ReadByte(PC++);
                cycles = Z80CycleTiming.LD_dd_nn;
                break;
            case Z80OpCode.LD_DE_nn: // LD DE, nn
                E = _memory.ReadByte(PC++);
                D = _memory.ReadByte(PC++);
                cycles = Z80CycleTiming.LD_dd_nn;
                break;
            case Z80OpCode.LD_HL_nn: // LD HL, nn
                L = _memory.ReadByte(PC++);
                H = _memory.ReadByte(PC++);
                cycles = Z80CycleTiming.LD_dd_nn;
                break;
            case Z80OpCode.LD_SP_nn: // LD SP, nn
                ushort spLow = _memory.ReadByte(PC++);
                ushort spHigh = _memory.ReadByte(PC++);
                SP = (ushort)(spLow | (spHigh << 8));
                cycles = Z80CycleTiming.LD_dd_nn;
                break;
            
            // Memory operations
            case Z80OpCode.LD_nn_A: // LD (nn), A
                ushort addrLow = _memory.ReadByte(PC++);
                ushort addrHigh = _memory.ReadByte(PC++);
                ushort addr = (ushort)(addrLow | (addrHigh << 8));
                _memory.WriteByte(addr, A);
                cycles = Z80CycleTiming.LD_nn_A;
                break;
            case Z80OpCode.LD_A_nn: // LD A, (nn)
                ushort loadAddrLow = _memory.ReadByte(PC++);
                ushort loadAddrHigh = _memory.ReadByte(PC++);
                ushort loadAddr = (ushort)(loadAddrLow | (loadAddrHigh << 8));
                A = _memory.ReadByte(loadAddr);
                cycles = Z80CycleTiming.LD_A_nn;
                break;
            case Z80OpCode.LD_HL_A: // LD (HL), A
                _memory.WriteByte(GetHL(), A);
                cycles = Z80CycleTiming.LD_HL_r;
                break;
            case Z80OpCode.LD_A_HL: // LD A, (HL)
                A = _memory.ReadByte(GetHL());
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
                AddToA(_memory.ReadByte(PC++));
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
                ushort low = _memory.ReadByte(PC++);
                ushort high = _memory.ReadByte(PC++);
                PC = (ushort)(low | (high << 8));
                cycles = Z80CycleTiming.JP_nn;
                break;
            case Z80OpCode.JR_e: // JR e (relative jump)
                sbyte offset = (sbyte)_memory.ReadByte(PC++);
                PC = (ushort)(PC + offset);
                cycles = Z80CycleTiming.JR_e_taken;
                break;
            case Z80OpCode.JR_NZ_e: // JR NZ, e
                sbyte nzOffset = (sbyte)_memory.ReadByte(PC++);
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
                sbyte zOffset = (sbyte)_memory.ReadByte(PC++);
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
        SetZeroFlag(result == 0);
        SetCarryFlag(result > 255);
        A = (byte)(result & Z80OpCode.BYTE_MASK);
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
    
    // Stack operations
    private void PushWord(ushort value)
    {
        _memory.WriteByte(--SP, (byte)(value >> 8));
        _memory.WriteByte(--SP, (byte)(value & Z80OpCode.BYTE_MASK));
    }
    
    private ushort PopWord()
    {
        byte low = _memory.ReadByte(SP++);
        byte high = _memory.ReadByte(SP++);
        return (ushort)((high << 8) | low);
    }
    
    // Flag operations (Z80 flags are in F register)
    private bool GetZeroFlag() => (F & Z80OpCode.FLAG_ZERO) != 0;
    private bool GetCarryFlag() => (F & Z80OpCode.FLAG_CARRY) != 0;
    
    private void SetZeroFlag(bool value)
    {
        if (value)
            F |= Z80OpCode.FLAG_ZERO;
        else
            F &= Z80OpCode.FLAG_ZERO_MASK;
    }
    
    private void SetCarryFlag(bool value)
    {
        if (value)
            F |= Z80OpCode.FLAG_CARRY;
        else
            F &= Z80OpCode.FLAG_CARRY_MASK;
    }
}
