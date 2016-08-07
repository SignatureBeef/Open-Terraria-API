namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Projectile
        {
            #region Handlers
            public delegate HookResult PreUpdateHandler(global::Terraria.Projectile projectile, ref int index);
            public delegate void PostUpdateHandler(global::Terraria.Projectile projectile, int index);
            #endregion

            /// <summary>
            /// Occurs at the start of the Update(int) method.
            /// Arg 1:  The projectile instance
            ///     2:  The root of whoAmI
            /// </summary>
            public static PreUpdateHandler PreUpdate;

            /// <summary>
            /// Occurs when the Update(int) method ends
            /// Arg 1:  The projectile instance
            ///     2:  The root of whoAmI
            /// </summary>
            public static PostUpdateHandler PostUpdate;
        }
    }
}
