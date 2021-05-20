/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace ModFramework.Plugins
{
    public class DefaultAssemblyLoader : IAssemblyLoader
    {
        public Assembly Load(string path)
        {
            path = Path.GetFullPath(path);

            // most likely better to have in the host app
            //var resolver = new AssemblyDependencyResolver(path);
            //AssemblyLoadContext.Default.Resolving += (AssemblyLoadContext arg1, AssemblyName arg2) =>
            //{
            //    string assemblyPath = resolver.ResolveAssemblyToPath(arg2);
            //    if (assemblyPath != null)
            //        return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            //    return null;
            //};

            return AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        }

        public Assembly Load(System.IO.MemoryStream assembly, System.IO.MemoryStream symbols = null)
        {
            return AssemblyLoadContext.Default.LoadFromStream(assembly, symbols);
        }
    }
}
