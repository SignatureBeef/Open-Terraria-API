namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class World
        {
            public static partial class IO
            {
                #region Handlers
                public delegate HookResult PreSaveWorldHandler(ref bool useCloudSaving, ref bool resetTime);
                public delegate void PostSaveWorldHandler(bool useCloudSaving, bool resetTime);
                #endregion

                /// <summary>
                /// Occurs at the start of the saveWorld(bool,bool) method.
                /// Arg 1:  Flag to save to the "cloud"
                ///     2:  The root of whoAmIFlag to indicate if the time is to be reset
                /// </summary>
                public static PreSaveWorldHandler PreSaveWorld;

                /// <summary>
                /// Occurs when the saveWorld(bool,bool) method ends
                /// Arg 1:  Flag to save to the "cloud"
                ///     2:  The root of whoAmIFlag to indicate if the time is to be reset
                /// </summary>
                public static PostSaveWorldHandler PostSaveWorld;
            }
        }
    }
}
