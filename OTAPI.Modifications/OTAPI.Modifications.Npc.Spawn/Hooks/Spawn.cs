namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate HookResult SpawnHandler(ref int index);
            #endregion

            /// <summary>
            /// Occurs when an npc is about to be spawned into the world.
            /// </summary>
            public static SpawnHandler Spawn;
        }
    }
}
