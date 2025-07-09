// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Core;

public class MsxMemoryMap
{
    private readonly byte[] _memory = new byte[0x10000];
    private int _romSize;
    private int _ramSize;

    public void Configure(int romSize, int ramSize)
    {
        _romSize = Math.Clamp(romSize, 0, 0x10000);
        _ramSize = Math.Clamp(ramSize, 0, 0x10000 - _romSize);
    }

    public void LoadRom(byte[] data)
    {
        var length = Math.Min(data.Length, _memory.Length);
        Array.Clear(_memory, 0, _memory.Length);
        Array.Copy(data, 0, _memory, 0, length);
        _romSize = length;
    }

    public byte ReadByte(ushort address) => _memory[address];

    public void WriteByte(ushort address, byte value)
    {
        if (address >= _romSize && address < _romSize + _ramSize)
        {
            _memory[address] = value;
        }
    }
}
