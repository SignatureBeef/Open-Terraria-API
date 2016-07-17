namespace OTAPI.Core
{
    public static partial class Hooks
    {
        public static partial class Net
        {
            public static partial class RemoteClient
            {
                #region Handlers
                public delegate HookResult PreResetHandler(global::Terraria.RemoteClient remoteClient);
                public delegate void PostResetHandler(global::Terraria.RemoteClient remoteClient);
                #endregion
				
                public static PreResetHandler PreReset;
                public static PostResetHandler PostReset;
			}
        }
    }
}
