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
using System.IO;
using System.Linq;

namespace OTAPI.Patcher.Targets
{
    [MonoMod.MonoModIgnore]
    public class TMLPCServerTarget : OTAPIPCServerTarget
    {
        public override string DisplayText { get; } = "TML PC Server";
        public override string NuGetPackageFileName { get; } = "OTAPI.TML.nupkg";
        public override string NuSpecFilePath { get; } = "../../../../docs/OTAPI.TML.nuspec";
        public override string MdFileName { get; } = "OTAPI.TML.PC.Server.mfw.md";
        public override string SupportedDownloadUrl { get; } = "https://github.com/tModLoader/tModLoader/releases/download/v0.11.8.4/tModLoader.Windows.v0.11.8.4.zip";
        public override string ArtifactName { get; } = "artifact-tml";

        public override string DetermineInputAssembly(string extractedFolder)
            => Directory.EnumerateFiles(extractedFolder, "tModLoaderServer.exe", SearchOption.AllDirectories).Single();

        public override string GetZipUrl() => SupportedDownloadUrl;
    }
}