using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace OTA.Patcher
{
    public partial class Injector
    {
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
            var addition = API.MainCallback.Methods.First(m => m.Name == "OnServerTick");

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

        [ServerHook]
        private void FixNetplayServerFull()
        {
            var method = Terraria.Netplay.Methods.Single(x => x.Name == "OnConnectionAccepted");
            var callback = API.NetplayCallback.Methods.Single(m => m.Name == "OnServerFull");

            var toReplace = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Call
                                && x.Operand is MethodReference
                                && (x.Operand as MethodReference).Name == "StopListening");

            toReplace.Operand = _asm.MainModule.Import(callback);

            var il = method.Body.GetILProcessor();
            il.InsertBefore(toReplace, il.Create(OpCodes.Ldarg_0));
        }

        [ServerHook]
        private void NurfWorldMap() //wip, dumped a lot of code in place of cecil extensions
        {
            var wm = _asm.MainModule.Types.Single(x => x.Name == "WorldMap");

            foreach (var method in wm.Methods)
            {
                //if (method.ReturnType.Name == "Boolean" || method.ReturnType.Name == "Void")
                method.EmptyInstructions();
            }

            foreach (var mth in new string[] { "checkMap", "DrawMap", "DrawToMap", "DrawToMap_Section" })
            {
                var method = Terraria.Main.Methods.Single(x => x.Name == mth);

                method.EmptyInstructions();
            }

            //            _asm.MainModule.Types.Remove(wm);
            //            Terraria.Main.Fields.Remove(Terraria.Main.Fields.Single(x => x.Name == "Map"));
        }

        //[ServerHook]
        //void SwapArchitecture()
        //{
        //    _asm.MainModule.Architecture = TargetArchitecture.AMD64; //Any CPU
        //}

        /// <summary>
        /// Wraps Terraria.Main.Update with calls to OTA.Callbacks.MainCallback.OnUpdate[Begin|End]
        /// </summary>
        [ServerHook]
        void HookUpdate()
        {
            //Grab the Update method
            var updateServer = Terraria.Main.Methods.Single(x => x.Name == "Update");
            //Wrap it with the API calls
            updateServer.InjectBeginEnd(API.MainCallback, "OnUpdate");
        }

        /// <summary>
        /// Wraps Terraria.Main.UpdateServer with calls to OTA.Callbacks.MainCallback.OnUpdateServer[Begin|End]
        /// </summary>
        [ServerHook]
        void HookUpdateServer()
        {
            //Grab the UpdateServer method
            var updateServer = Terraria.Main.Methods.Single(x => x.Name == "UpdateServer");
            //Wrap it with the API calls
            updateServer.InjectBeginEnd(API.MainCallback, "OnUpdateServer");
        }

        [ServerHook]
        void HookWorldSave()
        {
            var method = Terraria.WorldFile.Methods.Single(x => x.Name == "saveWorld" && x.Parameters.Count == 2);

            var apiMatch = API.WorldFileCallback.MatchMethodByParameters(method.Parameters, "OnWorldSave");

            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnWorldSave Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            method.Wrap(cbkBegin, cbkEnd, true);
        }

        [ServerHook]
        void HookStartHardMode()
        {
            var method = Terraria.WorldGen.Method("StartHardmode");
            var callback = Terraria.Import(API.WorldGenCallback.Method("OnStartHardMode"));

            var insHardMode = method.Body.Instructions.First(x => x.OpCode == OpCodes.Stsfld && x.Operand == Terraria.Main.Field("hardMode"));
            var insInsertBefore = insHardMode.Previous;

            var il = method.Body.GetILProcessor();

            Instruction insFirst = null;
            il.InsertBefore(insInsertBefore, insFirst = il.Create(OpCodes.Call, callback));
            insInsertBefore.ReplaceTransfer(insFirst, method);
            il.InsertBefore(insInsertBefore, il.Create(OpCodes.Brtrue_S, insInsertBefore));
            il.InsertBefore(insInsertBefore, il.Create(OpCodes.Ret));
        }

        [ServerHook]
        void HookChristmas()
        {
            var method = Terraria.Main.Method("checkXMas");
            var callback = Terraria.Import(API.MainCallback.Method("OnChristmasCheck"));

            var insLastLdcI40 = method.Body.Instructions.Last(x => x.OpCode == OpCodes.Ldc_I4_0);
            var il = method.Body.GetILProcessor();

            il.Replace(insLastLdcI40, il.Create(OpCodes.Call, callback));
        }

        [ServerHook]
        void HookHalloween()
        {
            var method = Terraria.Main.Method("checkHalloween");
            var callback = Terraria.Import(API.MainCallback.Method("OnHalloweenCheck"));

            var insLastLdcI40 = method.Body.Instructions.Last(x => x.OpCode == OpCodes.Ldc_I4_0);
            var il = method.Body.GetILProcessor();

            il.Replace(insLastLdcI40, il.Create(OpCodes.Call, callback));
        }

        /// <summary>
        /// When vanilla requests to ban a slot, this method is called.
        /// </summary>
        [ServerHook]
        void WrapAddBan()
        {
            var vanilla = Terraria.Netplay.Methods.Single(x => x.Name == "AddBan");

            var apiMatch = API.NetplayCallback.Methods.Where(x => x.Name.StartsWith("OnAddBan"));

            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnAddBan Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}

