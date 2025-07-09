// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core.Interrupt;

/// <summary>
/// Z80 interrupt modes
/// </summary>
public enum Z80InterruptMode
{
    /// <summary>IM 0 - External device places instruction on data bus</summary>
    Mode0 = 0,
    /// <summary>IM 1 - Fixed vector to 0x0038</summary>
    Mode1 = 1,
    /// <summary>IM 2 - Vector table using I register</summary>
    Mode2 = 2
}

/// <summary>
/// Z80 interrupt types
/// </summary>
public enum Z80InterruptType
{
    /// <summary>Maskable interrupt (INT)</summary>
    Maskable,
    /// <summary>Non-maskable interrupt (NMI)</summary>
    NonMaskable
}

/// <summary>
/// Z80 interrupt request
/// </summary>
public struct Z80InterruptRequest
{
    /// <summary>Type of interrupt</summary>
    public Z80InterruptType Type { get; init; }
    
    /// <summary>Vector or instruction for IM0/IM2 (ignored for IM1 and NMI)</summary>
    public byte Vector { get; init; }
    
    /// <summary>Priority level (lower number = higher priority)</summary>
    public int Priority { get; init; }
    
    /// <summary>Source device identifier</summary>
    public InterruptSourceBase Source { get; init; }
    
    public Z80InterruptRequest(Z80InterruptType type, byte vector = 0, int priority = 0, InterruptSourceBase? source = null)
    {
        Type = type;
        Vector = vector;
        Priority = priority;
        Source = source ?? NullInterruptSource.Instance;
    }
}

/// <summary>
/// Z80 interrupt controller for accurate interrupt handling emulation
/// </summary>
public class Z80Interrupt : IZ80Interrupt
{
    private readonly Queue<Z80InterruptRequest> _pendingInterrupts = new();
    private readonly object _interruptLock = new();
    
    // Interrupt state
    private bool _iff1 = false; // Interrupt flip-flop 1 (current interrupt enable state)
    private bool _iff2 = false; // Interrupt flip-flop 2 (previous interrupt enable state)
    private Z80InterruptMode _interruptMode = Z80InterruptMode.Mode0;
    private byte _interruptVector = 0; // I register for IM2
    private bool _nmiPending = false;
    private bool _nmiTriggered = false;
    
    // Interrupt timing
    private bool _interruptSamplePoint = false; // True when CPU samples for interrupts
    private int _eiDelay = 0; // EI instruction has 1-instruction delay
    
    /// <summary>
    /// Current interrupt mode (IM 0, 1, or 2)
    /// </summary>
    public Z80InterruptMode InterruptMode 
    { 
        get => _interruptMode;
        set => _interruptMode = value;
    }
    
    /// <summary>
    /// Interrupt vector register (I register) for IM2
    /// </summary>
    public byte InterruptVector 
    { 
        get => _interruptVector;
        set => _interruptVector = value;
    }
    
    /// <summary>
    /// Master interrupt enable flag (IFF1)
    /// </summary>
    public bool InterruptsEnabled => _iff1;
    
    /// <summary>
    /// Previous interrupt enable state (IFF2)
    /// </summary>
    public bool PreviousInterruptsEnabled => _iff2;
    
    /// <summary>
    /// Number of pending maskable interrupts
    /// </summary>
    public int PendingInterruptCount 
    { 
        get 
        { 
            lock (_interruptLock) 
            { 
                return _pendingInterrupts.Count(i => i.Type == Z80InterruptType.Maskable); 
            } 
        } 
    }
    
    /// <summary>
    /// True if NMI is pending
    /// </summary>
    public bool NmiPending => _nmiPending;
    
    /// <summary>
    /// Enable interrupts (EI instruction)
    /// </summary>
    public void EnableInterrupts()
    {
        _iff1 = true;
        _iff2 = true;
        _eiDelay = 1; // EI has one instruction delay
    }
    
    /// <summary>
    /// Disable interrupts (DI instruction)
    /// </summary>
    public void DisableInterrupts()
    {
        _iff1 = false;
        _iff2 = false;
        _eiDelay = 0;
    }
    
    /// <summary>
    /// Set interrupt mode (IM 0/1/2 instruction)
    /// </summary>
    public void SetInterruptMode(Z80InterruptMode mode)
    {
        _interruptMode = mode;
    }
    
    /// <summary>
    /// Request an interrupt
    /// </summary>
    public void RequestInterrupt(Z80InterruptRequest request)
    {
        lock (_interruptLock)
        {
            if (request.Type == Z80InterruptType.NonMaskable)
            {
                _nmiPending = true;
            }
            else
            {
                // Insert in priority order (lower priority number = higher priority)
                var inserted = false;
                var tempQueue = new Queue<Z80InterruptRequest>();
                
                while (_pendingInterrupts.Count > 0)
                {
                    var existing = _pendingInterrupts.Dequeue();
                    if (!inserted && request.Priority < existing.Priority)
                    {
                        tempQueue.Enqueue(request);
                        inserted = true;
                    }
                    tempQueue.Enqueue(existing);
                }
                
                if (!inserted)
                {
                    tempQueue.Enqueue(request);
                }
                
                // Restore queue with new order
                while (tempQueue.Count > 0)
                {
                    _pendingInterrupts.Enqueue(tempQueue.Dequeue());
                }
            }
        }
    }
    
