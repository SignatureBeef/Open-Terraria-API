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
//using Mono.Cecil;
//using MonoMod;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace OTAPI.Patcher
//{
//    public class ResourceAssemblyResolver : DefaultAssemblyResolver
//    {
//        protected MonoModder Modder { get; set; }

//        public ResourceAssemblyResolver(MonoModder modder)
//        {
//            this.Modder = modder;
//            this.AddResourceAssemblies();
//        }

//        public void AddResourceAssemblies()
//        {
//            var extractionFolder = Path.Combine(Path.GetDirectoryName(this.Modder.InputPath), "EmbeddedResources");
//            var def = AssemblyDefinition.ReadAssembly(this.Modder.InputPath);
//            this.RegisterAssembly(def);

//            Directory.CreateDirectory(extractionFolder);

//            foreach (var module in def.Modules)
//            {
//                if (module.HasResources)
//                {
//                    foreach (var resource in module.Resources)
//                    {
//                        if (resource.ResourceType == ResourceType.Embedded)
//                        {
//                            var er = resource as EmbeddedResource;
//                            var data = er.GetResourceData();

//                            if (data.Length > 2)
//                            {
//                                bool is_pe = data.Take(2).SequenceEqual(new byte[] { 77, 90 }); // MZ
//                                if (is_pe)
//                                {
//                                    var ms = new MemoryStream(data);
//                                    var asm = AssemblyDefinition.ReadAssembly(ms);
//                                    this.RegisterAssembly(asm);

//                                    File.WriteAllBytes(Path.Combine(extractionFolder, er.Name), data);
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        //private Dictionary<string, AssemblyDefinition> AssemblyRedirectors = new Dictionary<string, AssemblyDefinition>();

//        //public void AddAssemblyRedirection(string assemblyNamePattern, string targetAssemblyName)
//        //{
//        //    var assembly = this.Modder.DependencyMap.Keys.Single(x => x.Name == targetAssemblyName).Assembly;
//        //    AssemblyRedirectors.Add(assemblyNamePattern, assembly);
//        //}

//        ///// <summary>
//        ///// Resolves a referenced within the source assembly, typically because of types being redirected
//        ///// </summary>
//        ///// <returns></returns>
//        //public AssemblyDefinition ResolveInternalReference(AssemblyNameReference name)
//        //{
//        //    //var type = this.Modder.FindType(name.FullName);
//        //    //var matches = this.Modder.Module.Types.Where(x => x.Namespace == name.Name).ToArray();

//        //    foreach(var redirector in AssemblyRedirectors)
//        //    {
//        //        if(redirector.Key.EndsWith("*"))
//        //        {
//        //            if(name.Name.StartsWith(redirector.Key.Substring(0, redirector.Key.Length - 1)))
//        //            {
//        //                return redirector.Value;
//        //            }
//        //        }
//        //        else if(redirector.Key == name.Name)
//        //        {
//        //            return redirector.Value;
//        //        }
//        //    }

//        //    //if(name.Name.StartsWith("Microsoft.Xna.Framework"))
//        //    //{
//        //    //    //return this.Modder.Module.Assembly;
//        //    //    return this.Modder.DependencyMap.Keys.Single(x => x.Name == "TerrariaServer.OTAPI.Shims.mm.dll").Assembly;
//        //    //}

//        //    return null;
//        //}

//        //public override AssemblyDefinition Resolve(AssemblyNameReference name)
//        //{
//        //    try
//        //    {
//        //        return base.Resolve(name);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        var asm = ResolveInternalReference(name);
//        //        if (asm != null) return asm;

//        //        throw;
//        //    }
//        //}

//        //public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
//        //{
//        //    try
//        //    {
//        //        return base.Resolve(name, parameters);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        var asm = ResolveInternalReference(name);
//        //        if (asm != null) return asm;

//        //        throw;
//        //    }
//        //}
//    }
//}
