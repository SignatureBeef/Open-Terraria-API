using System;
using OTA.Plugin;

namespace OTA.Callbacks
{
    public static class CollisionCallback
    {
        public static bool OnPressurePlateTriggered(Terraria.Entity sender, int x, int y)
        {
            var ctx = new HookContext();
            var args = new HookArgs.PressurePlateTriggered()
            {
                Sender = sender,
                X = x,
                Y = y 
            };

            HookPoints.PressurePlateTriggered.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }
    }
}

