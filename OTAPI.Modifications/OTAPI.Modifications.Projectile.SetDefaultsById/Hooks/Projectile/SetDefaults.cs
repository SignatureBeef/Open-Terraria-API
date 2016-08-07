namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Projectile
        {
            #region Handlers
            public delegate HookResult PreSetDefaultsByIdHandler(global::Terraria.Projectile projectile, ref int type);
            public delegate void PostSetDefaultsByIdHandler(global::Terraria.Projectile projectile, int type);
            #endregion

            /// <summary>
            /// Occurs at the start of the SetDefaults(int) method.
            /// Arg 1:  The projectile instance
            ///     2:  Type
            /// </summary>
            public static PreSetDefaultsByIdHandler PreSetDefaultsById;

            /// <summary>
            /// Occurs when the SetDefaults(int) method ends
            /// Arg 1:  The projectile instance
            ///     2:  Type
            /// </summary>
            public static PostSetDefaultsByIdHandler PostSetDefaultsById;
        }
    }
}
