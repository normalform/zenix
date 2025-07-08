// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Zenix.Infrastructure;

public class ConsoleRenderer
{
    public void DrawFrame(byte[] frameBuffer)
    {
        System.Console.WriteLine($"Frame bytes: {frameBuffer.Length}");
    }
}
