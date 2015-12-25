using System;

namespace OTA.Plugin
{
    /// <summary>
    /// Plugin dependency attribute. Use this when your plugin depends on another plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class PluginDependencyAttribute : Attribute
    {
        public string Dependency { get; set; }

        public PluginDependencyAttribute(string dependencyType)
        {
            this.Dependency = dependencyType;
        }
    }
}

