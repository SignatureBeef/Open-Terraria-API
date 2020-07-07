using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OTAPI.Plugins
{
    public static class PluginLoader
    {
        private static List<Assembly> _assemblies;

        public static IEnumerable<Assembly> Assemblies => _assemblies;

        public static bool TryLoad()
        {
            if (_assemblies == null)
            {
                _assemblies = new List<Assembly>();
                _assemblies.Add(Assembly.GetExecutingAssembly());

                if (Directory.Exists("modifications"))
                {
                    foreach (var file in Directory.EnumerateFiles("modifications", "*.dll", SearchOption.AllDirectories))
                    {
                        try
                        {
                            Console.WriteLine($"[OTAPI:Startup] Loading {file}");
                            // todo allow for AssemblyLoadContext
                            var asm = System.Reflection.Assembly.Load(System.IO.File.ReadAllBytes(file));

                            _assemblies.Add(asm);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[OTAPI:Startup] Load failed {ex}");
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
