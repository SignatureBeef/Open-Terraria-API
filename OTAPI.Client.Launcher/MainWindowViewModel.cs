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
using System;
using System.IO;

namespace OTAPI.Client.Launcher
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string? _vanillaExe;
        public string? VanillaExe
        {
            get => _vanillaExe;
            set
            {
                IsVanillaReady = File.Exists(value);
                this.RaiseAndSetIfChanged(ref _vanillaExe, value);
            }
        }

        private string? _otapiExe;
        public string? OtapiExe
        {
            get => _otapiExe;
            set
            {
                IsOTAPIFound = File.Exists(value);
                IsOTAPIReady = IsOTAPIFound && !IsInstalling;
                this.RaiseAndSetIfChanged(ref _otapiExe, value);
            }
        }

        private bool _isVanillaReady;
        public bool IsVanillaReady { get => _isVanillaReady; set => this.RaiseAndSetIfChanged(ref _isVanillaReady, value); }

        private bool _isOTAPIReady;
        public bool IsOTAPIReady
        {
            get => _isOTAPIReady;
            set
            {
                this.RaiseAndSetIfChanged(ref _isOTAPIReady, value);
            }
        }

        private bool _isOTAPIFound;
        public bool IsOTAPIFound
        {
            get => _isOTAPIFound;
            set
            {
                this.RaiseAndSetIfChanged(ref _isOTAPIFound, value);
            }
        }

        public IPlatformTarget? LaunchTarget { get; set; }

        private ClientInstallPath<IPlatformTarget> _installPath;
        public ClientInstallPath<IPlatformTarget> InstallPath { get => _installPath; set => this.RaiseAndSetIfChanged(ref _installPath, value); }

        private bool _installPathValid;
        public bool InstallPathValid
        {
            get => _installPathValid;
            set
            {
                this.RaiseAndSetIfChanged(ref _installPathValid, value);
                CanInstall = InstallPathValid && !IsInstalling;
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
                CanInstall = InstallPathValid && !IsInstalling;
                IsOTAPIReady = File.Exists(OtapiExe) && !IsInstalling;
            }
        }

        private bool _canInstall;
        public bool CanInstall { get => _canInstall; set => this.RaiseAndSetIfChanged(ref _canInstall, value); }
    }
}
