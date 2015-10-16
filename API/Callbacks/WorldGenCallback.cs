using System;
using OTA.Plugin;

namespace OTA.Callbacks
{
    public static class WorldGenCallback
    {
        public static bool OnHardModeTileUpdate()
        {
//            Logging.ProgramLog.Admin.Log("Hard mode tile called");
            return true;
        }

        public static bool OnStartHardMode()
        {
            var ctx = new HookContext();
            var args = new HookArgs.StartHardMode();

            HookPoints.StartHardMode.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }
    }
}

