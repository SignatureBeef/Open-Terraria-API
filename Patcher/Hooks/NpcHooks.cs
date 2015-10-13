using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace OTA.Patcher
{
    public partial class Injector
    {
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

        [ServerHook]
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

        [ServerHook]
        private void HookNpcNetDefaults()
        {
            var setDefaults = Terraria.NPC.Methods.Single(x => x.Name == "netDefaults");

            var apiMatch = API.NPCCallback.MatchInstanceMethodByParameters("Terraria.NPC", setDefaults.Parameters, "OnNetDefaults");

            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching netDefaults Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            setDefaults.Wrap(cbkBegin, cbkEnd);
        }

        [ServerHook]
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

        [ServerHook]
        private void HookNpcTransform()
        {
            var method = Terraria.NPC.Method("Transform");

            var apiMatch = API.NPCCallback.MatchInstanceMethodByParameters("Terraria.NPC", method.Parameters, "OnTransform");

            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnTransform Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            method.Wrap(cbkBegin, cbkEnd, true);
        }

        [ServerHook]
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
            var insColorStart = insColour.FindInstructionByOpCodeBefore(OpCodes.Ldsfld); //Find the first instruction for the color call
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

        [ServerHook]
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
    }
}

