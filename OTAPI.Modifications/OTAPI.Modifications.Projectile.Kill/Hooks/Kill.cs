namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Projectile
        {
            #region Handlers
            public delegate HookResult PreKillHandler(global::Terraria.Projectile projectile);
            public delegate void PostKilledHandler(global::Terraria.Projectile projectile);
            #endregion
			
            public static PreKillHandler PreKill;
			
            public static PostKilledHandler PostKilled;
        }
    }
}
