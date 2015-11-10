using System;

namespace OTA.Logging
{
    public class StandardOutputTarget : InteractiveLogTarget
    {
        public StandardOutputTarget () : base ("Log1", Console.Out)
        {
        }

        protected override void SetColor (ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
        }

        protected override void ResetColor ()
        {
            System.Console.ResetColor ();
        }
    }
}

