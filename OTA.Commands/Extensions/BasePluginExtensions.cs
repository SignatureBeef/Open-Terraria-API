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
            var runningCommands = commands.Where(x => x.running).Count();
            var pausedCommands = commands.Where(x => x.paused).Count();

            return (runningCommands - pausedCommands - CommandManager<CommandInfo>.threadInCommand) > 0;
        }

        /// <summary>
        /// Adds a new command to the server's command list
        /// </summary>
        /// <param name="prefix">Command text</param>
        /// <returns>New Command</returns>
        public static CommandInfo AddCommand(this BasePlugin plugin, string prefix, bool replaceExisting = false)
        {
            return plugin.AddCommand<CommandInfo>(prefix, replaceExisting);
        }

        /// <summary>
        /// Adds a new command to the server's command list
        /// </summary>
        /// <param name="prefix">Command text</param>
        /// <returns>New Command</returns>
        public static T AddCommand<T>(this BasePlugin plugin, string prefix, bool replaceExisting = false) where T : CommandDefinition
        {
            var cmd = CommandManager.Parser.FindOrCreate<T>(prefix, replaceExisting);
            cmd.Plugin = plugin;

            return cmd;
        }
    }
}

