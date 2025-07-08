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
