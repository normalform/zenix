// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;

namespace Zenix.Core.Interrupt;

/// <summary>
/// Asynchronous interrupt emulator for injecting interrupts from external sources
/// </summary>
public class AsyncInterruptEmulator : IDisposable
{
    private readonly IZ80Interrupt _interruptController;
    private readonly ConcurrentQueue<InterruptEmulationRequest> _pendingRequests = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Task _emulationTask;
    private volatile bool _disposed = false;

    /// <summary>
    /// Interrupt emulation request
    /// </summary>
    public record InterruptEmulationRequest(
        Z80InterruptRequest InterruptRequest,
        TimeSpan Delay,
        bool Repeat = false,
        TimeSpan RepeatInterval = default,
        string? Description = null
    );

    /// <summary>
    /// Event fired when an interrupt is emulated
    /// </summary>
    public event EventHandler<InterruptEmulatedEventArgs>? InterruptEmulated;

    /// <summary>
    /// Event arguments for interrupt emulated event
    /// </summary>
    public class InterruptEmulatedEventArgs : EventArgs
    {
        public Z80InterruptRequest InterruptRequest { get; }
        public string? Description { get; }
        public DateTime Timestamp { get; }

        public InterruptEmulatedEventArgs(Z80InterruptRequest request, string? description = null)
        {
            InterruptRequest = request;
            Description = description;
            Timestamp = DateTime.UtcNow;
        }
    }

    public AsyncInterruptEmulator(IZ80Interrupt interruptController)
    {
        _interruptController = interruptController ?? throw new ArgumentNullException(nameof(interruptController));
        _emulationTask = Task.Run(EmulationLoop, _cancellationTokenSource.Token);
    }

    /// <summary>
    /// Schedule a maskable interrupt to be triggered after a delay
    /// </summary>
    /// <param name="delay">Delay before triggering the interrupt</param>
    /// <param name="vector">Interrupt vector (default 0xFF)</param>
    /// <param name="priority">Priority level (default 0)</param>
    /// <param name="source">Source identifier</param>
    /// <param name="description">Description for debugging</param>
    public void ScheduleMaskableInterrupt(
        TimeSpan delay, 
        byte vector, 
        int priority, 
        InterruptSourceBase source,
        string? description = null)
    {
        if (_disposed)
        {
            return;
        }

        var request = new InterruptEmulationRequest(
            new Z80InterruptRequest(Z80InterruptType.Maskable, vector, priority, source),
            delay,
            Description: description ?? $"Async maskable interrupt from {source.Name}"
        );

        _pendingRequests.Enqueue(request);
    }

    /// <summary>
    /// Schedule a maskable interrupt to be triggered after a delay with default source
    /// </summary>
    /// <param name="delay">Delay before triggering the interrupt</param>
    /// <param name="vector">Interrupt vector (default 0xFF)</param>
    /// <param name="priority">Priority level (default 0)</param>
    /// <param name="description">Description for debugging</param>
    public void ScheduleMaskableInterrupt(
        TimeSpan delay, 
        byte vector = 0xFF, 
        int priority = 0, 
        string? description = null)
    {
        ScheduleMaskableInterrupt(delay, vector, priority, IoDeviceInterruptSource.Default, description);
    }

    /// <summary>
    /// Schedule a non-maskable interrupt to be triggered after a delay
    /// </summary>
    /// <param name="delay">Delay before triggering the interrupt</param>
    /// <param name="source">Source identifier</param>
    /// <param name="description">Description for debugging</param>
    public void ScheduleNonMaskableInterrupt(
        TimeSpan delay, 
        InterruptSourceBase source,
        string? description = null)
    {
        if (_disposed)
        {
            return;
        }

        var request = new InterruptEmulationRequest(
            new Z80InterruptRequest(Z80InterruptType.NonMaskable, 0, 0, source),
            delay,
            Description: description ?? $"Async NMI from {source.Name}"
        );

        _pendingRequests.Enqueue(request);
    }

    /// <summary>
    /// Schedule a non-maskable interrupt to be triggered after a delay with default NMI source
    /// </summary>
    /// <param name="delay">Delay before triggering the interrupt</param>
    /// <param name="description">Description for debugging</param>
    public void ScheduleNonMaskableInterrupt(
        TimeSpan delay, 
        string? description = null)
    {
        ScheduleNonMaskableInterrupt(delay, NmiInterruptSource.Instance, description);
    }

    /// <summary>
    /// Schedule a repeating maskable interrupt (e.g., for timer interrupts)
    /// </summary>
    /// <param name="initialDelay">Initial delay before first interrupt</param>
    /// <param name="interval">Interval between repeating interrupts</param>
    /// <param name="vector">Interrupt vector (default 0xFF)</param>
    /// <param name="priority">Priority level (default 0)</param>
    /// <param name="source">Source identifier</param>
    /// <param name="description">Description for debugging</param>
    public void ScheduleRepeatingInterrupt(
        TimeSpan initialDelay,
        TimeSpan interval,
        byte vector,
        int priority,
        InterruptSourceBase source,
        string? description = null)
    {
        if (_disposed)
        {
            return;
        }

        var request = new InterruptEmulationRequest(
            new Z80InterruptRequest(Z80InterruptType.Maskable, vector, priority, source),
            initialDelay,
            Repeat: true,
            RepeatInterval: interval,
            Description: description ?? $"Repeating interrupt from {source.Name} every {interval.TotalMilliseconds}ms"
        );

        _pendingRequests.Enqueue(request);
    }

