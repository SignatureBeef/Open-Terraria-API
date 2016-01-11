using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using System.Collections.Generic;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [OTAPatch(SupportType.Server, "Hooking NPC kill")]
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

        [OTAPatch(SupportType.ClientServer, "Wrapping NPC.SetDefaults")]
        private void HookNpcSetDefaults()
        {
            var setDefaults = Terraria.NPC.Methods.Where(x => x.Name == "SetDefaults").ToArray();
            foreach (var method in setDefaults)
            {
                var apiMatch = API.NPCCallback.MatchInstanceMethodByParameters("Terraria.NPC", method.Parameters, "OnSetDefault");
                if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching SetDefault Begin/End calls in the API");

                var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
                var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

                method.Wrap(cbkBegin, cbkEnd);
            }
        }

        [OTAPatch(SupportType.ClientServer, "Wrapping NPC.netDefaults")]
        private void HookNpcNetDefaults()
        {
            var setDefaults = Terraria.NPC.Methods.Single(x => x.Name == "netDefaults");

            var apiMatch = API.NPCCallback.MatchInstanceMethodByParameters("Terraria.NPC", setDefaults.Parameters, "OnNetDefaults");

            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching netDefaults Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            setDefaults.Wrap(cbkBegin, cbkEnd);
        }

        [OTAPatch(SupportType.Server, "Hooking NPC strike")]
        private void HookNpcOnStrike()
        {
            var impCall = Terraria.Import(API.NPCCallback.Method("OnStrike"));
            var target = Terraria.NPC.Method("StrikeNPC");

            var il = target.Body.GetILProcessor();

            var first = target.Body.Instructions.First();

            //TODO cecil extension

            //Create our variable, to hold the modified damage
            VariableDefinition vrbResult = null;
            target.Body.Variables.Add(vrbResult = new VariableDefinition("otaResult", (impCall.Parameters[1 /*skip the npc insance*/].ParameterType as ByReferenceType).ElementType));

            //Create the API callback, and return the modified damage variable if cancelled
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0)); //NPC instance
            il.InsertBefore(first, il.Create(OpCodes.Ldloca_S, vrbResult)); //Loads our variable as a reference
            il.InsertBefore(first, il.Create(OpCodes.Call, impCall)); //Call the hook
            //                il.InsertBefore(first, il.Create(OpCodes.Pop));

            //If the hook cancelled the rest of the code, then we can return our modified damage variable
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ldloc, vrbResult));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        [OTAPatch(SupportType.Server, "Wrapping NPC Transform")]
        private void HookNpcTransform()
        {
            var method = Terraria.NPC.Method("Transform");

            var apiMatch = API.NPCCallback.MatchInstanceMethodByParameters("Terraria.NPC", method.Parameters, "OnTransform");

            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnTransform Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            method.Wrap(cbkBegin, cbkEnd, true);
        }

        [OTAPatch(SupportType.Server, "Hooking NPC DropLoot calls")]
        private void HookNpcLoot()
        {
            //Create an empty method with the same parameters as Terraria.Main.NewItem
            //Then with the method body, add the hooks and the actual call to ""
            //Replace all calls to NewItem in NPCLoot

            var npcLoot = Terraria.NPC.Method("NPCLoot");
            var newItem = Terraria.Item.Method("NewItem");

            //Create the new DropLoot call in the Terraria.NPC class
            var dropLoot = new MethodDefinition("DropLoot", MethodAttributes.Public | MethodAttributes.Static, newItem.ReturnType);
            Terraria.NPC.Methods.Add(dropLoot);

            //Clone the parameters
            foreach (var prm in newItem.Parameters)
                dropLoot.Parameters.Add(prm);
            //            //Add the this call to the end
            //            dropLoot.Parameters.Add(new ParameterDefinition("npcId", ParameterAttributes.HasDefault, Terraria.TypeSystem.Int32));

            //Collect the hooks
            var apiMatch = API.NPCCallback.Methods.Where(x => x.Name.StartsWith("OnDropLoot"));

            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnDropLoot Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            //Create the value to hold the new item id
            var il = dropLoot.Body.GetILProcessor();
            var vrbItemId = new VariableDefinition("otaItem", (cbkBegin.Parameters[0].ParameterType as ByReferenceType).ElementType);
            dropLoot.Body.Variables.Add(vrbItemId);

            il.Emit(OpCodes.Ldloca_S, vrbItemId); //Loads our variable as a reference
            var beginResult = dropLoot.InjectBeginCallback(cbkBegin, false, false);

            var insFirstForMethod = dropLoot.InjectMethodCall(newItem, false, false);
            il.Emit(OpCodes.Stloc, vrbItemId);

            //Set the instruction to be resumed upon not cancelling, if not already
            if (beginResult != null && beginResult.OpCode == OpCodes.Pop)
            {
                beginResult.OpCode = OpCodes.Brtrue_S;
                beginResult.Operand = insFirstForMethod;

                il.InsertAfter(beginResult, il.Create(OpCodes.Ret));
                il.InsertAfter(beginResult, il.Create(OpCodes.Ldloc, vrbItemId));
            }

            dropLoot.InjectEndCallback(cbkEnd, false);

            il.Emit(OpCodes.Ldloc, vrbItemId);
            il.Emit(OpCodes.Ret);

            var itemCalls = npcLoot.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
                                && x.Operand is MethodReference
                                && (x.Operand as MethodReference).Name == "NewItem"
                                && (x.Operand as MethodReference).DeclaringType.Name == "Item").ToArray();

            //            var whoAmI = Terraria.Entity.Field("whoAmI");
            foreach (var call in itemCalls)
            {
                call.Operand = dropLoot;
                //                il.InsertBefore(call, il.Create(OpCodes.Ldarg_0));
                //                il.InsertBefore(call, il.Create(OpCodes.Ldfld, whoAmI));
            }


            //This section will add '&& num40 >= 0' to the statement above "Main.item [num40].color = this.color;"
            var insColour = npcLoot.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldfld && x.Operand == Terraria.NPC.Field("color")); //Grab where the call is located
            var insColorStart = insColour.FindPreviousInstructionByOpCode(OpCodes.Ldsfld); //Find the first instruction for the color call
            var resumeInstruction = insColorStart.Previous.Operand as Instruction; //Find the instruction where it should be transferred to if false is evaludated

            il = npcLoot.Body.GetILProcessor();

            //Insert the num40 variable
            il.InsertBefore(insColorStart, il.Create(OpCodes.Ldloc, (VariableDefinition)insColorStart.Next.Operand));
            //Place 0 on the stack
            il.InsertBefore(insColorStart, il.Create(OpCodes.Ldc_I4_0));
            //Compare the current values on stack, using >=
            il.InsertBefore(insColorStart, il.Create(OpCodes.Blt, resumeInstruction));

            npcLoot.Body.OptimizeMacros();

            //            //Add the instance to DropLoot [still kills the stack]
            //            var dropLootCalls = npcLoot.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
            //                                    && x.Operand is MethodReference
            //                                    && (x.Operand as MethodReference).Name == "DropLoot").ToArray();
            //            dropLoot.Parameters.Add(new ParameterDefinition("npc", ParameterAttributes.None, Terraria.NPC)
            //                {
            //                    HasDefault = true,
            //                    IsOptional = true
            //                });
            //
            //            foreach (var call in dropLootCalls)
            //            {
            //                il.InsertBefore(call, il.Create(OpCodes.Ldarg_0));
            //            }
        }

        [OTAPatch(SupportType.Server, "Hooking NPC DropBossBag calls")]
        private void HookDropBossBags()
        {
            //Create an empty method with the same parameters as Terraria.Main.NewItem
            //Then with the method body, add the hooks and the actual call to ""
            //Replace all calls to NewItem in NPCLoot

            var npcLoot = Terraria.NPC.Method("DropBossBags");
            var newItem = Terraria.Item.Method("NewItem");

            //Create the new DropLoot call in the Terraria.NPC class
            var dropLoot = new MethodDefinition("DropBossBagItem", MethodAttributes.Public | MethodAttributes.Static, newItem.ReturnType);
            Terraria.NPC.Methods.Add(dropLoot);

            //Clone the parameters
            foreach (var prm in newItem.Parameters)
                dropLoot.Parameters.Add(prm);
            //            //Add the this call to the end
            //            dropLoot.Parameters.Add(new ParameterDefinition("npcId", ParameterAttributes.HasDefault, Terraria.TypeSystem.Int32));

            //Collect the hooks
            var apiMatch = API.NPCCallback.Methods.Where(x => x.Name.StartsWith("OnDropBossBag"));

            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnDropBossBag Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            //Create the value to hold the new item id
            var il = dropLoot.Body.GetILProcessor();
            var vrbItemId = new VariableDefinition("otaItem", (cbkBegin.Parameters[0].ParameterType as ByReferenceType).ElementType);
            dropLoot.Body.Variables.Add(vrbItemId);

            il.Emit(OpCodes.Ldloca_S, vrbItemId); //Loads our variable as a reference
            var beginResult = dropLoot.InjectBeginCallback(cbkBegin, false, false);

            var insFirstForMethod = dropLoot.InjectMethodCall(newItem, false, false);
            il.Emit(OpCodes.Stloc, vrbItemId);

            //Set the instruction to be resumed upon not cancelling, if not already
            if (beginResult != null && beginResult.OpCode == OpCodes.Pop)
            {
                beginResult.OpCode = OpCodes.Brtrue_S;
                beginResult.Operand = insFirstForMethod;

                il.InsertAfter(beginResult, il.Create(OpCodes.Ret));
                il.InsertAfter(beginResult, il.Create(OpCodes.Ldloc, vrbItemId));
            }

            dropLoot.InjectEndCallback(cbkEnd, false);

            il.Emit(OpCodes.Ldloc, vrbItemId);
            il.Emit(OpCodes.Ret);

            var itemCalls = npcLoot.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
                                && x.Operand is MethodReference
                                && (x.Operand as MethodReference).Name == "NewItem"
                                && (x.Operand as MethodReference).DeclaringType.Name == "Item").ToArray();

            //            var whoAmI = Terraria.Entity.Field("whoAmI");
            foreach (var call in itemCalls)
            {
                call.Operand = dropLoot;
                //                il.InsertBefore(call, il.Create(OpCodes.Ldarg_0));
                //                il.InsertBefore(call, il.Create(OpCodes.Ldfld, whoAmI));
            }


            npcLoot.Body.OptimizeMacros();

            //            //Add the instance to DropLoot [still kills the stack]
            //            var dropLootCalls = npcLoot.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
            //                                    && x.Operand is MethodReference
            //                                    && (x.Operand as MethodReference).Name == "DropLoot").ToArray();
            //            dropLoot.Parameters.Add(new ParameterDefinition("npc", ParameterAttributes.None, Terraria.NPC)
            //                {
            //                    HasDefault = true,
            //                    IsOptional = true
            //                });
            //
            //            foreach (var call in dropLootCalls)
            //            {
            //                il.InsertBefore(call, il.Create(OpCodes.Ldarg_0));
            //            }
        }

        [OTAPatch(SupportType.Server, "Hooking invasion NPC spawning")]
        private void HookSpawnNPC()
        {
            //Create an empty method with the same parameters as Terraria.Main.NewItem
            //Then with the method body, add the hooks and the actual call to ""
            //Replace all calls to NewItem in NPCLoot

            var spawnNPC = Terraria.NPC.Method("SpawnNPC");
            var newNPC = Terraria.Import(API.NPCCallback.Method("OnSpawnInvasionNPC"));

            var itemCalls = spawnNPC.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
                                && x.Operand is MethodReference
                                && (x.Operand as MethodReference).Name == "NewNPC"
                                && (x.Operand as MethodReference).DeclaringType.Name == "NPC").ToArray();
            
            //            var whoAmI = Terraria.Entity.Field("whoAmI");
            foreach (var call in itemCalls)
            {
                call.Operand = newNPC;
                //                il.InsertBefore(call, il.Create(OpCodes.Ldarg_0));
                //                il.InsertBefore(call, il.Create(OpCodes.Ldfld, whoAmI));
            }

