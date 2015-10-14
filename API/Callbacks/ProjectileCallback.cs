using System;
using OTA.Plugin;

namespace OTA.Callbacks
{
    public static class ProjectileCallback
    {
        public static int OnSetDefaultsBegin(Terraria.Projectile projectle, int type)
        {
            var ctx = new HookContext();
            var args = new HookArgs.ProjectileSetDefaults()
            {
                Projectile = projectle,
                Type = type,

                State = MethodState.Begin
            };

            HookPoints.ProjectileSetDefaults.Invoke(ref ctx, ref args);

            type = args.Type;
            if (ctx.Result == HookResult.RECTIFY && ctx.ResultParam is Int32)
                type = (int)ctx.ResultParam;

            return type;
        }

        public static void OnSetDefaultsEnd(Terraria.Projectile projectle, int type)
        {
            var ctx = new HookContext();
            var args = new HookArgs.ProjectileSetDefaults()
            {
                Projectile = projectle,
                Type = type,

                State = MethodState.End
            };
            HookPoints.ProjectileSetDefaults.Invoke(ref ctx, ref args);
        }

        public static void OnAIBegin(Terraria.Projectile projectle)
        {
            var ctx = new HookContext();
            var args = new HookArgs.ProjectileAI()
            {
                Projectile = projectle,

                State = MethodState.Begin
            };
            HookPoints.ProjectileAI.Invoke(ref ctx, ref args);
        }

        public static void OnAIEnd(Terraria.Projectile projectle)
        {
            var ctx = new HookContext();
            var args = new HookArgs.ProjectileAI()
            {
                Projectile = projectle,

                State = MethodState.End
            };
            HookPoints.ProjectileAI.Invoke(ref ctx, ref args);
        }
    }
}