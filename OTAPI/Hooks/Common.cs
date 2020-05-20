using Microsoft.Xna.Framework;
using System;

namespace OTAPI
{
    public delegate void HookHandler();
    public delegate HookResult HookResultHandler();

    public enum HookResult
    {
        Continue,
        Cancel
    }

    public static class Hooks
    {
        public static class Main
        {
            public delegate HookResult UpdateHandler(ref GameTime gameTime);

            public static UpdateHandler PreUpdate;
            public static UpdateHandler PostUpdate;

            public static HookResultHandler PreInitialize;
            public static HookResultHandler PostInitialize;

            public static HookResultHandler StartCommandThread;
        }
        public static class Program
        {
            public static HookResultHandler Launch;
        }
        public static class NPC
        {
            public delegate HookResult MechSpawnHandler(float x, float y, int type, int num, int num2, int num3, int i, Microsoft.Xna.Framework.Vector2 vector, float num6);

            public static MechSpawnHandler MechSpawn;
        }
        public static class Item
        {
            public delegate HookResult MechSpawnHandler(float x, float y, int type, int num, int num2, int num3, int i, Microsoft.Xna.Framework.Vector2 vector, float num6);

            public static MechSpawnHandler MechSpawn;
        }
    }
}
