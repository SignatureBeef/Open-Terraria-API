
using ModFramework;


namespace Microsoft.Xna.Framework
{
    [MonoMod.MonoModIgnore]
    static partial class Mods
    {
        [Modification(ModType.PrePatch, "Relinking XNA")]
        public static void RedirectAssembly(ModFwModder modder)
        {
            modder.RelinkAssembly("Microsoft.Xna.Framework");
            modder.RelinkAssembly("Microsoft.Xna.Framework.Game");
            modder.RelinkAssembly("Microsoft.Xna.Framework.Graphics");
            modder.RelinkAssembly("Microsoft.Xna.Framework.Xact");
        }
    }
}