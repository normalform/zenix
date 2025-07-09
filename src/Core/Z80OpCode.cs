// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core;

/// <summary>
/// Z80 CPU opcode constants and related bit masks for improved code readability and maintainability.
/// </summary>
public static class Z80OpCode
{
    #region Basic Operations
    
    /// <summary>NOP - No Operation</summary>
    public const byte NOP = 0x00;
    
    /// <summary>HALT - Halt CPU</summary>
    public const byte HALT = 0x76;
    
    #endregion

    #region 8-bit Load Instructions (immediate)
    
    /// <summary>LD A, n - Load immediate value into A register</summary>
    public const byte LD_A_n = 0x3E;
    
    /// <summary>LD B, n - Load immediate value into B register</summary>
    public const byte LD_B_n = 0x06;
    
    /// <summary>LD C, n - Load immediate value into C register</summary>
    public const byte LD_C_n = 0x0E;
    
    /// <summary>LD D, n - Load immediate value into D register</summary>
    public const byte LD_D_n = 0x16;
    
    /// <summary>LD E, n - Load immediate value into E register</summary>
    public const byte LD_E_n = 0x1E;
    
    /// <summary>LD H, n - Load immediate value into H register</summary>
    public const byte LD_H_n = 0x26;
    
    /// <summary>LD L, n - Load immediate value into L register</summary>
    public const byte LD_L_n = 0x2E;
    
    #endregion

    #region 8-bit Register to Register Loads (LD A, r)
    
    /// <summary>LD A, A - Copy A register to A register</summary>
    public const byte LD_A_A = 0x7F;
    
    /// <summary>LD A, B - Copy B register to A register</summary>
    public const byte LD_A_B = 0x78;
    
    /// <summary>LD A, C - Copy C register to A register</summary>
    public const byte LD_A_C = 0x79;
    
    /// <summary>LD A, D - Copy D register to A register</summary>
    public const byte LD_A_D = 0x7A;
    
    /// <summary>LD A, E - Copy E register to A register</summary>
    public const byte LD_A_E = 0x7B;
    
    /// <summary>LD A, H - Copy H register to A register</summary>
    public const byte LD_A_H = 0x7C;
    
    /// <summary>LD A, L - Copy L register to A register</summary>
    public const byte LD_A_L = 0x7D;
    
    #endregion

    #region 8-bit Register to Register Loads (LD B, r)
    
    /// <summary>LD B, A - Copy A register to B register</summary>
    public const byte LD_B_A = 0x47;
    
    /// <summary>LD B, B - Copy B register to B register</summary>
    public const byte LD_B_B = 0x40;
    
    /// <summary>LD B, C - Copy C register to B register</summary>
    public const byte LD_B_C = 0x41;
    
    /// <summary>LD B, D - Copy D register to B register</summary>
    public const byte LD_B_D = 0x42;
    
    /// <summary>LD B, E - Copy E register to B register</summary>
    public const byte LD_B_E = 0x43;
    
    /// <summary>LD B, H - Copy H register to B register</summary>
    public const byte LD_B_H = 0x44;
    
    /// <summary>LD B, L - Copy L register to B register</summary>
    public const byte LD_B_L = 0x45;
    
    #endregion

    #region 8-bit Register to Register Loads (LD C, r)
    
    /// <summary>LD C, A - Copy A register to C register</summary>
    public const byte LD_C_A = 0x4F;
    
    /// <summary>LD C, B - Copy B register to C register</summary>
    public const byte LD_C_B = 0x48;
    
    /// <summary>LD C, C - Copy C register to C register</summary>
    public const byte LD_C_C = 0x49;
    
    /// <summary>LD C, D - Copy D register to C register</summary>
    public const byte LD_C_D = 0x4A;
    
    /// <summary>LD C, E - Copy E register to C register</summary>
    public const byte LD_C_E = 0x4B;
    
    /// <summary>LD C, H - Copy H register to C register</summary>
    public const byte LD_C_H = 0x4C;
    
    /// <summary>LD C, L - Copy L register to C register</summary>
    public const byte LD_C_L = 0x4D;
    
    #endregion

    #region 16-bit Load Instructions
    
