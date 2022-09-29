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
using OTAPI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

//namespace OTAPI.Launcher;

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

static void Program_LaunchGame(On.Terraria.Program.orig_LaunchGame orig, string[] args, bool monoArgs)
{
#if !TML
    Console.WriteLine("Preloading assembly...");

    if (GetTerrariaAssembly().EntryPoint.DeclaringType.Name != "MonoLaunch")
    {
        Terraria.Main.dedServ = true;
        Terraria.Program.ForceLoadAssembly(GetTerrariaAssembly(), initializeStaticMembers: true);
    }

#endif
    orig(args, monoArgs);
}

static void Program_OnLaunched(object sender, EventArgs e)
{
    Console.WriteLine($"MonoMod runtime hooks active, runtime: " + DetourHelper.Runtime.GetType().Name);

    if (Common.IsTMLServer)
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

    On.Terraria.Main.DedServ += Main_DedServ;

    On.Terraria.RemoteClient.Update += (orig, rc) =>
    {
        Console.WriteLine($"RemoteClient.Update: HOOK ID#{rc.Id} IsActive:{rc.IsActive},PT:{rc.PendingTermination}");
        orig(rc);
    };
    On.Terraria.RemoteClient.Reset += (orig, rc) =>
    {
        Console.WriteLine($"RemoteClient.Reset: HOOK ID#{rc.Id} IsActive:{rc.IsActive},PT:{rc.PendingTermination}");
        orig(rc);
    };
}

static void Main_ctor(On.Terraria.Main.orig_ctor orig, Terraria.Main self)
{
    orig(self);
    Terraria.Main.SkipAssemblyLoad = true; // we will do this.
    Console.WriteLine("Main invoked");
}

static void Main_DedServ(On.Terraria.Main.orig_DedServ orig, Terraria.Main self)
{
    Console.WriteLine($"Server init process successful");

    if (!Environment.GetCommandLineArgs().Any(x => x.ToLower() == "-test-init"))
        orig(self);
}

Terraria.Program.OnLaunched += Program_OnLaunched;
On.Terraria.Program.LaunchGame += Program_LaunchGame;

#if TML
On.MonoLaunch.GetBaseDirectory += (orig) =>
{
    return Path.Combine(Environment.CurrentDirectory, "tModLoader");
};

Terraria.ModLoader.Engine.InstallVerifier.steamAPIPath = Path.Combine("tModLoader", Terraria.ModLoader.Engine.InstallVerifier.steamAPIPath);
if (args == null || args.Length == 0)
    args = new[] { "-server" };

if (!args.Any(s => s.Equals("-server")))
    args = args.Concat(new[] { "-server" }).ToArray();
#endif

// let plugins reference the runtime hooks. this is up to the consumer
Terraria.Program.ModContext.ReferenceFiles.Add("OTAPI.Runtime.dll");
Terraria.Program.ModContext.ReferenceFiles.Add("Mono.Cecil.dll");
Terraria.Program.ModContext.ReferenceFiles.Add("MonoMod.dll");
Terraria.Program.ModContext.ReferenceFiles.Add("MonoMod.RuntimeDetour.dll");
Terraria.Program.ModContext.ReferenceFiles.Add("ModFramework.dll");

GetTerrariaAssembly().EntryPoint.Invoke(null, new object[] { args });
