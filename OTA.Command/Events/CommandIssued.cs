using System;
using OTA.Command;
using OTA.Plugin;

namespace OTA.Commands.Events
{
    public static partial class CommandArgs
    {
        public struct CommandIssued
        {
            /// <summary>
            /// The command prefix that triggered the command.
            /// </summary>
            /// <example>help,say,exit</example>
            public string Prefix { get; internal set; }

            /// <summary>
            /// The ArgumentList instance that has pre-tokenised the arguments.
            /// </summary>
            public ArgumentList Arguments { get; set; }

            /// <summary>
            /// The full argument string received.
            /// </summary>
            public string ArgumentString { get; set; }
        }
    }

    public static partial class CommandEvents
    {
        /// <summary>
        /// Occurs when an unprocessed (yet valid) command is issued
        /// </summary>
        public static readonly HookPoint<CommandArgs.CommandIssued> CommandIssued = new HookPoint<CommandArgs.CommandIssued>();
    }
}