// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.App;
using Demos;

namespace Zenix.CLI;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "cycles")
        {
            CycleCountingDemo.RunDemo();
        }
        else
        {
            var host = new EmulatorHost();
            host.Step();
            System.Console.WriteLine("Zenix CLI skeleton running.");
            System.Console.WriteLine();
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine("  dotnet run cycles    - Run cycle counting demonstration");
        }
    }
}
