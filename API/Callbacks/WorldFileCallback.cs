using System;
using OTA.Plugin;

#if Full_API
using Terraria;
#endif

namespace OTA.Callbacks
{
    public static class WorldFileCallback
    {
        private static string _savePath;
        public static readonly object SavePathLock = new object();

        public static string SavePath
        {
#if Full_API
            get { return _savePath ?? Main.worldPathName; }
#else
            get { return null; }
#endif
            set { _savePath = value; }
        }

        public static bool OnAutoSave()
        {
            var args = new HookArgs.WorldAutoSave();
            var ctx = HookContext.Empty;

            HookPoints.WorldAutoSave.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        /// <summary>
        /// Clears the world, and is a direct replacement of Terraria.WorldGen.clearWorld.
        /// </summary>
        public static void ClearWorld()
        {
            MainCallback.ResetTileArray();
#if Full_API
            WorldGen.clearWorld();
#endif
            GC.Collect();
        }

        public static bool OnWorldSaveBegin(bool useCloudSaving, bool resetTime = false)
        {
            var ctx = new HookContext();
            var args = new HookArgs.WorldSave()
            {
                State = MethodState.Begin,

                ResetTime = resetTime,
                UseCloudSaving = useCloudSaving
            };

            HookPoints.WorldSave.Invoke(ref ctx, ref args);

            return ctx.Result == HookResult.DEFAULT;
        }

        public static void OnWorldSaveEnd(bool useCloudSaving, bool resetTime = false)
        {
            var ctx = new HookContext();
            var args = new HookArgs.WorldSave()
            {
                State = MethodState.End,

                ResetTime = resetTime,
                UseCloudSaving = useCloudSaving
            };

            HookPoints.WorldSave.Invoke(ref ctx, ref args);
        }
    }
}