    /// <summary>
    /// Request a maskable interrupt with default parameters
    /// </summary>
    public void RequestMaskableInterrupt(byte vector = 0xFF, int priority = 0, InterruptSourceBase? source = null)
    {
        RequestInterrupt(new Z80InterruptRequest(Z80InterruptType.Maskable, vector, priority, source));
    }
    
    /// <summary>
    /// Request a non-maskable interrupt
    /// </summary>
    public void RequestNonMaskableInterrupt(InterruptSourceBase? source = null)
    {
        RequestInterrupt(new Z80InterruptRequest(Z80InterruptType.NonMaskable, 0, 0, source ?? NmiInterruptSource.Instance));
    }
    
    /// <summary>
    /// Clear all pending maskable interrupts
    /// </summary>
    public void ClearMaskableInterrupts()
    {
        lock (_interruptLock)
        {
            _pendingInterrupts.Clear();
        }
    }
    
    /// <summary>
    /// Called by CPU before each instruction to handle interrupt timing
    /// </summary>
    public void BeforeInstruction()
    {
        // Handle EI delay
        if (_eiDelay > 0)
        {
            _eiDelay--;
        }
        
        // Set interrupt sample point
        _interruptSamplePoint = _eiDelay == 0;
    }
    
    /// <summary>
    /// Check if an interrupt should be processed
    /// </summary>
    public bool ShouldProcessInterrupt(out Z80InterruptRequest interrupt)
    {
        interrupt = default;
        
        // NMI has highest priority and is not maskable
        if (_nmiPending && !_nmiTriggered)
        {
            _nmiTriggered = true;
            _nmiPending = false;
            interrupt = new Z80InterruptRequest(Z80InterruptType.NonMaskable, 0, -1, NmiInterruptSource.Instance);
            return true;
        }
        
        // Check maskable interrupts only if enabled and at sample point
        if (_iff1 && _interruptSamplePoint)
        {
            lock (_interruptLock)
            {
                if (_pendingInterrupts.Count > 0)
                {
                    interrupt = _pendingInterrupts.Dequeue();
                    return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Handle interrupt acknowledgment and return vector/instruction
    /// </summary>
    public byte ProcessInterrupt(Z80InterruptRequest interrupt, out ushort vector)
    {
        vector = 0;
        
        if (interrupt.Type == Z80InterruptType.NonMaskable)
        {
            // NMI: Save IFF1 in IFF2, disable interrupts, jump to 0x0066
            _iff2 = _iff1;
            _iff1 = false;
            vector = 0x0066;
            _nmiTriggered = false; // Reset for next NMI
            return 0; // No instruction placed on bus for NMI
        }
        else
        {
            // Maskable interrupt: Save IFF1 in IFF2, disable interrupts
            _iff2 = _iff1;
            _iff1 = false;
            
            switch (_interruptMode)
            {
                case Z80InterruptMode.Mode0:
                    // IM0: External device places instruction on data bus
                    // Usually RST instructions (0xC7, 0xCF, 0xD7, 0xDF, 0xE7, 0xEF, 0xF7, 0xFF)
                    return interrupt.Vector;
                
                case Z80InterruptMode.Mode1:
                    // IM1: Fixed vector to 0x0038
                    vector = 0x0038;
                    return 0;
                
                case Z80InterruptMode.Mode2:
                    // IM2: Vector table using I register
                    // Vector = (I << 8) | interrupt.Vector
                    vector = (ushort)((_interruptVector << 8) | interrupt.Vector);
                    return 0;
                
                default:
                    throw new InvalidOperationException($"Invalid interrupt mode: {_interruptMode}");
            }
        }
    }
    
    /// <summary>
    /// Restore interrupts after RETN instruction
    /// </summary>
    public void ReturnFromNonMaskableInterrupt()
    {
        _iff1 = _iff2;
    }
    
    /// <summary>
    /// Restore interrupts after RETI instruction
    /// </summary>
    public void ReturnFromInterrupt()
    {
        _iff1 = _iff2;
        // RETI also signals interrupt controller that interrupt routine is complete
        // This is important for daisy-chained interrupt controllers
    }
    
    /// <summary>
    /// Reset interrupt controller to initial state
    /// </summary>
    public void Reset()
    {
        lock (_interruptLock)
        {
            _pendingInterrupts.Clear();
        }
        
        _iff1 = false;
        _iff2 = false;
        _interruptMode = Z80InterruptMode.Mode0;
        _interruptVector = 0;
        _nmiPending = false;
        _nmiTriggered = false;
        _interruptSamplePoint = false;
        _eiDelay = 0;
    }
    
    /// <summary>
    /// Get current interrupt controller state for debugging
    /// </summary>
    public string GetState()
    {
        lock (_interruptLock)
        {
            return $"IFF1={_iff1}, IFF2={_iff2}, IM={_interruptMode}, I=0x{_interruptVector:X2}, " +
                   $"Pending={_pendingInterrupts.Count}, NMI={_nmiPending}, EI_Delay={_eiDelay}";
        }
    }
}
