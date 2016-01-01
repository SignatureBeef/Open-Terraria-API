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

        public static Terraria.Projectile GetProjectile(int index, 
                                                        float x, float y,
                                                        float speedX, float speedY,
                                                        int type, int damage, float knockBack, int owner, 
                                                        float ai0, float ai1)
        {
            var ctx = new HookContext();
            var args = new HookArgs.NewProjectile()
            {
                Projectile = Terraria.Main.projectile[index],
                Index = index,

                X = x,
                Y = y,
                SpeedX = speedX,
                SpeedY = speedY,
                Type = type,
                Damage = damage,
                KnockBack = knockBack,
                Owner = owner,
                ai0 = ai0,
                ai1 = ai1
            };
            HookPoints.NewProjectile.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.RECTIFY)
                return ctx.ResultParam as Terraria.Projectile;
            else return Terraria.Main.projectile[index];
        }
    }
}