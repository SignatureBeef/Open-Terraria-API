namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Npc
        {
            #region Handlers
            public delegate global::Terraria.NPC CreateHandler
            (
                ref int index,
                ref int x,
                ref int y,
                ref int type,
                ref int start,
                ref float ai0,
                ref float ai1,
                ref float ai2,
                ref float ai3,
                ref int target
            );
            #endregion
            
            public static CreateHandler Create;
        }
    }
}
