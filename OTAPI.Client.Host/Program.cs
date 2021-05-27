// Copyright (C) 2020-2021 DeathCradle
//
// This file is part of Open Terraria API v3 (OTAPI)
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
using System;

namespace OTAPI.Client.Host
{
    class Program
    {
        static System.Reflection.Assembly TerrariaAssembly;

        public static void Main(string[] args)
        {
            Console.WriteLine("[OTAPI.Client] Hellow!");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;

            using (var lua = new Triton.Lua())
            {
                lua.ImportNamespace("System");
                lua.ImportNamespace("System.Console");
                lua.ImportType(typeof(System.Console));
                lua.DoString("Console.WriteLine('test from lua')");
            }

            Console.WriteLine("[OTAPI.Client] Starting!");

            //Terraria.Program.OnLaunched += (_, _) =>
            //{
            //    Console.WriteLine("Launched");

            //    Terraria.Main.versionNumber += " [OTAPI.Client]";
            //    Terraria.Main.versionNumber2 += " [OTAPI.Client]";

            //    using (var lua = new Triton.Lua())
            //    {
            //        lua.ImportNamespace("Terraria");
            //        lua.DoString(@"
            //            Main.versionNumber = Main.versionNumber .. ' Hellow from LUA'
            //        ");
            //    }
            //};
            //Terraria.MacLaunch.Main(args);
            IsolatedLaunch.Launch(args);
        }

        private static System.Reflection.Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("Looking for type: " + args.Name);
            return null;
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("Looking for: " + args.Name);
            if (args.Name.StartsWith("Terraria") || args.Name.StartsWith("OTAPI"))
            {
                if (TerrariaAssembly is null)
                {
                    TerrariaAssembly = System.Reflection.Assembly.LoadFile(
                        System.IO.Path.Combine(Environment.CurrentDirectory, "Terraria.patched.exe")
                    );
                    Console.WriteLine("Loaded terraria");
                }
                Console.WriteLine("Returning terraria");
                return TerrariaAssembly;
            }
            return null;
        }
    }
}
