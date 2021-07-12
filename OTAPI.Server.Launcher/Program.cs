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
using System.Linq;

namespace OTAPI.Launcher
{
    static class Program
    {
        static Hook LazyHook(string type, string method, Delegate callback)
        {
            var match = typeof(Terraria.Main).Assembly.GetType(type);
            var func = match?.GetMethod(method);

            if (func != null)
            {
                return new Hook(func, callback);
            }
            return null;
        }

        static void Nop() { }

        static void Main(string[] args)
        {
            On.Terraria.WindowsLaunch.Main += WindowsLaunch_Main;
            Terraria.Program.OnLaunched += Program_OnLaunched;
            Terraria.WindowsLaunch.Main(args);
        }

        private static void Program_OnLaunched(object sender, EventArgs e)
        {
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

            if (Environment.GetCommandLineArgs().Any(x => x.ToLower() == "-test-init"))
                On.Terraria.Main.DedServ += Main_DedServ;
        }

        //private static void Main_StatusTextChange(object sender, Hooks.Main.StatusTextChangeArgs e)
        //{
        //    e.Value = "[OTAPI] " + e.Value;
        //}

        private static void Main_ctor(On.Terraria.Main.orig_ctor orig, Terraria.Main self)
        {
            orig(self);
            Terraria.Main.SkipAssemblyLoad = true;
            //    //Terraria.Program.ForceLoadThread(null);
        }

        private static void Main_DedServ(On.Terraria.Main.orig_DedServ orig, Terraria.Main self)
        {
            Console.WriteLine($"Server init process successful");
        }

        private static void WindowsLaunch_Main(On.Terraria.WindowsLaunch.orig_Main orig, string[] args)
        {
            Console.WriteLine($"MonoMod runtime hooks active");
            orig(args);
        }
    }
}
