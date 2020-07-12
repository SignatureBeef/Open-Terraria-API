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
using Mono.Cecil;
using System.IO;
using System.Linq;

namespace ModFramework
{
    [MonoMod.MonoModIgnore]
    public class ResourceExtractor
    {
        public string Extract(string inputFile)
        {
            var extractionFolder = Path.Combine(Path.GetDirectoryName(inputFile), "EmbeddedResources");
            using (var asmms = new MemoryStream(File.ReadAllBytes(inputFile)))
            {
                var def = AssemblyDefinition.ReadAssembly(asmms);

                if (Directory.Exists(extractionFolder)) Directory.Delete(extractionFolder, true);
                Directory.CreateDirectory(extractionFolder);

                foreach (var module in def.Modules)
                {
                    if (module.HasResources)
                    {
                        foreach (var resource in module.Resources.ToArray())
                        {
                            if (resource.ResourceType == ResourceType.Embedded)
                            {
                                var er = resource as EmbeddedResource;
                                var data = er.GetResourceData();

                                if (data.Length > 2)
                                {
                                    bool is_pe = data.Take(2).SequenceEqual(new byte[] { 77, 90 }); // MZ
                                    if (is_pe)
                                    {
                                        var ms = new MemoryStream(data);
                                        var asm = AssemblyDefinition.ReadAssembly(ms);

                                        File.WriteAllBytes(Path.Combine(extractionFolder, $"{asm.Name.Name}.dll"), data);
                                        module.Resources.Remove(resource);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return extractionFolder;
        }
    }
}
