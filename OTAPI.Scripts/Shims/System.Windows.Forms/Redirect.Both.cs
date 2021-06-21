
using ModFramework;

namespace System.Windows.Forms
{
    [MonoMod.MonoModIgnore]
    static partial class Mods
    {
        [Modification(ModType.PrePatch, "Relinking System.Windows.Forms")]
        public static void RedirectAssembly(ModFwModder modder)
        {
            modder.RelinkAssembly("System.Windows.Forms");
        }
    }
}