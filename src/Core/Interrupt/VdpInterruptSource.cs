// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core.Interrupt;

/// <summary>
/// Represents a Video Display Processor interrupt source
/// </summary>
public sealed class VdpInterruptSource : InterruptSourceBase
{
    /// <summary>
    /// VDP vertical blank interrupt
    /// </summary>
    public static readonly VdpInterruptSource VerticalBlank = new("VBLANK", "Vertical Blank");

    /// <summary>
    /// VDP horizontal blank interrupt
    /// </summary>
    public static readonly VdpInterruptSource HorizontalBlank = new("HBLANK", "Horizontal Blank");

    /// <summary>
    /// VDP sprite collision interrupt
    /// </summary>
    public static readonly VdpInterruptSource SpriteCollision = new("SPRITE_COLLISION", "Sprite Collision");

    private VdpInterruptSource(string id, string name)
    {
        Id = $"VDP_{id}";
        Name = $"VDP {name}";
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
    /// Gets the description of this interrupt source
    /// </summary>
    public override string Description => $"{Name} interrupt";

    /// <summary>
    /// Gets the category of this interrupt source
    /// </summary>
    public override InterruptSourceCategory Category => InterruptSourceCategory.Video;
}
