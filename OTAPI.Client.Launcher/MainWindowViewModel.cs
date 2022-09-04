/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using OTAPI.Client.Launcher.Targets;
using OTAPI.Common;
using ReactiveUI;
using System.IO;

namespace OTAPI.Client.Launcher;

public class MainWindowViewModel : ReactiveObject
{
    public string VersionText => $"Open Terraria API {RemovePaddedVersion(typeof(OTAPI.Patcher.Common)!.Assembly!.GetName()!.Version!.ToString())}";
    static string RemovePaddedVersion(string str)
    {
        while(str.EndsWith(".0"))
            str = str.Remove(str.Length - 2, 2);
        return str;
    }

    private string? _vanillaExe;
    public string? VanillaExe
    {
        get => _vanillaExe;
        set
        {
            this.RaiseAndSetIfChanged(ref _vanillaExe, value);
            this.RaisePropertyChanged(nameof(IsVanillaFound));
            this.RaisePropertyChanged(nameof(IsVanillaReady));
        }
    }

    private string? _otapiExe;
    public string? OtapiExe
    {
        get => _otapiExe;
        set
        {
            this.RaiseAndSetIfChanged(ref _otapiExe, value);
            this.RaisePropertyChanged(nameof(IsOTAPIFound));
            this.RaisePropertyChanged(nameof(IsOTAPIReady));
        }
    }


    public bool IsVanillaFound => File.Exists(VanillaExe);
    public bool IsVanillaReady => IsVanillaFound && !IsInstalling;

    public bool IsOTAPIFound => File.Exists(OtapiExe);
    public bool IsOTAPIReady => IsOTAPIFound && !IsInstalling;

    public IPlatformTarget? LaunchTarget { get; set; }

    private ClientInstallPath<IPlatformTarget>? _installPath;
    public ClientInstallPath<IPlatformTarget>? InstallPath { get => _installPath; set => this.RaiseAndSetIfChanged(ref _installPath, value); }

    private bool _installPathValid;
    public bool InstallPathValid
    {
        get => _installPathValid;
        set
        {
            this.RaiseAndSetIfChanged(ref _installPathValid, value);
            this.RaisePropertyChanged(nameof(CanInstall));
        }
    }

    private string _installStatus = string.Empty;
    public string InstallStatus { get => _installStatus; set => this.RaiseAndSetIfChanged(ref _installStatus, value); }

    private bool _installing;
    public bool IsInstalling
    {
        get => _installing;
        set
        {
            this.RaiseAndSetIfChanged(ref _installing, value);
            this.RaisePropertyChanged(nameof(CanInstall));
            this.RaisePropertyChanged(nameof(IsOTAPIReady));
            this.RaisePropertyChanged(nameof(IsVanillaReady));
        }
    }

    public bool CanInstall => InstallPathValid && !IsInstalling;
}
