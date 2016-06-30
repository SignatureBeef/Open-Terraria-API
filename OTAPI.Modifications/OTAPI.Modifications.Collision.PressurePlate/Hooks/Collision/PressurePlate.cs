namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Collision
        {
            #region Handlers
            public delegate HookResult PressurePlateHandler
            (
                ref int x,
                ref int y,
                ref global::Terraria.Entity entity
            );
            #endregion

            /// <summary>
            /// Occurs when an entity triggers a pressure plate.
            /// </summary>
            public static PressurePlateHandler PressurePlate;
        }
    }
}
