using System;

namespace OTAPI.Callbacks.Terraria
{
    internal static partial class Main
    {
        /// <summary>
        /// Called from Terraria.Main.DedServ to write pending console data.
        /// </summary>
        internal static void UpdateStatusText()
        {
            var result = Hooks.Game.StatusTextUpdate?.Invoke();
            if (result.HasValue && result.Value == HookResult.Cancel)
                return;

            if (global::Terraria.Main.oldStatusText != global::Terraria.Main.statusText)
            {
                global::Terraria.Main.oldStatusText = global::Terraria.Main.statusText;
                Console.WriteLine(global::Terraria.Main.statusText);
            }
        }

        /// <summary>
        /// Called from vanilla as a replacement to all Terraria.Main.statusText writes.
        /// This allows us to capture exactly what data was going to be written
        /// to the statusText variable.
        /// This can be used to write all lines instead of overwriting data, but that's
        /// not what OTAPI is for.
        /// </summary>
        /// <param name="text"></param>
        internal static void SetStatusText(string text)
        {
            var result = Hooks.Game.StatusTextWrite?.Invoke(ref text);
            if (result.HasValue && result.Value == HookResult.Cancel)
                return;

            global::Terraria.Main.statusText = text;
        }
    }
}
