
using ModFramework;

namespace Microsoft.Win32
{
    [MonoMod.MonoModIgnore]
    static partial class Mods
    {
        [Modification(ModType.PrePatch, "Relinking Microsoft.Win32.Registry")]
        public static void RedirectAssembly(ModFwModder modder)
        {
            modder.RelinkAssembly("Microsoft.Win32.Registry");
        }
    }
}