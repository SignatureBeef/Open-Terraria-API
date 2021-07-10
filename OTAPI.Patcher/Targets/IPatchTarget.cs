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

using ModFramework;
using System.IO;

namespace OTAPI.Patcher.Targets
{
    [MonoMod.MonoModIgnore]
    public interface IPatchTarget
    {
        string DisplayText { get; }
        void Patch();
    }

    public static partial class Common
    {
        public static void AddPatchMetadata(this IPatchTarget target, ModFwModder modder,
            string initialModuleName = null,
            string inputName = null)
        {
            modder.AddMetadata("OTAPI.Target", target.DisplayText);
            if (initialModuleName != null) modder.AddMetadata("OTAPI.ModuleName", initialModuleName);
            if (inputName != null) modder.AddMetadata("OTAPI.Input", inputName);
        }

        public static void WriteCIArtifacts(this IPatchTarget target, string outputFolder)
        {
            if (Directory.Exists(outputFolder)) Directory.Delete(outputFolder, true);
            Directory.CreateDirectory(outputFolder);

            File.Copy("../../../../COPYING.txt", Path.Combine(outputFolder, "COPYING.txt"));
            File.Copy("OTAPI.dll", Path.Combine(outputFolder, "OTAPI.dll"));
            File.Copy("OTAPI.Runtime.dll", Path.Combine(outputFolder, "OTAPI.Runtime.dll"));
        }
    }
}
