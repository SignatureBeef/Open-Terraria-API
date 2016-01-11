using System;
using OTA.Command;
using OTA.Plugin;

namespace OTA.Commands.Events
{
    public static partial class CommandArgs
    {
        public struct Listening
        {
        }
    }

    public static partial class CommandEvents
    {
        public static readonly HookPoint<CommandArgs.Listening> Listening = new HookPoint<CommandArgs.Listening>();
    }
}