// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core.Interrupt;

/// <summary>
/// Represents an I/O device interrupt source
/// </summary>
public sealed class IoDeviceInterruptSource : InterruptSourceBase
{
    /// <summary>
    /// Default I/O device interrupt source for async operations
    /// </summary>
    public static readonly IoDeviceInterruptSource Default = new("ASYNC", "Async Device");

    /// <summary>
    /// Creates a new I/O device interrupt source
    /// </summary>
    /// <param name="deviceId">Unique device identifier</param>
    /// <param name="deviceName">Human-readable device name</param>
    /// <param name="portAddress">I/O port address (optional)</param>
    public IoDeviceInterruptSource(string deviceId, string deviceName, byte? portAddress = null)
    {
        Id = $"IO_{deviceId}";
        Name = deviceName;
        PortAddress = portAddress;
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
    /// Gets the I/O port address (if applicable)
    /// </summary>
    public byte? PortAddress { get; }

    /// <summary>
    /// Gets the description of this interrupt source
    /// </summary>
    public override string Description => PortAddress.HasValue 
        ? $"{Name} (Port 0x{PortAddress:X2})" 
        : Name;

    /// <summary>
    /// Gets the category of this interrupt source
    /// </summary>
    public override InterruptSourceCategory Category => InterruptSourceCategory.IO;
}
