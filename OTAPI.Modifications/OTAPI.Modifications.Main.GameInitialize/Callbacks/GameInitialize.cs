namespace OTAPI.Callbacks.Terraria
{
    internal static partial class Main
    {
        /// <summary>
        /// This method is injected to the beginning of the terraria Initialize method.
        /// </summary>
        internal static void InitializeBegin(global::Terraria.Main game) => Hooks.Game.PreInitialize?.Invoke();

        /// <summary>
        /// This method is injected into the end of the terraria Initialize method.
        /// </summary>
        internal static void InitializeEnd(global::Terraria.Main game) => Hooks.Game.PostInitialize?.Invoke();
    }
}
