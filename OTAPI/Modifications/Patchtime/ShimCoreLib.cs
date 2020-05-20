using System.Linq;

namespace OTAPI.Modifications.Patchtime
{
    [Modification("Shimming System.Private.CoreLib")]
    [MonoMod.MonoModIgnore]
    class ShimCoreLib // temporary shim
    {
        public ShimCoreLib(MonoMod.MonoModder modder)
        {
            foreach (var reference in modder.Module.AssemblyReferences
                .Where(x => x.Name.StartsWith("System.Private.CoreLib")).ToArray())
            {
                reference.Name = modder.Module.TypeSystem.CoreLibrary.Name;
                reference.Version = new System.Version("4.0.0.0");
            }
        }
    }
}