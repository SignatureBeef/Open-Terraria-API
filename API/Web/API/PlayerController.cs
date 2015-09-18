using System;
using System.Linq;
#if WEBSERVER
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace OTA.Web.API
{
    /// <summary>
    /// Player controller.
    /// </summary>
    [Authorize(Roles = "player,SuperAdmin")]
    public class PlayerController : ApiController
    {
        public HttpResponseMessage Get(string name)
        {
            #if Full_API
            return this.Request.CreateResponse(HttpStatusCode.OK,
                Terraria.Main.player
                .Where(x => x != null && x.active && x.Name == name)
                .Select(x => new { Name = x.name, Position = x.position })
                .FirstOrDefault()
            );
            #else
            return null;
            #endif
        }
    }
}
#endif