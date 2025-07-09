// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core.Interrupt;

/// <summary>
/// Represents a system-level interrupt source for reset and control operations
/// </summary>
public sealed class SystemInterruptSource : InterruptSourceBase
{
    /// <summary>
    /// System reset interrupt source
    /// </summary>
    public static readonly SystemInterruptSource Reset = new("SYSTEM_RESET", "System Reset");

    /// <summary>
    /// Creates a new system interrupt source
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <param name="name">Human-readable name</param>
    public SystemInterruptSource(string id, string name)
    {
        Id = id;
        Name = name;
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
    /// Gets the category of this interrupt source
    /// </summary>
    public override InterruptSourceCategory Category => InterruptSourceCategory.System;
}
