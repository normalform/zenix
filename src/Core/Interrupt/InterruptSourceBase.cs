// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core.Interrupt;

/// <summary>
/// Abstract base class representing an interrupt source in the Z80 system
/// </summary>
public abstract class InterruptSourceBase
{
    /// <summary>
    /// Gets the unique identifier for this interrupt source
    /// </summary>
    public abstract string Id { get; }

    /// <summary>
    /// Gets a human-readable name for this interrupt source
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the description of this interrupt source
    /// </summary>
    public virtual string Description => Name;

    /// <summary>
    /// Gets the category of this interrupt source
    /// </summary>
    public abstract InterruptSourceCategory Category { get; }

    /// <summary>
    /// Gets whether this is a null/unknown interrupt source
    /// </summary>
    public virtual bool IsNull => false;

    /// <summary>
    /// Returns a string representation of this interrupt source
    /// </summary>
    public override string ToString() => $"{Name} ({Id})";

    /// <summary>
    /// Determines whether two interrupt sources are equal
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is InterruptSourceBase other && Id == other.Id;
    }

    /// <summary>
    /// Gets the hash code for this interrupt source
    /// </summary>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(InterruptSourceBase? left, InterruptSourceBase? right)
    {
        return left?.Id == right?.Id;
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(InterruptSourceBase? left, InterruptSourceBase? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Categories of interrupt sources
/// </summary>
public enum InterruptSourceCategory
{
    /// <summary>CPU internal interrupts (NMI, etc.)</summary>
    Cpu,
    
    /// <summary>Timer-based interrupts</summary>
    Timer,
    
    /// <summary>Input/Output device interrupts</summary>
    IO,
    
    /// <summary>Video display processor interrupts</summary>
    Video,
    
    /// <summary>Audio processor interrupts</summary>
    Audio,
    
    /// <summary>External hardware interrupts</summary>
    External,
    
    /// <summary>System-level interrupts</summary>
    System,
    
    /// <summary>Test or mock interrupt sources</summary>
    Test,
    
    /// <summary>Unknown or unspecified category</summary>
    Unknown
}
