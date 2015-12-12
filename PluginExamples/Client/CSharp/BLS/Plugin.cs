using System;
using OTA.Client.Item;
using OTA.Client;
using OTA.Plugin;

namespace BLS
{
    [OTAVersion(1, 0)]
    public class Plugin : BasePlugin
    {
        public Plugin()
        {
            this.Author = "DeathCradle";
            this.Description = "Client hook testing plugin.";
            this.Name = "BLS";
            this.Version = "1.0";
        }

        protected override void Initialized(object state)
        {
            base.Initialized(state);
        }

        protected override void Enabled()
        {
            base.Enabled();
            OTA.Logging.ProgramLog.Plugin.Log($"BLS enabled");
        }
    }
}