//            //Create the new DropLoot call in the Terraria.NPC class
//            var invasionNPC = new MethodDefinition("SpawnInvasionNPC", spawnNPC.Attributes, newNPC.ReturnType);
//            Terraria.NPC.Methods.Add(invasionNPC);
//
//            //Clone the parameters
//            foreach (var prm in newNPC.Parameters)
//                invasionNPC.Parameters.Add(prm);
//            //            //Add the this call to the end
//            //            dropLoot.Parameters.Add(new ParameterDefinition("npcId", ParameterAttributes.HasDefault, Terraria.TypeSystem.Int32));
//
//            //Collect the hooks
//            var apiMatch = API.NPCCallback.Methods.Where(x => x.Name.StartsWith("OnSpawnInvasionNPC"));
//
//            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnDropBossBag Begin/End calls in the API");
//
//            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
//            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));
//
//            //Create the value to hold the new item id
//            var il = invasionNPC.Body.GetILProcessor();
//            var vrbItemId = new VariableDefinition("otaItem", (cbkBegin.Parameters[0].ParameterType as ByReferenceType).ElementType);
//            invasionNPC.Body.Variables.Add(vrbItemId);
//
//            il.Emit(OpCodes.Ldloca_S, vrbItemId); //Loads our variable as a reference
//            var beginResult = invasionNPC.InjectBeginCallback(cbkBegin, false, false);
//
//            var insFirstForMethod = invasionNPC.InjectMethodCall(newNPC, false, false);
//            il.Emit(OpCodes.Stloc, vrbItemId);
//
//            //Set the instruction to be resumed upon not cancelling, if not already
//            if (beginResult != null && beginResult.OpCode == OpCodes.Pop)
//            {
//                beginResult.OpCode = OpCodes.Brtrue_S;
//                beginResult.Operand = insFirstForMethod;
//
//                il.InsertAfter(beginResult, il.Create(OpCodes.Ret));
//                il.InsertAfter(beginResult, il.Create(OpCodes.Ldloc, vrbItemId));
//            }
//
//            invasionNPC.InjectEndCallback(cbkEnd, false);
//
//            il.Emit(OpCodes.Ldloc, vrbItemId);
//            il.Emit(OpCodes.Ret);
//
//            var itemCalls = spawnNPC.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
//                                && x.Operand is MethodReference
//                                && (x.Operand as MethodReference).Name == "NewNPC"
//                                && (x.Operand as MethodReference).DeclaringType.Name == "NPC").ToArray();
//
//            //            var whoAmI = Terraria.Entity.Field("whoAmI");
//            foreach (var call in itemCalls)
//            {
//                call.Operand = invasionNPC;
//                //                il.InsertBefore(call, il.Create(OpCodes.Ldarg_0));
//                //                il.InsertBefore(call, il.Create(OpCodes.Ldfld, whoAmI));
//            }
//
//
//            spawnNPC.Body.OptimizeMacros();
        }

        [OTAPatch(SupportType.ClientServer, "Hooking NPC creation")]
        private void HookNPCCreation()
        {
            var method = Terraria.NPC.Method("NewNPC");
            var replacement = API.NPCCallback.Method("OnNewNpc");

            var ctor = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Newobj
                           && x.Operand is MethodReference
                           && (x.Operand as MethodReference).DeclaringType.Name == "NPC");

            ctor.OpCode = OpCodes.Call;
            ctor.Operand = Terraria.Import(replacement);

            //Remove <npc>.SetDefault() as we do something custom
            var remFrom = ctor.Next;
            var il = method.Body.GetILProcessor();
            while (remFrom.Next.Next.OpCode != OpCodes.Call) //Remove until TypeToNum
            {
                il.Remove(remFrom.Next);
            }

