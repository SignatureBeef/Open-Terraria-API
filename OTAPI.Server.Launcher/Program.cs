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
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OTAPI.Launcher
{
    static class Program
    {
        static Assembly GetTerrariaAssembly() => typeof(Terraria.Animation).Assembly;

        static Hook LazyHook(string type, string method, Delegate callback)
        {
            var match = GetTerrariaAssembly().GetType(type);
            var func = match?.GetMethod(method);

            if (func != null)
            {
                return new Hook(func, callback);
            }
            return null;
        }

        static void Nop() { }

        static Dictionary<string, Assembly?> _assemblyCache = new Dictionary<string, Assembly?>();
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            Terraria.Program.OnLaunched += Program_OnLaunched;
            On.Terraria.Program.LaunchGame += Program_LaunchGame;
            //Terraria.WindowsLaunch.Main(args);

            GetTerrariaAssembly().EntryPoint.Invoke(null, new object[] { args });
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            lock (_assemblyCache)
            {
                var name = args.Name;
                var ix = name.IndexOf(',');
                if (ix > -1) name = name.Substring(0, ix);

                if (_assemblyCache.TryGetValue(name, out Assembly asm))
                {
                    return asm;
                }

                var path = Path.Combine("Libraries", name);
                if (Directory.Exists(path))
                {
                    var binaries = Directory.GetFiles(path, $"{name}.dll", SearchOption.AllDirectories);
                    foreach (var bin in binaries)
                    {
                        asm = Assembly.Load(File.ReadAllBytes(bin));
                        _assemblyCache[name] = asm;

                        return asm;
                    }
                }

                var file = $"{name}.dll";
                if (File.Exists(file))
                {
                    asm = Assembly.Load(File.ReadAllBytes(file));
                    _assemblyCache[name] = asm;
                    return asm;
                }

                return null;
            }
        }

        private static void Program_LaunchGame(On.Terraria.Program.orig_LaunchGame orig, string[] args, bool monoArgs)
        {
            Console.WriteLine("Preloading assembly...");

            if (GetTerrariaAssembly().EntryPoint.DeclaringType.Name != "MonoLaunch")
                Terraria.Program.ForceLoadAssembly(GetTerrariaAssembly(), initializeStaticMembers: true);

            orig(args, monoArgs);
        }

        private static void Program_OnLaunched(object sender, EventArgs e)
        {
            Console.WriteLine($"MonoMod runtime hooks active, runtime: " + DetourHelper.Runtime.GetType().Name);

            if (OTAPI.Common.IsTMLServer)
            {
                LazyHook("Terraria.ModLoader.Engine.HiDefGraphicsIssues", "Init", new Action(Nop));
            }

            On.Terraria.Main.ctor += Main_ctor;

            Hooks.MessageBuffer.ClientUUIDReceived += (_, args) =>
            {
                if (args.Event == HookEvent.After)
                    Console.WriteLine($"ClientUUIDReceived {Terraria.Netplay.Clients[args.Instance.whoAmI].ClientUUID}");
            };
            Hooks.NPC.MechSpawn += (_, args) =>
            {
                Console.WriteLine($"Hooks.NPC.MechSpawn x={args.X}, y={args.Y}, type={args.Type}, num={args.Num}, num2={args.Num2}, num3={args.Num3}");
            };
            Hooks.Item.MechSpawn += (_, args) =>
            {
                Console.WriteLine($"Hooks.Item.MechSpawn x={args.X}, y={args.Y}, type={args.Type}, num={args.Num}, num2={args.Num2}, num3={args.Num3}");
            };

            //Hooks.Main.StatusTextChange += Main_StatusTextChange;

            On.Terraria.Main.DedServ += Main_DedServ;

            On.Terraria.RemoteClient.Update += (orig, rc) =>
            {
                System.Console.WriteLine($"RemoteClient.Update: HOOK ID#{rc.Id} IsActive:{rc.IsActive},PT:{rc.PendingTermination}");
                orig(rc);
            };
            On.Terraria.RemoteClient.Reset += (orig, rc) =>
            {
                System.Console.WriteLine($"RemoteClient.Reset: HOOK ID#{rc.Id} IsActive:{rc.IsActive},PT:{rc.PendingTermination}");
                orig(rc);
            };
        }

        //private static void Main_StatusTextChange(object sender, Hooks.Main.StatusTextChangeArgs e)
        //{
        //    e.Value = "[OTAPI] " + e.Value;
        //}

        private static void Main_ctor(On.Terraria.Main.orig_ctor orig, Terraria.Main self)
        {
            orig(self);
            Terraria.Main.SkipAssemblyLoad = true; // we will do this.
            Console.WriteLine("Main invoked");
        }

        private static void Main_DedServ(On.Terraria.Main.orig_DedServ orig, Terraria.Main self)
        {
            Console.WriteLine($"Server init process successful");

            if (!Environment.GetCommandLineArgs().Any(x => x.ToLower() == "-test-init"))
                orig(self);
        }
    }
}
