using System;
using System.IO;
using System.Threading.Tasks;

namespace OTAPI.Client.Launcher;

public class ConsoleWriter : StreamWriter
{
    MainWindowViewModel Context { get; set; }

    public ConsoleWriter(MainWindowViewModel context, Stream stream) : base(stream)
    {
        Context = context;
    }

    void Log(string? value)
    {
        if (!String.IsNullOrWhiteSpace(value))
            Context.Console.Insert(0, $"[{DateTime.Now:yyyyMMdd HH:mm:ss}] {value}");
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