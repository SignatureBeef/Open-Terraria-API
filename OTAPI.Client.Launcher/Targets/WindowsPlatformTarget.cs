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
using OTAPI.Common;
using System;
using System.IO;

namespace OTAPI.Client.Launcher.Targets;

public class WindowsPlatformTarget : WindowsInstallDiscoverer, IPlatformTarget
{
    public void OnUILoad(MainWindowViewModel vm)
    {
        vm.OtapiExe = Path.Combine(Environment.CurrentDirectory, "client", "OTAPI.exe");
        vm.VanillaExe = (vm.InstallPath?.Path is not null && Directory.Exists(vm.InstallPath.Path)) ? Path.Combine(vm.InstallPath.Path, "Terraria.exe") : null;
    }

    public void Install(string installPath)
    {
        var sourceContentPath = Path.Combine(installPath, "Content");
        var clientPath = Path.Combine(Environment.CurrentDirectory, "client");
#if USE_BIN_FOLDER
        var destContentPath = Path.Combine(Environment.CurrentDirectory, "bin", "Content");
#else
        var destContentPath = Path.Combine(clientPath, "Content");
#endif

        Console.WriteLine(Status = "Installing FNA libs...");
        this.InstallLibs(clientPath);

        Console.WriteLine(Status = "Installing Steamworks...");
        this.InstallSteamworks64(clientPath, installPath);

        Console.WriteLine(Status = "Linking Terraria Content files...");
        if (!Directory.Exists(destContentPath))
            Directory.CreateSymbolicLink(destContentPath, sourceContentPath);
    }
}
