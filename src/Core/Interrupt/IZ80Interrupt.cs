// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core.Interrupt;

/// <summary>
/// Interface for Z80 interrupt controller to enable dependency injection and mocking
/// </summary>
public interface IZ80Interrupt
{
    /// <summary>
    /// Current interrupt mode (IM 0, 1, or 2)
    /// </summary>
    Z80InterruptMode InterruptMode { get; set; }
    
    /// <summary>
    /// Interrupt vector register (I register) for IM2
    /// </summary>
    byte InterruptVector { get; set; }
    
    /// <summary>
    /// Master interrupt enable flag (IFF1)
    /// </summary>
    bool InterruptsEnabled { get; }
    
    /// <summary>
    /// Previous interrupt enable state (IFF2)
    /// </summary>
    bool PreviousInterruptsEnabled { get; }
    
    /// <summary>
    /// Number of pending maskable interrupts
    /// </summary>
    int PendingInterruptCount { get; }
    
    /// <summary>
    /// True if NMI is pending
    /// </summary>
    bool NmiPending { get; }
    
    /// <summary>
    /// Enable interrupts (EI instruction)
    /// </summary>
    void EnableInterrupts();
    
    /// <summary>
    /// Disable interrupts (DI instruction)
    /// </summary>
    void DisableInterrupts();
    
    /// <summary>
    /// Set interrupt mode (IM 0/1/2 instruction)
    /// </summary>
    /// <param name="mode">The interrupt mode to set</param>
    void SetInterruptMode(Z80InterruptMode mode);
    
    /// <summary>
    /// Request an interrupt
    /// </summary>
    /// <param name="request">The interrupt request to queue</param>
    void RequestInterrupt(Z80InterruptRequest request);
    
    /// <summary>
    /// Request a maskable interrupt with default parameters
    /// </summary>
    /// <param name="vector">Interrupt vector (default 0xFF)</param>
    /// <param name="priority">Priority level (default 0)</param>
    /// <param name="source">Source identifier (default null for unknown)</param>
    void RequestMaskableInterrupt(byte vector = 0xFF, int priority = 0, InterruptSourceBase? source = null);
    
    /// <summary>
    /// Request a non-maskable interrupt
    /// </summary>
    /// <param name="source">Source identifier (default null for NMI)</param>
    void RequestNonMaskableInterrupt(InterruptSourceBase? source = null);
    
    /// <summary>
    /// Clear all pending maskable interrupts
    /// </summary>
    void ClearMaskableInterrupts();
    
    /// <summary>
    /// Called by CPU before each instruction to handle interrupt timing
    /// </summary>
    void BeforeInstruction();
    
    /// <summary>
    /// Check if an interrupt should be processed
    /// </summary>
    /// <param name="interrupt">The interrupt request to process</param>
    /// <returns>True if an interrupt should be processed</returns>
    bool ShouldProcessInterrupt(out Z80InterruptRequest interrupt);
    
    /// <summary>
    /// Handle interrupt acknowledgment and return vector/instruction
    /// </summary>
    /// <param name="interrupt">The interrupt request being processed</param>
    /// <param name="vector">Output vector address for the interrupt</param>
    /// <returns>Instruction byte for IM0, or 0 for other modes</returns>
    byte ProcessInterrupt(Z80InterruptRequest interrupt, out ushort vector);
    
    /// <summary>
    /// Restore interrupts after RETN instruction
    /// </summary>
    void ReturnFromNonMaskableInterrupt();
    
    /// <summary>
    /// Restore interrupts after RETI instruction
    /// </summary>
    void ReturnFromInterrupt();
    
    /// <summary>
    /// Reset interrupt controller to initial state
    /// </summary>
    void Reset();
    
    /// <summary>
    /// Get current interrupt controller state for debugging
    /// </summary>
    /// <returns>String representation of current state</returns>
    string GetState();
}
