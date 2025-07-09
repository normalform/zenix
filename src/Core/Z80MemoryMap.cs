// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core;

/// <summary>
/// Generic Z80 memory map implementation providing 64KB address space
/// with configurable ROM and RAM regions
/// </summary>
public class Z80MemoryMap
{
    private readonly byte[] _memory = new byte[0x10000];
    private int _romSize;
    private int _ramSize;

    /// <summary>
    /// Configure the memory layout with ROM and RAM sizes
    /// </summary>
    /// <param name="romSize">Size of ROM region in bytes</param>
    /// <param name="ramSize">Size of RAM region in bytes</param>
    public void Configure(int romSize, int ramSize)
    {
        _romSize = Math.Clamp(romSize, 0, 0x10000);
        _ramSize = Math.Clamp(ramSize, 0, 0x10000 - _romSize);
    }

    /// <summary>
    /// Load ROM data into memory starting at address 0x0000
    /// </summary>
    /// <param name="data">ROM data to load</param>
    public void LoadRom(byte[] data)
    {
        var length = Math.Min(data.Length, _memory.Length);
        Array.Clear(_memory, 0, _memory.Length);
        Array.Copy(data, 0, _memory, 0, length);
        _romSize = length;
    }

    /// <summary>
    /// Read a byte from the specified memory address
    /// </summary>
    /// <param name="address">Memory address to read from</param>
    /// <returns>Byte value at the specified address</returns>
    public byte ReadByte(ushort address) => _memory[address];

    /// <summary>
    /// Write a byte to the specified memory address
    /// Only allows writes to RAM region (addresses >= ROM size)
    /// </summary>
    /// <param name="address">Memory address to write to</param>
    /// <param name="value">Byte value to write</param>
    public void WriteByte(ushort address, byte value)
    {
        if (address >= _romSize && address < _romSize + _ramSize)
        {
            _memory[address] = value;
        }
    }
}
