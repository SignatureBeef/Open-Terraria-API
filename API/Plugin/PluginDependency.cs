using System;

namespace OTA.Plugin
{
    /// <summary>
    /// Plugin dependency attribute. Use this when your plugin depends on another plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class PluginDependencyAttribute : Attribute
    {
        public string AssemblyName { get; set; }

        public PluginDependencyAttribute(string assemblyName)
        {
            this.AssemblyName = assemblyName;
        }
    }
}

