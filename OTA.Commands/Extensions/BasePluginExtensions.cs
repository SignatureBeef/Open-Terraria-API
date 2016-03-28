using System;
using OTA.Plugin;
using OTA.Command;
using System.Linq;

namespace OTA.Commands
{
    public static class BasePluginExtensions
    {
        public static bool HasRunningCommands(this BasePlugin plugin)
        {
            var commands = CommandManager.Parser.GetPluginCommands(plugin);
            var runningCommands = commands.Where(x => x._running).Count();
            var pausedCommands = commands.Where(x => x._paused).Count();

            return (runningCommands - pausedCommands - CommandManager<CommandInfo>.threadInCommand) > 0;
        }

        /// <summary>
        /// Adds a new command to the server's command list
        /// </summary>
        /// <param name="prefix">Command text</param>
        /// <returns>New Command</returns>
        public static CommandInfo AddCommand(this BasePlugin plugin, params string[] prefix)
        {
            return plugin.AddCommand<CommandInfo>(prefix);
        }

        /// <summary>
        /// Adds a new command to the server's command list
        /// </summary>
        /// <param name="prefix">Command text</param>
        /// <returns>New Command</returns>
        public static T AddCommand<T>(this BasePlugin plugin, params string[] aliases) where T : CommandDefinition
        {
            var cmd = CommandManager.Parser.FindOrCreate<T>(aliases);
            cmd.Plugin = plugin;

            return cmd;
        }
    }
}

