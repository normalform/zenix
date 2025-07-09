// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core.Interrupt;

/// <summary>
/// Represents a timer-based interrupt source
/// </summary>
public sealed class TimerInterruptSource : InterruptSourceBase
{
    /// <summary>
    /// Default timer interrupt source for repeating operations
    /// </summary>
    public static readonly TimerInterruptSource Default = new("DEFAULT", "Default Timer", 60.0);

    /// <summary>
    /// Creates a new timer interrupt source
    /// </summary>
    /// <param name="timerId">Unique timer identifier</param>
    /// <param name="name">Human-readable timer name</param>
    /// <param name="frequency">Timer frequency in Hz</param>
    public TimerInterruptSource(string timerId, string name, double frequency)
    {
        Id = $"TIMER_{timerId}";
        Name = name;
        Frequency = frequency;
    }

    /// <summary>
    /// Gets the unique identifier for this interrupt source
    /// </summary>
    public override string Id { get; }

    /// <summary>
    /// Gets a human-readable name for this interrupt source
    /// </summary>
    public override string Name { get; }

    /// <summary>
    /// Gets the timer frequency in Hz
    /// </summary>
    public double Frequency { get; }

    /// <summary>
    /// Gets the description of this interrupt source
    /// </summary>
    public override string Description => $"{Name} - {Frequency:F1} Hz timer";

    /// <summary>
    /// Gets the category of this interrupt source
    /// </summary>
    public override InterruptSourceCategory Category => InterruptSourceCategory.Timer;
}