    /// <summary>
    /// Schedule a repeating maskable interrupt with default timer source
    /// </summary>
    /// <param name="initialDelay">Initial delay before first interrupt</param>
    /// <param name="interval">Interval between repeating interrupts</param>
    /// <param name="vector">Interrupt vector (default 0xFF)</param>
    /// <param name="priority">Priority level (default 0)</param>
    /// <param name="description">Description for debugging</param>
    public void ScheduleRepeatingInterrupt(
        TimeSpan initialDelay,
        TimeSpan interval,
        byte vector = 0xFF,
        int priority = 0,
        string? description = null)
    {
        var timerSource = new TimerInterruptSource("REPEAT", "Repeating Timer", 1000.0 / interval.TotalMilliseconds);
        ScheduleRepeatingInterrupt(initialDelay, interval, vector, priority, timerSource, description);
    }

    /// <summary>
    /// Emulate a keyboard interrupt (typical MSX use case)
    /// </summary>
    /// <param name="delay">Delay before interrupt</param>
    public void EmulateKeyboardInterrupt(TimeSpan delay = default)
    {
        ScheduleMaskableInterrupt(
            delay, 
            vector: 0xFF, 
            priority: 10, 
            source: new IoDeviceInterruptSource("KEYBOARD", "Keyboard"),
            description: "Keyboard input interrupt"
        );
    }

    /// <summary>
    /// Emulate a VDP (Video Display Processor) interrupt (typical MSX use case)
    /// </summary>
    /// <param name="delay">Delay before interrupt</param>
    public void EmulateVdpInterrupt(TimeSpan delay = default)
    {
        ScheduleMaskableInterrupt(
            delay, 
            vector: 0xFF, 
            priority: 5, 
            source: VdpInterruptSource.VerticalBlank,
            description: "VDP vertical blank interrupt"
        );
    }

    /// <summary>
    /// Emulate a system reset via NMI
    /// </summary>
    /// <param name="delay">Delay before NMI</param>
    public void EmulateSystemReset(TimeSpan delay = default)
    {
        ScheduleNonMaskableInterrupt(
            delay,
            source: SystemInterruptSource.Reset,
            description: "System reset NMI"
        );
    }

    /// <summary>
    /// Clear all pending interrupt emulation requests
    /// </summary>
    public void ClearPendingRequests()
    {
        while (_pendingRequests.TryDequeue(out _)) { }
    }

    /// <summary>
    /// Get the number of pending interrupt emulation requests
    /// </summary>
    public int PendingRequestCount => _pendingRequests.Count;

    private async Task EmulationLoop()
    {
        var activeRequests = new List<(InterruptEmulationRequest Request, DateTime ScheduledTime)>();

        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                // Process new requests
                while (_pendingRequests.TryDequeue(out var newRequest))
                {
                    var scheduledTime = DateTime.UtcNow + newRequest.Delay;
                    activeRequests.Add((newRequest, scheduledTime));
                }

                // Check for ready interrupts
                var now = DateTime.UtcNow;
                var readyRequests = activeRequests.Where(x => x.ScheduledTime <= now).ToList();

                foreach (var (request, _) in readyRequests)
                {
                    // Trigger the interrupt
                    _interruptController.RequestInterrupt(request.InterruptRequest);
                    
                    // Fire event
                    InterruptEmulated?.Invoke(this, new InterruptEmulatedEventArgs(
                        request.InterruptRequest, 
                        request.Description
                    ));

                    // Handle repeating requests
                    if (request.Repeat && request.RepeatInterval > TimeSpan.Zero)
                    {
                        var nextScheduledTime = now + request.RepeatInterval;
                        activeRequests.Add((request, nextScheduledTime));
                    }
                }

                // Remove processed requests
                activeRequests.RemoveAll(x => readyRequests.Any(r => ReferenceEquals(r.Request, x.Request) && !x.Request.Repeat));

                // Sleep for a short time to avoid busy waiting
                await Task.Delay(1, _cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
        }
        catch (Exception ex)
        {
            // Log or handle unexpected exceptions
            Console.WriteLine($"AsyncInterruptEmulator error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _cancellationTokenSource.Cancel();
            
            try
            {
                _emulationTask.Wait(TimeSpan.FromSeconds(1));
            }
            catch (AggregateException)
            {
                // Ignore cancellation exceptions during disposal
            }
            
            _cancellationTokenSource.Dispose();
        }
    }
}
