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
#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

#if !tModLoader_V1_4 && (Terraria_1442_OrAbove || TerrariaServer_1442_OrAbove)

/// <summary>
/// @doc Fixes ReLogic.OS.Platform.Current using RuntimeInformation
/// </summary>
namespace ReLogic.OS.Windows
{
    public class patch_PathService : ReLogic.OS.Windows.PathService
    {
        public extern bool orig_MoveToRecycleBin(string path);
        public override bool MoveToRecycleBin(string path)
        {
            System.IO.File.Delete(path);
            return true;
        }
    }
}
#endif