namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Main
    {
        /// <summary>
        /// This method is injected to the beginning of the terraria Initialize method.
        /// </summary>
        internal static void InitializeBegin()
        {
            if (Hooks.Game.PreInitialize != null)
                Hooks.Game.PreInitialize();
        }

        /// <summary>
        /// This method is injected into the end of the terraria Initialize method.
        /// </summary>
        internal static void InitializeEnd()
        {
            if (Hooks.Game.PostInitialize != null)
                Hooks.Game.PostInitialize();
        }
    }
}
