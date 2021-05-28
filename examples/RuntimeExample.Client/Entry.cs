using System;

namespace RuntimeExample.Client
{
    public static class Entry
    {
        [ModFramework.Modification(ModFramework.ModType.Runtime, "Client example")]
        public static void ClientExample()
        {
            On.Terraria.Main.Initialize += Main_Initialize;
        }

        private static void Main_Initialize(On.Terraria.Main.orig_Initialize orig, global::Terraria.Main self)
        {
            Console.WriteLine("[OTAPI] MonoMod reuntime hooks are active");
            orig(self);
        }
    }
}
