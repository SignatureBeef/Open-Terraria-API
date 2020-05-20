#pragma warning disable CS0626
using Microsoft.Xna.Framework;
using OTAPI;
using System;

namespace Terraria
{
    class patch_Main //: Terraria.Main
    {
        protected extern void orig_Update(GameTime gameTime);
        protected void Update(GameTime gameTime)
        {
            if (Hooks.Main.PreUpdate == null || Hooks.Main.PreUpdate(ref gameTime) == HookResult.Continue)
            {
                orig_Update(gameTime);
                Hooks.Main.PostUpdate?.Invoke(ref gameTime);
            }
        }

        protected extern void orig_Initialize();
        protected void Initialize()
        {
            new OTAPI.Modifications.Modifier().Apply("OTAPI.Modifications.Runtime");

            if (Hooks.Main.PreInitialize == null || Hooks.Main.PreInitialize() == HookResult.Continue)
            {
                Console.WriteLine($"PreInitialize");
                orig_Initialize();
                Hooks.Main.PostInitialize?.Invoke();
            }
        }
        public static extern void orig_startDedInput();
        public static void startDedInput()
        {
            if (Hooks.Main.StartCommandThread == null || Hooks.Main.StartCommandThread() == HookResult.Continue)
            {
                orig_startDedInput();
            }
        }
    }
}