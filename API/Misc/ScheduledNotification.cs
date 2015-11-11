using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTA.Logging;

namespace OTA.Misc
{
    /// <summary>
    /// Scheduled notification for players or the console
    /// </summary>
    // TODO Allow per-player
    public class ScheduledNotification : GameTask
    {
        private new Action<GameTask> Method { get; set; }

        private string _message;
        private Color _colour;

        public bool ConsoleOnly { get; set; }

        public ScheduledNotification(string message, Color colour, int seconds)
        {
            _message = message;
            _colour = colour;
            base.Trigger = seconds;
            base.Method = (tsk) =>
            {
                if (ConsoleOnly) Logger.Log(_message);
                else Tools.NotifyAllPlayers(_message, _colour);
            };
            Tasks.Schedule(this);
        }
    }
}
