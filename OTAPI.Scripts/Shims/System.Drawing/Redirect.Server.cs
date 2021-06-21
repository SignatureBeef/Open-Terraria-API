
using ModFramework;

namespace System.Drawing
{
    [MonoMod.MonoModIgnore]
    static partial class Mods
    {
        [Modification(ModType.PrePatch, "Relinking System.Drawing.Graphics")]
        public static void RedirectAssembly(ModFwModder modder)
        {
            modder.RelinkAssembly("System.Drawing.Graphics");
        }
    }
}