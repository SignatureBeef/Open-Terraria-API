namespace OTAPI.Core.Callbacks.Terraria
{
    internal static partial class Main
    {
        /// <summary>
        /// This method is injected to the beginning of the terraria Initialize method.
        /// </summary>
        internal static void InitializeBegin() => Hooks.Game.PreInitialize?.Invoke();

        /// <summary>
        /// This method is injected into the end of the terraria Initialize method.
        /// </summary>
        internal static void InitializeEnd() => Hooks.Game.PostInitialize?.Invoke();
    }
}
