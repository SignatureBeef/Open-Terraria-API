using System;

namespace OTAPI.Client.Launcher;

public class Plugin
{
    public string Name { get; set; }

    private bool _enabled = false;
    public bool IsEnabled
    {
        get => _enabled;
        set
        {
            var changing = value != _enabled;
            _enabled = value;

            if (changing) OnEnabledChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public string Path { get; set; }

    public event EventHandler? OnEnabledChanged;

    public Plugin(string name, bool isEnabled, string path)
    {
        Name = name;
        _enabled = isEnabled;
        Path = path;
    }
}