    /// <summary>LD BC, nn - Load 16-bit immediate value into BC register pair</summary>
    public const byte LD_BC_nn = 0x01;
    
    /// <summary>LD DE, nn - Load 16-bit immediate value into DE register pair</summary>
    public const byte LD_DE_nn = 0x11;
    
    /// <summary>LD HL, nn - Load 16-bit immediate value into HL register pair</summary>
    public const byte LD_HL_nn = 0x21;
    
    /// <summary>LD SP, nn - Load 16-bit immediate value into SP register</summary>
    public const byte LD_SP_nn = 0x31;
    
    #endregion

    #region Memory Operations
    
    /// <summary>LD (nn), A - Store A register to memory address nn</summary>
    public const byte LD_nn_A = 0x32;
    
    /// <summary>LD A, (nn) - Load A register from memory address nn</summary>
    public const byte LD_A_nn = 0x3A;
    
    /// <summary>LD (HL), A - Store A register to memory address in HL</summary>
    public const byte LD_HL_A = 0x77;
    
    /// <summary>LD A, (HL) - Load A register from memory address in HL</summary>
    public const byte LD_A_HL = 0x7E;
    
    #endregion

    #region Arithmetic Operations
    
    /// <summary>ADD A, A - Add A register to A register</summary>
    public const byte ADD_A_A = 0x87;
    
    /// <summary>ADD A, B - Add B register to A register</summary>
    public const byte ADD_A_B = 0x80;
    
    /// <summary>ADD A, C - Add C register to A register</summary>
    public const byte ADD_A_C = 0x81;
    
    /// <summary>ADD A, D - Add D register to A register</summary>
    public const byte ADD_A_D = 0x82;
    
    /// <summary>ADD A, E - Add E register to A register</summary>
    public const byte ADD_A_E = 0x83;
    
    /// <summary>ADD A, H - Add H register to A register</summary>
    public const byte ADD_A_H = 0x84;
    
    /// <summary>ADD A, L - Add L register to A register</summary>
    public const byte ADD_A_L = 0x85;
    
    /// <summary>ADD A, n - Add immediate value to A register</summary>
    public const byte ADD_A_n = 0xC6;
    
    #endregion

    #region Increment Operations
    
    /// <summary>INC A - Increment A register</summary>
    public const byte INC_A = 0x3C;
    
    /// <summary>INC B - Increment B register</summary>
    public const byte INC_B = 0x04;
    
    /// <summary>INC C - Increment C register</summary>
    public const byte INC_C = 0x0C;
    
    /// <summary>INC D - Increment D register</summary>
    public const byte INC_D = 0x14;
    
    /// <summary>INC E - Increment E register</summary>
    public const byte INC_E = 0x1C;
    
    /// <summary>INC H - Increment H register</summary>
    public const byte INC_H = 0x24;
    
    /// <summary>INC L - Increment L register</summary>
    public const byte INC_L = 0x2C;
    
    #endregion

    #region Decrement Operations
    
    /// <summary>DEC A - Decrement A register</summary>
    public const byte DEC_A = 0x3D;
    
    /// <summary>DEC B - Decrement B register</summary>
    public const byte DEC_B = 0x05;
    
    /// <summary>DEC C - Decrement C register</summary>
    public const byte DEC_C = 0x0D;
    
    /// <summary>DEC D - Decrement D register</summary>
    public const byte DEC_D = 0x15;
    
    /// <summary>DEC E - Decrement E register</summary>
    public const byte DEC_E = 0x1D;
    
    /// <summary>DEC H - Decrement H register</summary>
    public const byte DEC_H = 0x25;
    
    /// <summary>DEC L - Decrement L register</summary>
    public const byte DEC_L = 0x2D;
    
    #endregion

    #region Jump Instructions
    
    /// <summary>JP nn - Jump to absolute address nn</summary>
    public const byte JP_nn = 0xC3;
    
    /// <summary>JR e - Jump relative by offset e</summary>
    public const byte JR_e = 0x18;
    
    /// <summary>JR NZ, e - Jump relative if zero flag is not set</summary>
    public const byte JR_NZ_e = 0x20;
    
