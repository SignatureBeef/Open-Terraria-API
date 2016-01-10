using System;

namespace OTA.Patcher
{
    /// <summary>
    /// Provides additional functionality for use with the console
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Blanks out the current line based on the width of the Console.
        /// </summary>
        public static void ClearLine()
        {
            var current = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, current);
        }
    }
}

