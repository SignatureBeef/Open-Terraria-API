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
using System;
using System.IO;
using System.Linq;

namespace OTAPI.Patcher.Targets
{
    [MonoMod.MonoModIgnore]
    public class OTAPIMobileServerTarget : OTAPIPCServerTarget
    {
        public override string DisplayText { get; } = "OTAPI Mobile Server";
        public override string HtmlSearchKey { get; } = ">Mobile Dedicated Server";
        public override string NuGetPackageFileName { get; } = "OTAPI.Mobile.nupkg";
        public override string NuSpecFilePath { get; } = "../../../../OTAPI.Mobile.nuspec";
        public override string MdFileName { get; } = "OTAPI.Mobile.Server.mfw.md";
        public override string SupportedDownloadUrl { get; } = "https://terraria.org/server/MobileTerrariaServer.zip";

        public override string DetermineInputAssembly(string extractedFolder)
        {
            var zip = Directory.EnumerateFiles(extractedFolder, "Windows_MobileServer*.zip", SearchOption.AllDirectories).Single();
            extractedFolder = ExtractZip(zip);

            return Directory.EnumerateFiles(extractedFolder, "TerrariaServer.exe", SearchOption.AllDirectories).Single(x =>
                Path.GetFileName(Path.GetDirectoryName(x)).Equals("WindowsServer", StringComparison.CurrentCultureIgnoreCase)
            );
        }
    }
}
