using System;
using System.Linq;
#if WEBSERVER
using System.Web.Http;
using System.Security.Claims;

namespace OTA.Web.API
{
    [Authorize(Roles = "registered")]
    public class UserController : ApiController
    {
        public System.Collections.Generic.IEnumerable<object> Get()
        {
            var identity = User.Identity as ClaimsIdentity;

            return identity.Claims.Select(x => new
                {
                    Type = x.Type,
                    Value = x.Value
                });
        }
    }
}
#endif