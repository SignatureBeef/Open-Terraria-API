using System;
using System.Net.Http;
using System.Net;
using System.Data.Entity;
using OTA.Data;
using System.Collections.Concurrent;
using OTA.Misc;

#if WEBSERVER
using System.Threading;
using System.IO;
using OTA.Logging;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.DataHandler.Serializer;
using System.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security.OAuth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
#endif

//Note to self, roles are to be kept at a minimum as the roles build the bearer token

namespace OTA.Web
{
    /// <summary>
    /// OTA web server for plugins. Currently based around OWIN
    /// </summary>
    public static class WebServer
    {
        #if WEBSERVER
        public static System.Web.Http.HttpConfiguration Config { get; private set; }

        public static string StaticFileDirectory = "Web";
        public static bool AllowInsecureHttp = true;
        public static string ServerKey = Guid.NewGuid().ToString();
        public static int SessionTimeoutHours = 12;

        public static int RequestLockoutDuration = 10;
        public static int MaxRequestsPerLapse = 15;

        internal static readonly AutoResetEvent Switch = new AutoResetEvent(false);

        static WebServer()
        {
            Config = new System.Web.Http.HttpConfiguration();

            Config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = System.Web.Http.RouteParameter.Optional }
            );

            //            Config.DependencyResolver = new AssembliesResolver();
            Config.Services.Replace(typeof(System.Web.Http.Dispatcher.IAssembliesResolver), new PluginServiceResolver());

            Config.MapHttpAttributeRoutes();
            //            Config.Formatters.Add(new System.Net.Http.Formatting.JsonMediaTypeFormatter());
        }
        #endif

        /// <summary>
        /// Start the web server at the specified address
        /// </summary>
        /// <param name="baseAddress">Base address.</param>
        public static void Start(string baseAddress)
        {
#if WEBSERVER
            Switch.Reset();
            (new Thread(OWINServer.StartServer)).Start(baseAddress);
#endif
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public static void Stop()
        {
#if WEBSERVER
            Switch.Set();
#endif
        }
    }

    #if WEBSERVER
    class PluginServiceResolver : System.Web.Http.Dispatcher.DefaultAssembliesResolver
    {
        public override System.Collections.Generic.ICollection<System.Reflection.Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }

    //    public class ApplicationRoleManager : RoleManager<IdentityRole>
    //    {
    //        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
    //            : base(roleStore)
    //        {
    //        }
    //
    //        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
    //        {
    //            var appRoleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<OTAContext>()));
    //
    //            return appRoleManager;
    //        }
    //    }



    class OWINServer
    {
        

        public static OAuthAuthorizationServerOptions ServerOptions = new OAuthAuthorizationServerOptions()
        {
            TokenEndpointPath = new Microsoft.Owin.PathString("/token"),
//            Provider = new PermissionsOAuthProvider(),

            AccessTokenExpireTimeSpan = TimeSpan.FromHours(WebServer.SessionTimeoutHours),

            AllowInsecureHttp = true, //WebServer.AllowInsecureHttp,

            /* Use app.SetDataProtectionProvider ? */
            RefreshTokenFormat =
                new SecureDataFormat<AuthenticationTicket>(DataSerializers.Ticket,
                new MonoDataProtector(WebServer.ServerKey), TextEncodings.Base64),
            AccessTokenFormat =
                new SecureDataFormat<AuthenticationTicket>(DataSerializers.Ticket,
                new MonoDataProtector(WebServer.ServerKey), TextEncodings.Base64),
            AuthorizationCodeFormat =
                new SecureDataFormat<AuthenticationTicket>(DataSerializers.Ticket,
                new MonoDataProtector(WebServer.ServerKey), TextEncodings.Base64),

            ApplicationCanDisplayErrors = true
        };

        public static OAuthBearerAuthenticationOptions BearerOptions = new OAuthBearerAuthenticationOptions()
        {
            AccessTokenFormat =
                new SecureDataFormat<AuthenticationTicket>(DataSerializers.Ticket,
                new MonoDataProtector(WebServer.ServerKey), TextEncodings.Base64),
            AuthenticationMode = AuthenticationMode.Active,
            AuthenticationType = "Bearer"
        };

        public void Configuration(Owin.IAppBuilder app)
        {
            //app.UseErrorPage();
            app.UseOAuthAuthorizationServer(ServerOptions);
            app.UseOAuthBearerAuthentication(BearerOptions);
            //            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            app.UseWebApi(WebServer.Config);

            //            app.SetDataProtectionProvider( TODO

            app.UseFileServer(new Microsoft.Owin.StaticFiles.FileServerOptions()
                {
                    RequestPath = new Microsoft.Owin.PathString("/web"),
                    FileSystem = new Microsoft.Owin.FileSystems.PhysicalFileSystem(WebServer.StaticFileDirectory),
                    EnableDirectoryBrowsing = false
                });

        }

        /// <summary>
        /// A mono compatible data protector for use with OWIN
        /// </summary>
        internal class MonoDataProtector : IDataProtector
        {
            private string[] _purposes;
            const String DefaultPurpose = "ota-web-dp";

            public MonoDataProtector()
            {
                _purposes = null;
            }

            //            public MonoDataProtector(string[] purposes)
            //            {
            //                _purposes = purposes;
            //            }

            public MonoDataProtector(params string[] purposes)
            {
                _purposes = purposes;
            }

            public byte[] Protect(byte[] data)
            {
                return System.Security.Cryptography.ProtectedData.Protect(data, this.GenerateEntropy(), DataProtectionScope.CurrentUser);
            }

            public byte[] Unprotect(byte[] data)
            {
                return System.Security.Cryptography.ProtectedData.Unprotect(data, this.GenerateEntropy(), DataProtectionScope.CurrentUser);
            }

            byte[] GenerateEntropy()
            {
                using (var hasher = SHA256.Create())
                {
                    using (var ms = new MemoryStream())
                    using (var cr = new CryptoStream(ms, hasher, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cr))
                    {
                        //Default purpose 
                        sw.Write(DefaultPurpose);

                        if (_purposes != null)
                            foreach (var purpose in _purposes)
                            {
                                sw.Write(purpose);
                            }
                    }

                    return hasher.Hash;
                }
            }
        }

        internal static void StartServer(object baseAddress)
        {
            System.Threading.Thread.CurrentThread.Name = "Web";

            ProgramLog.Web.Log("Initialising web server");

            if (!Directory.Exists(WebServer.StaticFileDirectory))
            {
                Directory.CreateDirectory(WebServer.StaticFileDirectory);
            }

            try
            {
                using (Microsoft.Owin.Hosting.WebApp.Start<OWINServer>(url: baseAddress as String))
                {
                    ProgramLog.Web.Log("Web server started listening on {0}", baseAddress);
                    WebServer.Switch.WaitOne();
                }
            }
            catch (Exception e)
            {
                ProgramLog.Log(e);
            }
        }
    }
    #endif
}