    /// <summary>JR Z, e - Jump relative if zero flag is set</summary>
    public const byte JR_Z_e = 0x28;
    
    #endregion

    #region Stack Operations
    
    /// <summary>PUSH BC - Push BC register pair onto stack</summary>
    public const byte PUSH_BC = 0xC5;
    
    /// <summary>PUSH DE - Push DE register pair onto stack</summary>
    public const byte PUSH_DE = 0xD5;
    
    /// <summary>PUSH HL - Push HL register pair onto stack</summary>
    public const byte PUSH_HL = 0xE5;
    
    /// <summary>PUSH AF - Push AF register pair onto stack</summary>
    public const byte PUSH_AF = 0xF5;
    
    /// <summary>POP BC - Pop BC register pair from stack</summary>
    public const byte POP_BC = 0xC1;
    
    /// <summary>POP DE - Pop DE register pair from stack</summary>
    public const byte POP_DE = 0xD1;
    
    /// <summary>POP HL - Pop HL register pair from stack</summary>
    public const byte POP_HL = 0xE1;
    
    /// <summary>POP AF - Pop AF register pair from stack</summary>
    public const byte POP_AF = 0xF1;
    
    #endregion

    #region Flag Bit Masks
    
    /// <summary>Zero flag bit mask (bit 6)</summary>
    public const byte FLAG_ZERO = 0x40;
    
    /// <summary>Carry flag bit mask (bit 0)</summary>
    public const byte FLAG_CARRY = 0x01;
    
    /// <summary>Mask to clear zero flag (all bits except bit 6)</summary>
    public const byte FLAG_ZERO_MASK = 0xBF;
    
    /// <summary>Mask to clear carry flag (all bits except bit 0)</summary>
    public const byte FLAG_CARRY_MASK = 0xFE;
    
    #endregion

    #region Common Masks
    
    /// <summary>8-bit byte mask</summary>
    public const byte BYTE_MASK = 0xFF;
    
    #endregion

    #region Interrupt Instructions
    
    /// <summary>EI - Enable interrupts</summary>
    public const byte EI = 0xFB;
    
    /// <summary>DI - Disable interrupts</summary>
    public const byte DI = 0xF3;
    
    /// <summary>IM 0 - Set interrupt mode 0 (ED prefix)</summary>
    public const byte IM_0 = 0x46; // After ED prefix
    
    /// <summary>IM 1 - Set interrupt mode 1 (ED prefix)</summary>
    public const byte IM_1 = 0x56; // After ED prefix
    
    /// <summary>IM 2 - Set interrupt mode 2 (ED prefix)</summary>
    public const byte IM_2 = 0x5E; // After ED prefix
    
    /// <summary>RETI - Return from interrupt (ED prefix)</summary>
    public const byte RETI = 0x4D; // After ED prefix
    
    /// <summary>RETN - Return from non-maskable interrupt (ED prefix)</summary>
    public const byte RETN = 0x45; // After ED prefix
    
    /// <summary>ED prefix for extended instructions</summary>
    public const byte ED_PREFIX = 0xED;
    
    #endregion

    #region RST Instructions (for interrupt mode 0)
    
    /// <summary>RST 00h - Restart to address 0x0000</summary>
    public const byte RST_00 = 0xC7;
    
    /// <summary>RST 08h - Restart to address 0x0008</summary>
    public const byte RST_08 = 0xCF;
    
    /// <summary>RST 10h - Restart to address 0x0010</summary>
    public const byte RST_10 = 0xD7;
    
    /// <summary>RST 18h - Restart to address 0x0018</summary>
    public const byte RST_18 = 0xDF;
    
    /// <summary>RST 20h - Restart to address 0x0020</summary>
    public const byte RST_20 = 0xE7;
    
    /// <summary>RST 28h - Restart to address 0x0028</summary>
    public const byte RST_28 = 0xEF;
    
    /// <summary>RST 30h - Restart to address 0x0030</summary>
    public const byte RST_30 = 0xF7;
    
    /// <summary>RST 38h - Restart to address 0x0038</summary>
    public const byte RST_38 = 0xFF;
    
    #endregion
}
