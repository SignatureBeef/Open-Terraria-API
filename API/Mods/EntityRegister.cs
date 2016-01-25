using System;
using System.Collections.Concurrent;
using OTA.Mod.Npc;
using System.Linq;
using System.Reflection;
#if CLIENT
using OTA.Mod.Chest;
#endif
using OTA.Mod.Item;
using OTA.Mod.Tile;
using OTA.Mod.Projectile;
using OTA.Logging;

namespace OTA.Mod
{
    public static class EntityRegistrar
    {
        public static ItemModRegister Items { get; } = new ItemModRegister();

        public static NpcModRegister Npcs { get; } = new NpcModRegister();

        #if CLIENT
        public static ShopModRegister Shops { get; } = new ShopModRegister();
        #endif

        public static TileModRegister Tiles { get; } = new TileModRegister();

        public static ProjectileModRegister Projectiles { get; } = new ProjectileModRegister();

        internal static void ScanAssembly(Assembly asm)
        {
            DebugFramework.Assert.Expression(() => asm == null);

            Logger.Debug($"Scanning assemby {asm.FullName}");

            //Look for INativeMod
            var nm = typeof(NativeModAttribute);

            var npc = typeof(OTANpc);
            var item = typeof(OTAItem);
            var tile = typeof(OTATile);
            var projectile = typeof(OTAProjectile);

            var npcRegister = typeof(NpcModRegister).GetMethod("Register");
            var itemRegister = typeof(ItemModRegister).GetMethod("Register");
            var tileRegister = typeof(TileModRegister).GetMethod("Register");
            var projectileRegister = typeof(ProjectileModRegister).GetMethod("Register");

            if (null != asm.ExportedTypes)
                foreach (var nativeMod in asm.ExportedTypes.Where(x => Attribute.IsDefined(x, nm)))
                {
                    var attr = (NativeModAttribute)Attribute.GetCustomAttribute(nativeMod, nm);

                    Logger.Debug($"Flagged class {nativeMod.Name}");
                    if (npc.IsAssignableFrom(nativeMod))
                    {
                        Logger.Debug($"Detected custom NPC {nativeMod.Name}");
                        npcRegister.MakeGenericMethod(nativeMod).Invoke(Npcs, new object[] { attr.EntityName });
                    }
                    else if (item.IsAssignableFrom(nativeMod))
                    {
                        Logger.Debug($"Detected custom ITEM {nativeMod.Name}");
                        itemRegister.MakeGenericMethod(nativeMod).Invoke(Items, new object[] { attr.EntityName });
                    }
                    else if (tile.IsAssignableFrom(nativeMod))
                    {
                        Logger.Debug($"Detected custom TILE {nativeMod.Name}");
                        tileRegister.MakeGenericMethod(nativeMod).Invoke(Tiles, new object[] { attr.EntityName });
                    }
                    else if (projectile.IsAssignableFrom(nativeMod))
                    {
                        Logger.Debug($"Detected custom PROJECTILE {nativeMod.Name}");
                        projectileRegister.MakeGenericMethod(nativeMod).Invoke(Projectiles, new object[] { attr.EntityName });
                    }
                }
        }
    }
}