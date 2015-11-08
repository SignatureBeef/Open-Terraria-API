using System;
using System.Linq;

namespace OTA.Extensions
{
    /// <summary>
    /// Assembly extensions.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets types where they can be successfully loaded. This is useful when asseblies use lazy loading and don't have an optional component.
        /// </summary>
        /// <returns>The types loaded.</returns>
        /// <param name="assembly">Assembly.</param>
        public static Type[] GetTypesLoaded(this System.Reflection.Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null).ToArray();
            } 
        }
    }
}

