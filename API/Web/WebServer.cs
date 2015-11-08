using System;
using System.IO;
using System.Threading;
using System.Web.Http;

using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Microsoft.Owin.Security.OAuth;
using Owin;

using OTA.Logging;
using OTA.Web.Internals;

namespace OTA.Web
{
    //Note to self, roles are to be kept at a minimum as the roles build the bearer token

    /// <summary>
    /// OTA web server for plugins. Currently based around OWIN
    /// </summary>
    public class WebServer
    {
        /// <summary>
        /// Gets or sets the Web API configuration.
        /// </summary>
        /// <value>The config.</value>
        public HttpConfiguration WebApiConfig { get; set; } = new HttpConfiguration();

        /// <summary>
        /// Gets or sets the static file directory when using the file server.
        /// </summary>
        /// <value>The static file directory.</value>
        public string StaticFileDirectory  { get; set; } = "Web";

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OTA.Web.WebServer"/> is allowed to use insecure http requests.
        /// </summary>
        /// <value><c>true</c> if to allow insecure http; otherwise, <c>false</c>.</value>
        public bool AllowInsecureHttp { get; set; } = true;

        /// <summary>
        /// Gets or sets the server key for use with data protection.
        /// </summary>
        /// <value>The server key.</value>
        public string ServerKey { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the session timeout in hours.
        /// </summary>
        /// <value>The session timeout hours.</value>
        public int SessionTimeoutHours  { get; set; } = 12;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OTA.Web.WebServer"/> uses OAuth.
        /// </summary>
        /// <value><c>true</c> if to use OAuth; otherwise, <c>false</c>.</value>
        public bool UseOAuth { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OTA.Web.WebServer"/> use the web API.
        /// </summary>
        /// <value><c>true</c> if to use the web API; otherwise, <c>false</c>.</value>
        public bool UseWebApi { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OTA.Web.WebServer"/> is used as a static file server
        /// </summary>
        /// <value><c>true</c> if you wish to use static files; otherwise, <c>false</c>.</value>
        public bool UseFileServer { get; set; } = true;

        /// <summary>
        /// Occurs before any configurations has been applied to the IAppBuilder
        /// </summary>
        public event EventHandler PreConfigure;

        /// <summary>
        /// Occurs after alls configurations have been applied to the IAppBuilder
        /// </summary>
        public event EventHandler PostConfigure;

        /// <summary>
        /// Occurs when the server has started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Occurs when the server has stopped
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Gets or sets the server options for OAuth.
        /// </summary>
        /// <value>The server options.</value>
        public OAuthAuthorizationServerOptions ServerOptions { get; set; }

        /// <summary>
        /// Gets or sets the bearer options for OAuth.
        /// </summary>
        /// <value>The bearer options.</value>
        public OAuthBearerAuthenticationOptions BearerOptions { get; set; }

        /// <summary>
        /// Gets or sets the OAuth token endpoint.
        /// </summary>
        /// <value>The OAuth token endpoint.</value>
        public string OAuthTokenEndpoint { get; set; } = "/token";

        /// <summary>
        /// If set this will stop the web server.
        /// </summary>
        internal readonly AutoResetEvent Switch = new AutoResetEvent(false);

        /// <summary>
        /// This will be called by OWIN to prepare the configuration for use.
        /// </summary>
        /// <param name="app">App.</param>
        public void Configuration(Owin.IAppBuilder app)
        {
            if (PreConfigure != null) PreConfigure.Invoke(this, EventArgs.Empty);

            if (UseOAuth)
            {
                app.UseOAuthAuthorizationServer(ServerOptions);
                app.UseOAuthBearerAuthentication(BearerOptions);
            }

            if (UseWebApi) app.UseWebApi(this.WebApiConfig);

            if (UseFileServer) app.UseFileServer(new Microsoft.Owin.StaticFiles.FileServerOptions()
                    {
                        RequestPath = new Microsoft.Owin.PathString("/web"),
                        FileSystem = new Microsoft.Owin.FileSystems.PhysicalFileSystem(this.StaticFileDirectory),
                        EnableDirectoryBrowsing = false
                    });
            
            if (PostConfigure != null) PostConfigure.Invoke(this, EventArgs.Empty);
        }

        public WebServer()
        {
            this.InitialiseOptions();

            WebApiConfig.Routes.MapHttpRoute
            (
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = System.Web.Http.RouteParameter.Optional }
            );

            //Ensure the plugin resolver is added
            WebApiConfig.Services.Replace(typeof(System.Web.Http.Dispatcher.IAssembliesResolver), new PluginServiceResolver());

            WebApiConfig.MapHttpAttributeRoutes();
        }

        /// <summary>
        /// Start the web server at the specified address
        /// </summary>
        /// <param name="baseAddress">Base address.</param>
        public void Start(string baseAddress)
        {
            Switch.Reset();
            (new Thread(StartServer)).Start(baseAddress);
        }

        /// <summary>
        /// Stop the web serverserver.
        /// </summary>
        public void Stop()
        {
            Switch.Set();
        }

        /// <summary>
        /// Initialises default server options
        /// </summary>
        private void InitialiseOptions()
        {
            ServerOptions = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new Microsoft.Owin.PathString(OAuthTokenEndpoint),

                AccessTokenExpireTimeSpan = TimeSpan.FromHours(SessionTimeoutHours),

                AllowInsecureHttp = AllowInsecureHttp,

                RefreshTokenFormat = new SecureDataFormat<AuthenticationTicket>(
                    DataSerializers.Ticket,
                    new MonoDataProtector(ServerKey),
                    TextEncodings.Base64
                ),
                AccessTokenFormat = new SecureDataFormat<AuthenticationTicket>(
                    DataSerializers.Ticket,
                    new MonoDataProtector(ServerKey),
                    TextEncodings.Base64
                ),
                AuthorizationCodeFormat = new SecureDataFormat<AuthenticationTicket>(
                    DataSerializers.Ticket,
                    new MonoDataProtector(ServerKey),
                    TextEncodings.Base64
                ),

                ApplicationCanDisplayErrors = true
            };

            BearerOptions = new OAuthBearerAuthenticationOptions()
            {
                AccessTokenFormat = new SecureDataFormat<AuthenticationTicket>(
                    DataSerializers.Ticket,
                    new MonoDataProtector(ServerKey),
                    TextEncodings.Base64
                ),
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = "Bearer"
            };
        }

        /// <summary>
        /// This is the thread that the OWIN server will run on
        /// </summary>
        /// <param name="baseAddress">Address to listen on.</param>
        private void StartServer(object baseAddress)
        {
            System.Threading.Thread.CurrentThread.Name = "Web";

            if (!Directory.Exists(StaticFileDirectory))
                Directory.CreateDirectory(StaticFileDirectory);

            try
            {
                using (Microsoft.Owin.Hosting.WebApp.Start<WebServer>(url: baseAddress as String))
                {
                    if (Started != null) Started.Invoke(this, EventArgs.Empty);
                    Switch.WaitOne();
                    if (Stopped != null) Stopped.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {
                ProgramLog.Log(e);
            }
        }
    }
}

