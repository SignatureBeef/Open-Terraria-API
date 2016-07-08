namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate void TransformHandler(global::Terraria.NPC npc);
            #endregion

            /// <summary>
            /// Occurs when an npc transforms
            /// Arg 1:  The npc instance=
            /// </summary>
            public static TransformHandler Transform;
        }
    }
}
