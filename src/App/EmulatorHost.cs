// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.Core;

namespace Zenix.App;

public class EmulatorHost
{
    private readonly Z80Cpu _cpu = new();
    private readonly MsxMemoryMap _memory = new();

    public void RunFrame()
    {
        _cpu.Step();
    }
}
