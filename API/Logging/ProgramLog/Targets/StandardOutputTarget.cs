using System;
using OTA.Plugin;

namespace OTA.Logging
{
    /// <summary>
    /// StandardOutputTarget is used to capture the ProgramLog messages and to forward them into the Console window.
    /// The class inherits the <see cref="InteractiveLogTarget"/> so each instance will have a thread created.
    /// </summary>
    public class StandardOutputTarget : InteractiveLogTarget
    {
        public StandardOutputTarget() : base("Log1", Console.Out)
        {
        }

        protected override void SetColor(ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
        }

        protected override void ResetColor()
        {
            System.Console.ResetColor();
        }

        protected override void OnMessageReceived(string text)
        {
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };

            var args = new HookArgs.ConsoleMessageReceived()
            {
                Message = text
            };

            HookPoints.ConsoleMessageReceived.Invoke(ref ctx, ref args);
        }
    }
}

