using System.Text.Json;
using Zenix.Core;
using Zenix.Tools.Z80DebugServer;

var memory = new Z80MemoryMap();
var cpu = new Z80Cpu(memory, new Zenix.Core.Interrupt.Z80Interrupt());
var server = new DebugServer(cpu, memory);

await server.RunAsync(Console.OpenStandardInput(), Console.OpenStandardOutput(), CancellationToken.None);
