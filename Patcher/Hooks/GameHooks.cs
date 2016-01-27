using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [OTAPatch(SupportType.ClientServer, "Wrapping Main.Initialize")]
        private void HookMainInitialize()
        {
            var target = Terraria.Main.Method("Initialize");

            var apiMatch = API.MainCallback.Methods.Where(x => x.Name.StartsWith("OnInitialise"));
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnInitialise Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            target.Wrap(cbkBegin, cbkEnd);
        }

        [OTAPatch(SupportType.Client, "Hooking Npc Drawing")]
        private void HookNpcDraw()
        {
            var method = Terraria.Main.Method("DrawNPC");

            var apiMatch = API.NPCCallback.Methods.Where(x => x.Name.StartsWith("OnDrawNPC"));
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching OnDrawNPC Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            method.Wrap(cbkBegin, cbkEnd, true);
        }

        #if CLIENT
        [OTAPatch(SupportType.Client, "Allowing custom NPC's to be drawn")]
        private void AllowMoreNPCDrawing()
        {
            var method = Terraria.Main.Method("DrawNPCs");
            var replacement = API.NpcModRegister.Field("MaxNpcId");

            var ldLoc_540 = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldc_I4 && x.Operand.Equals(540));

            ldLoc_540.OpCode = OpCodes.Ldsfld;
            ldLoc_540.Operand = Terraria.Import(replacement);
        }
        #endif

        [OTAPatch(SupportType.Client, "Hooking Npc texture loading")]
        private void HookNpcLoad()
        {
            var method = Terraria.Main.Method("LoadNPC");
            var callback = API.MainCallback.Method("OnLoadNPC");

            method.Wrap(callback, beginIsCancellable: true);
        }

        [OTAPatch(SupportType.Client, "Hooking Npc chat buttons")]
        private void HookNpcGetChatButtons()
        {
            var method = Terraria.Main.Method("GUIChatDrawInner");
            var callback = Terraria.Import(API.NPCCallback.Method("OnGetChatButtons"));

            var il = method.Body.GetILProcessor();

            //Find the first instance of [MeasureString].
            //Cycle back to [int num19 = 180 + (Main.screenWidth - 800) / 2;] (look for concat)
            //Inject our callback with references to text,text2

            var measureString = method.Body.Instructions.First(x => x.OpCode == OpCodes.Callvirt
                                    && x.Operand is MethodReference
                                    && (x.Operand as MethodReference).Name == "MeasureString");

            var insertionPoint = measureString.FindPreviousInstructionByOpCode(OpCodes.Call).Next.Next;

            var cb = il.Create(OpCodes.Call, callback);
            il.InsertBefore(insertionPoint, cb);

            //Find the variable references to text,text2
            //It appears they are both initialised before [statLifeMax2]
            var statLifeMax2 = method.Body.Instructions.First(x => x.OpCode == OpCodes.Ldfld
                                   && x.Operand is FieldReference
                                   && (x.Operand as FieldReference).Name == "statLifeMax2");

            var text2 = statLifeMax2.FindPreviousInstructionByOpCode(OpCodes.Stloc_S);
            var text = text2.FindPreviousInstructionByOpCode(OpCodes.Stloc_S);

            Instruction newTarget;
            il.InsertBefore(cb, newTarget = il.Create(OpCodes.Ldloca_S, text.Operand as VariableDefinition));
            il.InsertBefore(cb, il.Create(OpCodes.Ldloca_S, text2.Operand as VariableDefinition));


            //When injecting our call, we are now the new receiver from other blocks!
            foreach (var ins in method.Body.Instructions.Where(x => x.Operand == insertionPoint))
                ins.Operand = newTarget;
        }

        [OTAPatch(SupportType.Client, "Hooking Npc chat buttons events")]
        private void HookNpcChatButtonsClick()
        {
            var method = Terraria.Main.Method("GUIChatDrawInner");
            var callback = Terraria.Import(API.NPCCallback.Method("OnChatButtonClicked"));

            var il = method.Body.GetILProcessor();

            //Find the last Main.mouseLeftRelease, the following npcChatFocus[1,2,3] are the targets

            var mouseLeftRelease = method.Body.Instructions.Last(x => x.OpCode == OpCodes.Stsfld
                                       && x.Operand is FieldReference
                                       && (x.Operand as FieldReference).Name == "mouseLeftRelease");


            var npcChatFocus1 = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldsfld
                                    && x.Operand is FieldReference
                                    && (x.Operand as FieldReference).Name == "npcChatFocus1"
                                    && (x.Next.OpCode == OpCodes.Brfalse/*Mac*/ || x.Next.OpCode == OpCodes.Brfalse_S/*Windows*/)
                                    && x.Offset > mouseLeftRelease.Offset);
            var npcChatFocus2 = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldsfld
                                    && x.Operand is FieldReference
                                    && (x.Operand as FieldReference).Name == "npcChatFocus2"
                                    && (x.Next.OpCode == OpCodes.Brfalse/*Mac*/ || x.Next.OpCode == OpCodes.Brfalse_S/*Windows*/)
                                    && x.Offset > mouseLeftRelease.Offset);
            var npcChatFocus3 = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldsfld
                                    && x.Operand is FieldReference
                                    && (x.Operand as FieldReference).Name == "npcChatFocus3"
                                    && (x.Next.OpCode == OpCodes.Brfalse/*Mac*/ || x.Next.OpCode == OpCodes.Brfalse_S/*Windows*/)
                                    && x.Offset > mouseLeftRelease.Offset);

            var ins3 = npcChatFocus3.Next.Operand as Instruction;
            var ins2 = npcChatFocus2.Next.Operand as Instruction;
            var ins1 = npcChatFocus1.Next.Operand as Instruction;

            il.InsertAfter(npcChatFocus3.Next, il.Create(OpCodes.Brfalse, ins3));
            il.InsertAfter(npcChatFocus3.Next, il.Create(OpCodes.Call, callback));

            il.InsertAfter(npcChatFocus2.Next, il.Create(OpCodes.Brfalse, ins2));
            il.InsertAfter(npcChatFocus2.Next, il.Create(OpCodes.Call, callback));

            il.InsertAfter(npcChatFocus1.Next, il.Create(OpCodes.Brfalse, ins1));
            il.InsertAfter(npcChatFocus1.Next, il.Create(OpCodes.Call, callback));
        }

        [OTAPatch(SupportType.ClientServer, "Hooking INativeMod")]
        private void HookInINativeMod()
        {
            //Rather than tracking what MOD object is attatched to what vanilla object
            //It's easier to just attatch a new "Mod" field.
            foreach (var type in new[] {
                Terraria.Chest,
                Terraria.Item,
                Terraria.NPC,
                Terraria.Projectile
                //Terraria.Tile
            })
            {
                type.Fields.Add(new FieldDefinition("Mod", FieldAttributes.Public, Terraria.Import(API.INativeMod)));
            }
        }

        #if CLIENT
        [OTAPatch(SupportType.Client, "Hooking chat box")]
        private void HookChatBox()
        {
            var method = Terraria.Main.Method("Update");

            var startInstruction = method.Body.Instructions.Single(
                                       x => x.OpCode == OpCodes.Ldc_I4_S && x.Operand.Equals((sbyte)13)
                                       && x.Next.Next.Next.OpCode == OpCodes.Ldsfld
                                       && x.Next.Next.Next.Operand is FieldReference
                                       && (x.Next.Next.Next.Operand as FieldReference).Name == "netMode"
                                   ).Previous;

            startInstruction.OpCode = OpCodes.Call;
            startInstruction.Operand = Terraria.Import(API.InterfaceCallback.Method("CanOpenChat"));

            var il = method.Body.GetILProcessor();

            while (!(startInstruction.Next.Next.OpCode == OpCodes.Ldsfld
                   && startInstruction.Next.Next.Operand is FieldReference
                   && (startInstruction.Next.Next.Operand as FieldReference).Name == "chatRelease"))
            {
                il.Remove(startInstruction.Next);
            }
        }

        [OTAPatch(SupportType.Client, "Hooking chat box text")]
        private void HookChatText()
        {
            var method = Terraria.Main.Method("Update");
            var callback = Terraria.Import(API.InterfaceCallback.Method("OnChatTextSend"));

            var insEntry = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldsfld
                               && x.Operand is FieldReference
                               && (x.Operand as FieldReference).Name == "chatRelease"

                               && x.Previous.Previous.OpCode == OpCodes.Ldsfld
                               && x.Previous.Previous.Operand is FieldReference
                               && (x.Previous.Previous.Operand as FieldReference).Name == "inputTextEnter"
                           ).Next;

            var il = method.Body.GetILProcessor();

            il.InsertAfter(insEntry, il.Create(OpCodes.Brfalse, insEntry.Operand as Instruction));
            il.InsertAfter(insEntry, il.Create(OpCodes.Call, callback));
        }
        #endif

        [OTAPatch(SupportType.ClientServer, "Replacing XNA SetTitle")]
        private void ReplaceSetTitle()
        {
            var callback = Terraria.Import(API.MainCallback.Method("SetWindowTitle"));

            var instructions = Terraria.Types
                .Where(a => a.HasMethods)
                .SelectMany(x => x.Methods)
                .Where(b => b.HasBody && b.Body.Instructions != null)
                .Select(y => new MethodInstructions()
                {
                    Method = y,
                    Instructions = y.Body.Instructions
                        .Where(z => z.Operand is MethodReference
                        && (z.Operand as MethodReference).Name == "set_Title")
                            .ToArray()
                }
                               ).ToArray();
            foreach (var mth in instructions)
            {
                var il = mth.Method.Body.GetILProcessor();
                foreach (var ins in mth.Instructions)
                {
                    var mr = ins.Operand as MethodReference;
                    if (mr.HasThis)
                    {
                        var prev = ins.FindPreviousInstructionByOpCode(OpCodes.Call);
                        il.Remove(prev.Previous);
                        il.Remove(prev);
                    }

                    ins.Operand = callback;
                    ins.OpCode = OpCodes.Call;
                }
            }
        }

        [OTAPatch(SupportType.ClientServer, "Making entities virtual", 2000/*
            This is a requirement to be last, as every method may be changed and it will ruin other IL
        */
        )] 
        private void MakeEntitiesVirtual()
        {
            foreach (var type in new [] 
                {
                    Terraria.NPC,
                    Terraria.Projectile,
                    Terraria.Item
                })
            {
                //Make everything virtual
                foreach (var method in type.Methods.Where(x=> !x.IsStatic 
                    && !x.IsGetter 
                    && !x.IsSetter 
                    && x.IsPublic 
                    && !x.IsVirtual 
                    && !x.IsNewSlot
                    && x.Overrides.Count == 0
                    && x.Name != ".ctor"
                    && x.Name != ".cctor"))
                {
                    method.IsVirtual = true;
                    method.IsNewSlot = true;

                    var instructions = Terraria.Types
                        .Where(a => a.HasMethods)
                        .SelectMany(x => x.Methods)
                        .Where(b => b.HasBody && b.Body.Instructions != null)
                        .SelectMany(y => y.Body.Instructions)
                        .Where(z => z.Operand == method);

                    foreach (var ins in instructions)
                    {
                        if (ins.OpCode == OpCodes.Callvirt) break;

                        if (ins.OpCode == OpCodes.Call)
                        {
                            ins.OpCode = OpCodes.Callvirt;
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                }
            }
        }

        //        [OTAPatch(SupportType.ClientServer, "Test")]
        //        private void Test()
        //        {
        //            var methods = _asm.MainModule.Types
        //                .SelectMany(t => t.Methods)
        //                .Where(m => m.HasBody);
        //
        //            var field = Terraria.Import(API.NpcModRegister.Field("MaxNpcId"));
        //
        //            foreach (var mth in methods)
        //            {
        //                var ins = mth.Body.Instructions
        //                    .Where(x => x.Operand != null && x.Operand.Equals(540)).ToArray();
        //
        //                if (ins.Length > 0)
        //                {
        //                    Console.WriteLine(mth.FullName);
        ////                    var asd = "";
        //                    foreach (var npc in ins)
        //                    {
        //                        npc.OpCode = OpCodes.Ldsfld;
        //                        npc.Operand = field;
        //                    }
        //                }
        //            }
        //        }

        private struct MethodInstructions
        {
            public MethodDefinition Method;
            public Instruction[] Instructions;
        }

        #if CLIENT
        [OTAPatch(SupportType.Client, "Hooking menu drawing")] 
        private void Test()
        {
            var method = Terraria.Main.Method("DrawMenu");
            var callback = Terraria.Import(API.InterfaceCallback.Method("OnDrawMenu"));

            //Find the last 888 (ldc.i4) call. We must inject before this if block
            var last888 = method.Body.Instructions.Last(x => x.OpCode == OpCodes.Ldc_I4 && x.Operand.Equals(888));


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
                                     && x.Offset < last888.Offset)
                .Select(y => y.Operand as VariableDefinition)


                .Union(
                                     method.Body.Instructions
                    .Where(x => (new OpCode []
                    { 
                        OpCodes.Ldloc_0,
                        OpCodes.Stloc_0
                    }).Contains(x.OpCode)
                                         && x.Offset < last888.Offset)
                    .Select(y => method.Body.Variables[0])
                                 )

                .Union(
                                     method.Body.Instructions
                    .Where(x => (new OpCode []
                    { 
                        OpCodes.Ldloc_1,
                        OpCodes.Stloc_1
                    }).Contains(x.OpCode)
                                         && x.Offset < last888.Offset)
                    .Select(y => method.Body.Variables[1])
                                 )

                .Union(
                                     method.Body.Instructions
                    .Where(x => (new OpCode []
                    { 
                        OpCodes.Ldloc_2,
                        OpCodes.Stloc_2
                    }).Contains(x.OpCode)
                                         && x.Offset < last888.Offset)
                    .Select(y => method.Body.Variables[2])
                                 )

                .Union(
                                     method.Body.Instructions
                    .Where(x => (new OpCode []
                    { 
                        OpCodes.Ldloc_3,
                        OpCodes.Stloc_3
                    }).Contains(x.OpCode)
                                         && x.Offset < last888.Offset)
                    .Select(y => method.Body.Variables[3])
                                 )

                .Distinct()
                .OrderBy(z => z.Index);
            var variables = localVariables.OrderBy(x => x.VariableType.ToString()).ToArray();

//            int prmIdx = 0;
//            foreach (var local in variables)
//            {
//                if (prmIdx > 0) Console.Write(", //\n");
//                Console.Write("ref {0} prm{1}", local.VariableType, prmIdx++);
//                //                Console.Write("/*" + local.Name + "*/");
//            }

            var il = method.Body.GetILProcessor();
            Instruction insCall;

            il.InsertBefore(last888.Previous, insCall = il.Create(OpCodes.Call, callback));

            Instruction first = null;
            foreach (var pair in variables)
            {
                var vrb = il.Create(OpCodes.Ldloca_S, pair);
                if (first == null)
                {
                    first = vrb;
                }
                il.InsertBefore(insCall, vrb);
            }
            last888.Previous.ReplaceTransfer(first, method);
        }
        #endif
    }
}

