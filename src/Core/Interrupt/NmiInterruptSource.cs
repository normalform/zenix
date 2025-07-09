// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core.Interrupt;

/// <summary>
/// Represents the Non-Maskable Interrupt source
/// </summary>
public sealed class NmiInterruptSource : InterruptSourceBase
{
    /// <summary>
    /// Singleton instance of the NMI interrupt source
    /// </summary>
    public static readonly NmiInterruptSource Instance = new();

    private NmiInterruptSource() { }

    /// <summary>
    /// Gets the unique identifier for this interrupt source
    /// </summary>
    public override string Id => "NMI";

    /// <summary>
    /// Gets a human-readable name for this interrupt source
    /// </summary>
    public override string Name => "Non-Maskable Interrupt";

    /// <summary>
    /// Gets the description of this interrupt source
    /// </summary>
    public override string Description => "Z80 CPU Non-Maskable Interrupt";

    /// <summary>
    /// Gets the category of this interrupt source
    /// </summary>
    public override InterruptSourceCategory Category => InterruptSourceCategory.Cpu;
}
