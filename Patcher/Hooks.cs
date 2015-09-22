using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace OTA.Patcher
{
    /// <summary>
    /// Console helper.
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// Clears the current console line
        /// </summary>
        public static void ClearLine()
        {
            var current = System.Console.CursorTop;
            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(0, current);
        }
    }

    /// <summary>
    /// Server specific hook
    /// </summary>
    public sealed class ServerHookAttribute : Attribute
    {

    }

    /// <summary>
    /// Client specific hook
    /// </summary>
    public sealed class ClientHookAttribute : Attribute
    {

    }

    /// <summary>
    /// This file is specifically for Vanilla hooks.
    /// When Terraria is released for multi-platforms our core server class will not be of service anymore, or atleast we can reuse the Packet code from vanilla.
    /// What this class will do is expose the hooks we need for plugins. E.g. OnPlayerJoined
    /// </summary>
    public partial class Injector
    {
        public TerrariaOrganiser Terraria;
        public APIOrganiser API;

        private void InitOrganisers()
        {
            Terraria = new TerrariaOrganiser(_asm);
            API = new APIOrganiser(_self);
        }

        /// <summary>
        /// Grabs all available hooks and executes them to hook into the target assembly
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void InjectHooks<T>()
        {
            var hooks = typeof(Injector)
                .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes(typeof(T), false).Count() == 1)
                .ToArray();

            string line = null;

            for (var x = 0; x < hooks.Length; x++)
            {
                const String Fmt = "Patching in hooks - {0}/{1}";

                if (line != null)
                    ConsoleHelper.ClearLine();

                line = String.Format(Fmt, x + 1, hooks.Length);
                Console.Write(line);

                hooks[x].Invoke(this, null);
            }

            //Clear ready for the Ok\n
            if (line != null)
                ConsoleHelper.ClearLine();
            Console.Write("Patching in hooks - ");
        }

        [ServerHook]
        void OnPlayerKilled() //OnEntityHurt
        {
            //Routing all instances because i'm yet again in another rush
            //Anyone, feel free to swap out to pure IL directly in the deathMsg body ;)

            var mth = Terraria.Player.Methods.Single(x => x.Name == "KillMe");
            var hook = _asm.MainModule.Import(API.Player.Methods.Single(x => x.Name == "OnPlayerKilled"));

            var il = mth.Body.GetILProcessor();
            var first = mth.Body.Instructions.First();

            //Contruct the call to the API

            //Arguments
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            //            il.InsertBefore(first, il.Create(OpCodes.Ldarg_1));
            //            il.InsertBefore(first, il.Create(OpCodes.Ldarg_2));
            il.InsertBefore(first, il.Create(OpCodes.Ldarga_S, mth.Parameters.Single(x => x.Name == "dmg")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarga_S, mth.Parameters.Single(x => x.Name == "hitDirection")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarga_S, mth.Parameters.Single(x => x.Name == "pvp")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarga_S, mth.Parameters.Single(x => x.Name == "deathText")));

            //Call
            il.InsertBefore(first, il.Create(OpCodes.Call, hook));
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ret));

            //Remove the Concat with the Entity name+death-message and leave the message
            //We will reproduce this in the API so plugins can have full control
            var matches = mth.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
                              && x.Operand is MethodReference
                              && (x.Operand as MethodReference).Name == "Concat")
                .Reverse() /* Remove IL from the bottom up */
                .ToArray();

            foreach (var match in matches)
            {
                il.Remove(match.Previous.Previous.Previous); //this
                il.Remove(match.Previous.Previous); //.name
                il.Remove(match); //call
            }
        }

        [ClientHook]
        [ServerHook]
        void OnPlayerHurt() //OnEntityHurt
        {
            //Routing all instances because i'm yet again in another rush
            //Anyone, feel free to swap out to pure IL directly in the deathMsg body ;)

            var mth = Terraria.Player.Methods.Single(x => x.Name == "Hurt");
            var hook = _asm.MainModule.Import(API.Player.Methods.Single(x => x.Name == "OnPlayerHurt"));

            var il = mth.Body.GetILProcessor();
            var first = mth.Body.Instructions.First();

            //Contruct the call to the API

            //Arguments
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_1));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_2));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, mth.Parameters.Single(x => x.Name == "pvp")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, mth.Parameters.Single(x => x.Name == "quiet")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, mth.Parameters.Single(x => x.Name == "deathText")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, mth.Parameters.Single(x => x.Name == "Crit")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, mth.Parameters.Single(x => x.Name == "cooldownCounter")));

            //Call
            il.InsertBefore(first, il.Create(OpCodes.Call, hook));
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ldc_R8, 0.0));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        [ServerHook]
        void OnDeathMessage()
        {
            //Routing all instances because i'm yet again in another rush
            //Anyone, feel free to swap out to pure IL directly in the deathMsg body ;)

            //            var mth = Terraria.Lang.Methods.Single(x => x.Name == "deathMsg");
            var hook = _asm.MainModule.Import(API.VanillaHooks.Methods.Single(x => x.Name == "OnDeathMessage"));

            foreach (var type in _asm.MainModule.Types)
            {
                foreach (var mth in type.Methods)
                {
                    if (mth.Body != null)
                        foreach (var ins in mth.Body.Instructions)
                        {
                            var mr = ins.Operand as MethodReference;
                            if (mr != null && mr.Name == "deathMsg")
                            {
                                ins.Operand = hook;
                            }
                        }
                }
            }
        }

        [ServerHook]
        private void OnPlayerEntering()
        {
            var getData = Terraria.MessageBuffer.Methods.Single(x => x.Name == "GetData");
            var match = getData.Body.Instructions.First(x => x.Operand is String && x.Operand.Equals("Empty name."));
            var callback = API.VanillaHooks.Methods.Single(x => x.Name == "OnPlayerEntering");

            var il = getData.Body.GetILProcessor();

            //Find the second RETURN and insert our callback just before.
            int count = 0;
            while (count < 2)
            {
                match = match.Next;

                if (match.OpCode == OpCodes.Ret)
                    count++;
            }

            var playerObject = match.Previous;
            count = 0;
            while (count < 2)
            {
                playerObject = playerObject.Previous;

                if (playerObject.OpCode == OpCodes.Ldloc_S)
                    count++;
            }

            il.InsertBefore(match, il.Create(OpCodes.Ldloc, playerObject.Operand as VariableDefinition));
            il.InsertBefore(match, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        [ServerHook]
        private void OnPlayerLeave()
        {
            var clientReset = Terraria.RemoteClient.Methods.Single(x => x.Name == "Reset");
            var callback = API.VanillaHooks.Methods.Single(x => x.Name == "OnPlayerLeave");

            //Find the first ldsfld where the player is being reset
            var ins = clientReset.Body.Instructions.First(x => x.OpCode == OpCodes.Ldsfld && x.Operand is FieldReference && (x.Operand as FieldReference).Name == "player");

            var il = clientReset.Body.GetILProcessor();

            //Insert the player reference on the stack
            il.InsertBefore(ins, il.Create(OpCodes.Ldsfld, ins.Operand as FieldReference)); //Static Player array
            il.InsertBefore(ins, il.Create(OpCodes.Ldarg_0)); //Current RemoteClient object
            il.InsertBefore(ins, il.Create(OpCodes.Ldfld, _asm.MainModule.Import(Terraria.RemoteClient.Fields.Single(x => x.Name == "Id")))); //Id (index) for use with the player array
            il.InsertBefore(ins, il.Create(OpCodes.Ldelem_Ref)); //Now load the array at the index

            //Call the hook with the player
            il.InsertBefore(ins, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        [ServerHook]
        private void OnGreetPlayer()
        {
            var greetPlayer = Terraria.NetMessage.Methods.Single(x => x.Name == "greetPlayer");
            var callback = API.VanillaHooks.Methods.Single(x => x.Name == "OnGreetPlayer");
            var il = greetPlayer.Body.GetILProcessor();

            var first = greetPlayer.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        [ServerHook]
        private void OnConnectionAccepted()
        {
            var oca = Terraria.Netplay.Methods.Single(x => x.Name == "OnConnectionAccepted");
            var callback = API.NetplayCallback.Methods.Single(x => x.Name == "OnNewConnection");
            var ldsfld = oca.Body.Instructions.First(x => x.OpCode == OpCodes.Ldsfld);

            var il = oca.Body.GetILProcessor();
            il.InsertBefore(ldsfld, il.Create(OpCodes.Ldloc_0));
            il.InsertBefore(ldsfld, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        [ServerHook]
        private void OnNPCKilled()
        {
            var oca = Terraria.NPC.Methods.Single(x => x.Name == "checkDead");
            var callback = API.NPCCallback.Methods.Single(x => x.Name == "OnNPCKilled");


            var ins = oca.Body.Instructions.Where(x =>
                x.OpCode == OpCodes.Stfld
                          && x.Operand is FieldReference
                          && (x.Operand as FieldReference).Name == "active").FirstOrDefault().Previous.Previous;


            var il = oca.Body.GetILProcessor();
            il.InsertAfter(ins, il.Create(OpCodes.Ldarg_0));
            il.InsertAfter(ins, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        /// <summary>
        /// Hooks Begin and End of Terraria.Main.[Draw/Update]
        /// </summary>
        [ClientHook]
        private void HookXNAEvents()
        {
            foreach (var mth in new string[] { "Draw", "Update", "UpdateClient" })
            {
                var method = Terraria.Main.Methods.Single(x => x.Name == mth);
                var begin = API.MainCallback.Methods.First(m => m.Name == "On" + mth + "Begin");
                var end = API.MainCallback.Methods.First(m => m.Name == "On" + mth + "End");

                var il = method.Body.GetILProcessor();
                var first = method.Body.Instructions.First();

                il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(begin)));

                //Before any exit point
                foreach (var ins in method.Body.Instructions.Where(x => x.OpCode == OpCodes.Ret).Reverse())
                {
                    il.InsertBefore(ins, il.Create(OpCodes.Call, _asm.MainModule.Import(end)));
                }
            }
        }

        /// <summary>
        /// Hooks Begin and End of Terraria.WorldGen.clearWorld
        /// </summary>
        [ServerHook]
        [ClientHook]
        private void HookClearWorld()
        {
            var method = Terraria.WorldGen.Methods.Single(x => x.Name == "clearWorld");
            var replacement = API.WorldFileCallback.Methods.First(m => m.Name == "ClearWorld");

            var calls = _asm.MainModule.Types
                .SelectMany(x => x.Methods)
                .Where(y => y.HasBody)
                .SelectMany(z => z.Body.Instructions)
                .Where(ins => ins.OpCode == OpCodes.Call && ins.Operand is MethodReference && (ins.Operand as MethodReference).Name == "clearWorld")
                .ToArray();

            foreach (var ins in calls)
            {
                ins.Operand = _asm.MainModule.Import(replacement);
            }
        }

        /// <summary>
        /// Called on every server update tick, regardless of connections
        /// </summary>
        [ServerHook]
        private void HookServerTick()
        {
            var method = Terraria.Main.Methods.Single(x => x.Name == "DedServ");
            var addition = API.MainCallback.Methods.First(m => m.Name == "ServerTick");

            var onTick = method.Body.Instructions
                .Where(ins => ins.OpCode == OpCodes.Ldsfld && ins.Operand is FieldReference && (ins.Operand as FieldReference).Name == "OnTick")
                .First();

            var il = method.Body.GetILProcessor();

            //Inject our call
            var repl = il.Create(OpCodes.Call, _asm.MainModule.Import(addition));
            il.InsertBefore(onTick, repl);

            //Now move the Update if block back above us
            repl.Previous.Previous.Previous.Previous.Operand = repl;
        }

        /// <summary>
        /// Replaces the world save with OTA's custom one
        /// </summary>
        [ServerHook]
        private void HookWorldAutoSave()
        {
            var method = Terraria.WorldGen.Methods.Single(x => x.Name == "saveAndPlay");
            var replacement = _asm.MainModule.Import(API.WorldFileCallback.Methods.First(m => m.Name == "OnAutoSave"));

            //Instead of replacing, let's hook directly in the method
            //            foreach (var method in _asm.MainModule.Types
            //                .SelectMany(x => x.Methods)
            //                .Where(y => y.HasBody))
            //            {
            //                var il = method.Body.GetILProcessor();
            //
            //                foreach (var ins in method.Body.Instructions
            //                    .Where(ins => ins.OpCode == OpCodes.Call && ins.Operand is MethodReference && (ins.Operand as MethodReference).Name == "clearWorld"))
            //                {
            //                    il.Replace(ins, il.Create(OpCodes.Call, replacement));
            //                }
            //            }

            var il = method.Body.GetILProcessor();
            var first = method.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Call, replacement));

            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        /// <summary>
        /// Replaces Main.worldPathName with OTA's
        /// </summary>
        [ServerHook]
        private void PatchInSavePath()
        {
            var method = Terraria.WorldFile.Methods.Single(x => x.Name == "saveWorld" && x.Parameters.Count == 2);
            var replacement = API.WorldFileCallback.Properties.Single(m => m.Name == "SavePath");

            var il = method.Body.GetILProcessor();

            foreach (var ins in method.Body.Instructions)
            {
                if (ins.Operand != null && ins.Operand is MethodReference)
                {
                    var pr = ins.Operand as MethodReference;
                    if (pr.Name == "get_worldPathName")
                    {
                        ins.Operand = _asm.MainModule.Import(replacement.GetMethod);
                    }
                }
            }
        }
    }

    public class TerrariaOrganiser
    {
        private AssemblyDefinition _asm;

        public TerrariaOrganiser(AssemblyDefinition assembly)
        {
            this._asm = assembly;
        }

        public TypeDefinition MessageBuffer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "MessageBuffer"); }
        }

        public TypeDefinition NetMessage
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NetMessage"); }
        }

        public TypeDefinition Main
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Main"); }
        }

        //        public TypeDefinition ProgramServer
        //        {
        //            get
        //            { return _asm.MainModule.Types.Single(x => x.Name == "ProgramServer"); }
        //        }

        /// <summary>
        /// Entry class for windows
        /// </summary>
        /// <value>The windows launch.</value>
        public TypeDefinition WindowsLaunch
        {
            get

            { return _asm.MainModule.Types.Single(x => x.Name == "WindowsLaunch"); }
        }

        /// <summary>
        /// Entry class for Mac
        /// </summary>
        /// <value>The program.</value>
        public TypeDefinition Program
        {
            get

            { return _asm.MainModule.Types.Single(x => x.Name == "Program"); }
        }

        public TypeDefinition NPC
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NPC"); }
        }

        public TypeDefinition WorldFile
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "WorldFile"); }
        }

        public TypeDefinition WorldGen
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "WorldGen"); }
        }

        public TypeDefinition Netplay
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Netplay"); }
        }

        public TypeDefinition Player
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Player"); }
        }

        //        public TypeDefinition ServerSock
        //        {
        //            get
        //            { return _asm.MainModule.Types.Single(x => x.Name == "ServerSock"); }
        //        }

        public TypeDefinition RemoteClient
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "RemoteClient"); }
        }

        public TypeDefinition RemoteServer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "RemoteServer"); }
        }

        public TypeDefinition LaunchInitializer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "LaunchInitializer"); }
        }

        public TypeDefinition Lang
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Lang"); }
        }

        public TypeDefinition Projectile
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Projectile"); }
        }
    }

    public class APIOrganiser
    {
        private AssemblyDefinition _asm;

        public APIOrganiser(AssemblyDefinition assembly)
        {
            this._asm = assembly;
        }

        #region "Callbacks"

        public TypeDefinition BasePlayer
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "BasePlayer"); }
        }

        public TypeDefinition Configuration
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Configuration" && x.Namespace == "OTA.Callbacks"); }
        }

        public TypeDefinition GameWindow
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "GameWindow"); }
        }

        public TypeDefinition IAPISocket
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "IAPISocket"); }
        }

        public TypeDefinition MainCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "MainCallback"); }
        }

        public TypeDefinition MessageBufferCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "MessageBufferCallback"); }
        }

        public TypeDefinition NetMessageCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NetMessageCallback"); }
        }

        public TypeDefinition NetplayCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NetplayCallback"); }
        }

        public TypeDefinition NPCCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NPCCallback"); }
        }

        public TypeDefinition Rand
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Rand"); }
        }

        public TypeDefinition Patches
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Patches"); }
        }

        public TypeDefinition Tools
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Tools"); }
        }

        public TypeDefinition ProgramLog
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "ProgramLog"); }
        }

        public TypeDefinition UserInput
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "UserInput"); }
        }

        public TypeDefinition WorldFileCallback
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "WorldFileCallback"); }
        }

        public TypeDefinition VanillaHooks
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "VanillaHooks"); }
        }

        #endregion

        public TypeDefinition NAT
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "NAT"); }
        }

        public TypeDefinition ClientConnection
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "ClientConnection"); }
            //{ return _asm.MainModule.Types.Single(x => x.Name == "TemporarySynchSock"); }
        }

        public TypeDefinition Utilities
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "Utilities"); }
        }

        public TypeDefinition Player
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "PlayerCallback"); }
        }

        public TypeDefinition WorldSender
        {
            get
            { return _asm.MainModule.Types.Single(x => x.Name == "WorldSender"); }
        }
    }
}
