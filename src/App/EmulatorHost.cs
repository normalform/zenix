// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.Core;

namespace Zenix.App;

public class EmulatorHost
{
    private readonly MsxMemoryMap _memory = new();
    private readonly Z80Cpu _cpu;

    public EmulatorHost()
    {
        _cpu = new Z80Cpu(_memory);
    }

    public void RunFrame()
    {
        _cpu.Step();
    }
}
