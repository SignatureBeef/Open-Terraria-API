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

namespace OTAPI.Patcher.Resolvers;

[MonoMod.MonoModIgnore]
public class TMLFileResolver : IFileResolver
{
    public virtual string SupportedDownloadUrl { get; } = "https://github.com/tModLoader/tModLoader/releases/download/v2022.07.58.3/tModLoader.zip";

    public virtual string AquireLatestBinaryUrl() => SupportedDownloadUrl;

    public virtual string DetermineInputAssembly(string extractedFolder)
            => Directory.EnumerateFiles(extractedFolder, "tModLoader.dll", SearchOption.TopDirectoryOnly).Single();
}
