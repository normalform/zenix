// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core;

/// <summary>
/// Z80 CPU instruction cycle timing constants for cycle-accurate emulation.
/// Based on official Z80 documentation and timing specifications.
/// </summary>
public static class Z80CycleTiming
{
    #region Basic Operations
    
    /// <summary>NOP - No Operation (4 cycles)</summary>
    public const byte NOP = 4;
    
    /// <summary>HALT - Halt CPU (4 cycles)</summary>
    public const byte HALT = 4;
    
    #endregion

    #region 8-bit Load Instructions (immediate)
    
    /// <summary>LD r, n - Load 8-bit immediate to register (7 cycles)</summary>
    public const byte LD_r_n = 7;
    
    #endregion

    #region 8-bit Register to Register Loads
    
    /// <summary>LD r, r' - Load register to register (4 cycles)</summary>
    public const byte LD_r_r = 4;
    
    #endregion

    #region 16-bit Load Instructions
    
    /// <summary>LD dd, nn - Load 16-bit immediate to register pair (10 cycles)</summary>
    public const byte LD_dd_nn = 10;
    
    #endregion

    #region Memory Operations
    
    /// <summary>LD (nn), A - Store A to memory address (13 cycles)</summary>
    public const byte LD_nn_A = 13;
    
    /// <summary>LD A, (nn) - Load A from memory address (13 cycles)</summary>
    public const byte LD_A_nn = 13;
    
    /// <summary>LD (HL), r - Store register to memory at HL (7 cycles)</summary>
    public const byte LD_HL_r = 7;
    
    /// <summary>LD r, (HL) - Load register from memory at HL (7 cycles)</summary>
    public const byte LD_r_HL = 7;
    
    #endregion

    #region Arithmetic Operations
    
    /// <summary>ADD A, r - Add register to A (4 cycles)</summary>
    public const byte ADD_A_r = 4;
    
    /// <summary>ADD A, n - Add immediate to A (7 cycles)</summary>
    public const byte ADD_A_n = 7;
    
    #endregion

    #region Increment/Decrement Operations
    
    /// <summary>INC r - Increment register (4 cycles)</summary>
    public const byte INC_r = 4;
    
    /// <summary>DEC r - Decrement register (4 cycles)</summary>
    public const byte DEC_r = 4;
    
    #endregion

    #region Jump Instructions
    
    /// <summary>JP nn - Absolute jump (10 cycles)</summary>
    public const byte JP_nn = 10;
    
    /// <summary>JR e - Relative jump (12 cycles when taken, 7 when not taken)</summary>
    public const byte JR_e_taken = 12;
    public const byte JR_e_not_taken = 7;
    
    /// <summary>JR cc, e - Conditional relative jump (12 cycles when taken, 7 when not taken)</summary>
    public const byte JR_cc_e_taken = 12;
    public const byte JR_cc_e_not_taken = 7;
    
    #endregion

    #region Stack Operations
    
    /// <summary>PUSH qq - Push register pair to stack (11 cycles)</summary>
    public const byte PUSH_qq = 11;
    
    /// <summary>POP qq - Pop register pair from stack (10 cycles)</summary>
    public const byte POP_qq = 10;
    
    #endregion

    #region Clock Frequency Constants
    
    /// <summary>Standard Z80 clock frequency (4 MHz)</summary>
    public const uint CLOCK_FREQUENCY_HZ = 4_000_000;
    
    /// <summary>Cycles per millisecond at 4MHz</summary>
    public const uint CYCLES_PER_MILLISECOND = CLOCK_FREQUENCY_HZ / 1000;
    
    /// <summary>Cycles per second at 4MHz</summary>
    public const uint CYCLES_PER_SECOND = CLOCK_FREQUENCY_HZ;
    
    /// <summary>Maximum cycle count for 10+ years of operation at 4MHz</summary>
    /// <remarks>
    /// Calculation: 4MHz × 60s × 60m × 24h × 365.25d × 10y = ~1.26 × 10^15 cycles
    /// This requires a 64-bit counter (ulong) which can hold up to 1.84 × 10^19
    /// </remarks>
    public const ulong MAX_CYCLE_COUNT = ulong.MaxValue;
    
    #endregion
}
