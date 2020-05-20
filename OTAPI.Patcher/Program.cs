using Mono.Cecil;
using MonoMod;
using OTAPI.Common;
using System;
using System.IO;
using System.Linq;

namespace OTAPI.Patcher
{
    public class ResourceAssemblyResolver : DefaultAssemblyResolver
    {
        protected MonoModder Modder { get; set; }

        public ResourceAssemblyResolver(MonoModder modder)
        {
            this.Modder = modder;
            this.AddResourceAssemblies();
        }

        public void AddResourceAssemblies()
        {
            var def = AssemblyDefinition.ReadAssembly(this.Modder.InputPath);
            this.RegisterAssembly(def);

            foreach (var module in def.Modules)
            {
                if (module.HasResources)
                {
                    foreach (var resource in module.Resources)
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
                                    this.RegisterAssembly(asm);
                                }
                            }
                        }
                    }
                }
            }
        }

        ///// <summary>
        ///// Resolves a referenced within the source assembly, typically because of types being redirected
        ///// </summary>
        ///// <returns></returns>
        //public AssemblyDefinition ResolveInternalReference(AssemblyNameReference name)
        //{
        //    //var type = this.Modder.FindType(name.FullName);
        //    var matches = this.Modder.Module.Types.Where(x => x.Namespace == name.Name).ToArray();

        //    return null;
        //}

        //public override AssemblyDefinition Resolve(AssemblyNameReference name)
        //{
        //    try
        //    {
        //        return base.Resolve(name);
        //    }
        //    catch (Exception ex)
        //    {
        //        var asm = ResolveInternalReference(name);
        //        if (asm != null) return asm;

        //        throw;
        //    }
        //}

        //public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        //{
        //    try
        //    {
        //        return base.Resolve(name, parameters);
        //    }
        //    catch (Exception ex)
        //    {
        //        var asm = ResolveInternalReference(name);
        //        if (asm != null) return asm;

        //        throw;
        //    }
        //}
    }

    public class OTAPIModder : MonoModder
    {
        public override void PatchRefs()
        {
            base.PatchRefs();


        }
    }

    static class Program
    {
        public static void Main(string[] args)
        {
            var input = Remote.DownloadServer();

            using (var mm = new OTAPIModder()
            {
                InputPath = input,
                //OutputPath = "OTAPI.dll",
                OutputPath = "TerrariaServer.dll",
                MissingDependencyThrow = false,
                //LogVerboseEnabled = true,
            })
            {
                mm.AssemblyResolver = new ResourceAssemblyResolver(mm);

                mm.Read();

                mm.Log("[Main] Scanning for mods in directory.");
                mm.ReadMod(Environment.CurrentDirectory);

                mm.MapDependencies();

                mm.Log("[Main] mm.AutoPatch();");
                mm.AutoPatch();

                //mm.Write();

                new OTAPI.Modifications.Modifier().Apply("OTAPI.Modifications.Patchtime", modder: mm);

                mm.Write();

                mm.Log("[Main] Done.");
            }
        }
    }
}
