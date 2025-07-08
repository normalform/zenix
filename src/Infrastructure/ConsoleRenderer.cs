namespace Zenix.Infrastructure;

public class ConsoleRenderer
{
    public void DrawFrame(byte[] frameBuffer)
    {
        System.Console.WriteLine($"Frame bytes: {frameBuffer.Length}");
    }
}
