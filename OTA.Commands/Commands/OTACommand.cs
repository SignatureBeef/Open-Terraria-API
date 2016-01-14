using System;
using OTA.Extensions;
using System.Linq;

namespace OTA.Commands
{
    public abstract class OTACommand
    {
        public CommandPlugin Plugin { get; private set; }

        public abstract void Initialise();

        public CommandInfo AddCommand(string prefix)
        {
            return Plugin.AddCommand(prefix);
        }

        public static void Initialise(CommandPlugin plugin)
        {
            var type = typeof(OTACommand);
            foreach (var messageType in type.Assembly
                .GetTypesLoaded()
                .Where(x => type.IsAssignableFrom(x) && x != type && !x.IsAbstract))
            {
                var cmd = (OTACommand)Activator.CreateInstance(messageType);
                cmd.Plugin = plugin;

                cmd.Initialise();
            }
        }
    }
}

