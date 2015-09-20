using System;
using System.Linq;

#if WEBSERVER
using System.Net.Http;
using System.Web.Http;
using System.Net;

namespace OTA.Web.API
{
    /// <summary>
    /// Public access controllers.
    /// </summary>
    [AllowAnonymous]
    public class PublicController : ApiController
    {
        public static bool ShowPlugins { get; set; }

        /// <summary>
        /// Get the online player names
        /// </summary>
        public HttpResponseMessage Get()
        {
            return this.Request.CreateResponse(HttpStatusCode.OK, new {
                #if Full_API
                //Connection info
                Name = Terraria.Main.ActiveWorldFileData.Name,
                Port = Terraria.Netplay.ListenPort,

                PasswordRequired = !String.IsNullOrEmpty(Terraria.Netplay.ServerPassword),

                //Allocations
                MaxPlayers = Terraria.Main.maxNetPlayers,
                Player = Terraria.Main.player
                    .Where(x => x != null && x.active && !String.IsNullOrEmpty(x.Name))
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray(),

                //Installed plugins/mods
                Plugins = GetPlugins(),

                //Version info
                OTA = Globals.BuildInfo,
                Terraria = Terraria.Main.versionNumber,

                //Can be used to determine if the actual server is started or not
                ServerState = Globals.CurrentState
                #endif
            });
        }

        private string[] GetPlugins()
        {
            if (ShowPlugins)
            {
                return PluginManager._plugins
                    .Where(x => x.Value.IsEnabled)
                    .Select(y => y.Value.Name)
                    .OrderBy(z => z)
                    .ToArray();
            }

            return null;
        }
    }
}
#endif