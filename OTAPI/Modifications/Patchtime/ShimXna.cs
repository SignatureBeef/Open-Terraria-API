using System.Linq;

namespace OTAPI.Modifications.Patchtime
{
    [Modification("Shimming Xna")]
    [MonoMod.MonoModIgnore]
    class PatchCoreLib
    {
        public PatchCoreLib(MonoMod.MonoModder modder)
        {
            foreach (var reference in modder.Module.AssemblyReferences
                .Where(x => x.Name.StartsWith("Microsoft.Xna.Framework")).ToArray())
            {
                var target = typeof(Microsoft.Xna.Framework.Game).Assembly.GetName();
                reference.Name = target.Name;
                reference.PublicKey = target.GetPublicKey();
                reference.PublicKeyToken = target.GetPublicKeyToken();
                reference.Version = target.Version;
            }
        }
    }
}