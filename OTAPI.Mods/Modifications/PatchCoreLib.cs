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
using MonoMod;
using System.Linq;

namespace OTAPI.Modifications
{
    [Modification(ModType.PostProcess, "Removing hard references to System.Private.CoreLib")]
    [MonoMod.MonoModIgnore]
    class PatchCoreLib
    {
        public PatchCoreLib(MonoModder modder)
        {
            foreach (var reference in modder.Module.AssemblyReferences
                .Where(x => x.Name.StartsWith("mscorlib") || x.Name.StartsWith("System.Private.CoreLib"))
                .ToArray()
            )
            {
                var asm = modder.Module.TypeSystem.CoreLibrary as Mono.Cecil.AssemblyNameReference;
                reference.Name = asm.Name;
                reference.Version = asm.Version;
                reference.PublicKey = asm.PublicKey;
                reference.PublicKeyToken = asm.PublicKeyToken;
            }
        }
    }
}
