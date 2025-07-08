// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.App;

namespace Zenix.CLI;

public static class Program
{
    public static void Main(string[] args)
    {
        var host = new EmulatorHost();
        host.RunFrame();
        System.Console.WriteLine("Zenix CLI skeleton running.");
    }
}
