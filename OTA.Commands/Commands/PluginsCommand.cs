using System;
using OTA.Command;
using OTA.Plugin;

namespace OTA.Commands
{
    public class PluginsCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("plugins")
                .WithDescription("Lists installed plugins")
                .SetDefaultUsage()
                .WithPermissionNode("ota.plugins")
                .Calls(Plugins);
        }

        public void Plugins(ISender sender, ArgumentList args)
        {
            if (PluginManager.PluginCount > 0)
            {
                string plugins = String.Empty;

                foreach (var plugin in PluginManager.EnumeratePlugins)
                {
                    if (!plugin.IsEnabled || plugin.Name.Trim().Length > 0)
                    {
                        var name = plugin.Name.Trim();
                        if (!String.IsNullOrEmpty(plugin.Version))
                        {
                            name += " (" + plugin.Version + ")";
                        }
                        plugins += ", " + name;
                    }
                }
                if (plugins.StartsWith(","))
                {
                    plugins = plugins.Remove(0, 1).Trim(); //Remove the ', ' from the start and trim the ends
                }
                sender.Message("Loaded plugins: " + plugins + ".");
            }
            else
            {
                sender.Message("No plugins loaded.");
            }
        }
    }
}