namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class World
        {
            public static partial class IO
            {
                #region Handlers
                public delegate HookResult PreLoadWorldHandler(ref bool loadFromCloud);
                public delegate void PostLoadWorldHandler(bool loadFromCloud);
                #endregion

                public static PreLoadWorldHandler PreLoadWorld;

                public static PostLoadWorldHandler PostLoadWorld;
            }
        }
    }
}
