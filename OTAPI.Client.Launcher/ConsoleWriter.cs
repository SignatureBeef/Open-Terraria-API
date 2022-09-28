using System;
using System.IO;
using System.Threading.Tasks;

namespace OTAPI.Client.Launcher;

public class ConsoleWriter : StreamWriter
{
    public delegate void OnLineReceived(string line);
    public event OnLineReceived? LineReceived;

    public ConsoleWriter(Stream stream) : base(stream) { }

    void Log(string? value)
    {
        if (!String.IsNullOrWhiteSpace(value))
            LineReceived?.Invoke(value);
    }

    public override void Write(string? value)
    {
        Log(value);
        base.Write(value);
    }

    public override void WriteLine(string? value)
    {
        Log(value);
        base.WriteLine(value);
    }

    public override Task WriteAsync(string? value)
    {
        Log(value);
        return base.WriteAsync(value);
    }

    public override Task WriteLineAsync(string? value)
    {
        Log(value);
        return base.WriteLineAsync(value);
    }
}