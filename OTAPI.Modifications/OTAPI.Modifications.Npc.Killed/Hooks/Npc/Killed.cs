namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate void NpcHandler(Terraria.NPC npc);
            #endregion

            /// <summary>
            /// Occurs when an npc has been killed
            /// </summary>
            public static NpcHandler Killed;
        }
    }
}
