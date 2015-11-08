using System;

namespace OTA.Web.Internals
{
    /// <summary>
    /// OWIN will call this to try to resolve Rest ApiControllers from a plugin
    /// </summary>
    internal class PluginServiceResolver : System.Web.Http.Dispatcher.DefaultAssembliesResolver
    {
        public override System.Collections.Generic.ICollection<System.Reflection.Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}

