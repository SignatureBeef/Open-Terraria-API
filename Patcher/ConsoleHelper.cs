using System;

namespace OTA.Patcher
{
    /// <summary>
    /// Provides additional functionality for use with the console
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Blanks out the current line based on the width of the console.
        /// </summary>
        public static void ClearLine()
        {
            var current = System.Console.CursorTop;
            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(0, current);
        }
    }
}

