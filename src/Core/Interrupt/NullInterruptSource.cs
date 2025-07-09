// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core.Interrupt;

/// <summary>
/// Represents a null interrupt source that implements the Null Object pattern.
/// This provides a safe default when no interrupt source is specified, eliminating null checks.
/// </summary>
public sealed class NullInterruptSource : InterruptSourceBase
{
    /// <summary>
    /// Singleton instance of the null interrupt source
    /// </summary>
    public static readonly NullInterruptSource Instance = new();

    private NullInterruptSource() { }

    /// <summary>
    /// Gets the unique identifier for this interrupt source
    /// </summary>
    public override string Id => "NULL";

    /// <summary>
    /// Gets a human-readable name for this interrupt source
    /// </summary>
    public override string Name => "No Source";

    /// <summary>
    /// Gets the description of this interrupt source
    /// </summary>
    public override string Description => "No interrupt source specified";

    /// <summary>
    /// Gets the category of this interrupt source
    /// </summary>
    public override InterruptSourceCategory Category => InterruptSourceCategory.System;

    /// <summary>
    /// Gets whether this is a null interrupt source
    /// </summary>
    public override bool IsNull => true;
}
