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

        static Logging.ProgressLogger _logger;

        public static void OnSaveWorldTiles_Progress(object[] args, int index)
        {
            if (null == _logger || index == 0)
            {
                if (_logger != null) _logger.Dispose();
                _logger = new OTA.Logging.ProgressLogger(100, Terraria.Lang.gen[49]);
            }

            float num = (float)index / (float)Terraria.Main.maxTilesX;
            _logger.Value = (int)(num * 100 + 1);
        }

        public static void OnValidateWorld_Progress(object[] args, int index)
        {
            if (null == _logger || index == 0)
            {
                if (_logger != null) _logger.Dispose();
                _logger = new OTA.Logging.ProgressLogger(100, Terraria.Lang.gen[73]);
            }

            float num = (float)index / (float)Terraria.Main.maxTilesX;
            _logger.Value = (int)(num * 100 + 1);
        }

        public static void OnSaveWorldDirect_StatusText(string text)
        {
            if (_logger != null) _logger.Dispose();

            if (!String.IsNullOrEmpty(text))
            {
                Logging.ProgramLog.Log(text);
            }
        }
    }
}