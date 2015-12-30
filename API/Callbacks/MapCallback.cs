#if CLIENT
using System;
using OTA.Plugin;

namespace OTA.Callbacks
{
    public static class MapCallback
    {
        public static bool OnMapHelperInitializeBegin()
        {
            var ctx = new HookContext();
            var args = new HookArgs.MapHelperInitialize()
            {
                State = MethodState.Begin
            };

            HookPoints.MapHelperInitialize.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnMapHelperInitializeEnd()
        {
            var ctx = new HookContext();
            var args = new HookArgs.MapHelperInitialize()
            {
                State = MethodState.End
            };

            HookPoints.MapHelperInitialize.Invoke(ref ctx, ref args);
        }
    }
}
#endif