//            //Add Type to our callback
//            il.InsertBefore(ctor, il.Create(OpCodes.Ldarg_2));

            il.InsertBefore(ctor, il.Create(OpCodes.Ldloc_0));
            foreach (var prm in method.Parameters)
            {
                il.InsertBefore(ctor, il.Create(OpCodes.Ldarg, prm));
            }
        }

        [OTAPatch(SupportType.Client, "Hooking Npc Updating")]
        private void HookNpcUpdate()
        {
            var method = Terraria.NPC.Method("UpdateNPC");

            var apiMatch = API.NPCCallback.Methods.Where(x => x.Name.StartsWith("OnUpdateNPC"));
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnInitialise Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            method.Wrap(cbkBegin, cbkEnd, true);
        }

        [OTAPatch(SupportType.Client, "Hooking Npc AI")]
        private void HookNpcAI()
        {
            var method = Terraria.NPC.Method("AI");

            var apiMatch = API.NPCCallback.Methods.Where(x => x.Name.StartsWith("OnAI"));
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnAI Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            method.Wrap(cbkBegin, cbkEnd, true);
        }

        [OTAPatch(SupportType.Client, "Hooking Npc FindFrame")]
        private void HookNpcFindFrame()
        {
            var method = Terraria.NPC.Method("FindFrame");

            var apiMatch = API.NPCCallback.Methods.Where(x => x.Name.StartsWith("OnFindFrame"));
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnFindFrame Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            method.Wrap(cbkBegin, cbkEnd, true);
        }

        [OTAPatch(SupportType.Client, "Altering NPC.FindFrame", 50)]
        private void HookAlterNpcFindFrame()
        {
            var method = Terraria.NPC.Method("FindFrame");

            //Include a IsTownNPC flag
            var isTownNpc = new FieldDefinition("IsTownNpc", FieldAttributes.Public, Terraria.TypeSystem.Boolean);
            Terraria.NPC.Fields.Add(isTownNpc);

            var typeCmp = method.Body.Instructions.First(x => x.OpCode == OpCodes.Ldfld
                              && x.Operand is FieldReference
                              && (x.Operand as FieldReference).Name == "type"
                              && x.Next.OpCode == OpCodes.Ldc_I4
                              && x.Next.Operand.Equals(338));

            var il = method.Body.GetILProcessor();

            //Allow FindFrame to determine if the Npc is a town npc

            il.InsertBefore(typeCmp, il.Create(OpCodes.Ldfld, isTownNpc));
            il.InsertBefore(typeCmp, il.Create(OpCodes.Ldc_I4_1));
            il.InsertBefore(typeCmp, il.Create(OpCodes.Beq, typeCmp.Next.Next.Operand as Instruction));
            il.InsertBefore(typeCmp, il.Create(OpCodes.Ldarg_0));
        }

        [OTAPatch(SupportType.Client, "Hooking NPC Chat")]
        private void HookNpcGetChat()
        {
            var method = Terraria.NPC.Method("GetChat");
            var call = API.NPCCallback.Method("OnGetChat");

            method.ReplaceInstanceMethod(call);
        }

        [OTAPatch(SupportType.Client, "Hooking NPC pre spawn")]
        private void HookNPCPreSpawn()
        {
            var method = Terraria.NPC.Method("SpawnNPC");
            var call = Terraria.Import(API.NPCCallback.Method("OnPreSpawn"));

            //The insertion point is located above the second player.ZoneTowerNebula
            var insEntry = method.Body.Instructions.Where(x => x.OpCode == OpCodes.Callvirt
                               && x.Operand is MethodReference
                               && (x.Operand as MethodReference).Name == "get_ZoneTowerNebula")
                .Skip(1)
                .First()
                .FindPreviousInstructionByOpCode(OpCodes.Ldsfld);

            var il = method.Body.GetILProcessor();

            var insCall = method.InsertCancellableMethodBefore(insEntry, call);

            var localVariables = method.Body.Instructions
                .Where(x => (new OpCode []
                { 
                    OpCodes.Ldloc,
//                    OpCodes.Ldloca,
                    OpCodes.Ldloc_S,
                    OpCodes.Ldloca_S,

                    OpCodes.Stloc,
                    OpCodes.Stloc_S
                }).Contains(x.OpCode)
                                     && x.Offset < insEntry.Offset)
                .Select(y => y.Operand as VariableDefinition)


                .Union(
                                     method.Body.Instructions
                    .Where(x => (new OpCode []
                    { 
                        OpCodes.Ldloc_0,
                        OpCodes.Stloc_0
                    }).Contains(x.OpCode)
                                         && x.Offset < insEntry.Offset)
                    .Select(y => method.Body.Variables[0])
                                 )

                .Union(
                                     method.Body.Instructions
                .Where(x => (new OpCode []
                    { 
                        OpCodes.Ldloc_1,
                        OpCodes.Stloc_1
                    }).Contains(x.OpCode)
                                         && x.Offset < insEntry.Offset)
                    .Select(y => method.Body.Variables[1])
                                 )

                .Union(
                                     method.Body.Instructions
                    .Where(x => (new OpCode []
                    { 
                        OpCodes.Ldloc_2,
                        OpCodes.Stloc_2
                    }).Contains(x.OpCode)
                                         && x.Offset < insEntry.Offset)
                    .Select(y => method.Body.Variables[2])
                                 )

                .Union(
                                     method.Body.Instructions
                    .Where(x => (new OpCode []
                    { 
                        OpCodes.Ldloc_3,
                        OpCodes.Stloc_3
                    }).Contains(x.OpCode)
                                         && x.Offset < insEntry.Offset)
                    .Select(y => method.Body.Variables[3])
                                 )

                .Distinct()
                .OrderBy(z => z.Index);

            var variables = localVariables.OrderBy(x => x.VariableType.ToString()).ToArray();

//            int prmIdx = 0;
//            foreach (var local in variables)
//            {
//                if (prmIdx > 0) Console.Write(",\n");
//                Console.Write("{0} prm{1}", local.VariableType, prmIdx++);
////                Console.Write("/*" + local.Value.Name + "*/");
//            }

            foreach (var pair in variables)
            {
                /*if (pair.Value == OpCodes.Ldloc_0)
                {
                    il.InsertBefore(insCall, il.Create(OpCodes.Ldloc_0));
                }
                else if (pair.Value == OpCodes.Ldloc_1)
                {
                    il.InsertBefore(insCall, il.Create(OpCodes.Ldloc_1));
                }
                else if (pair.Value == OpCodes.Ldloc_2)
                {
                    il.InsertBefore(insCall, il.Create(OpCodes.Ldloc_2));
                }
                else*/

                il.InsertBefore(insCall, il.Create(OpCodes.Ldloc, pair));
            }
        }

        #if CLIENT
        [OTAPatch(SupportType.Client, "Hooking Npc saving")]
        private void HookNpcSaving()
        {
            var method = Terraria.WorldFile.Method("SaveWorldHeader");
            var field = Terraria.Import(API.NpcModRegister.Field("MaxNpcId"));

            var npcCount = method.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Ldc_I4 && x.Operand.Equals(540))
                .ToArray();

            foreach (var npc in npcCount)
            {
                npc.OpCode = OpCodes.Ldsfld;
                npc.Operand = field;
            }
        }
        #endif
    }
}

