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
                                    && x.Next.OpCode == OpCodes.Brfalse
                                    && x.Offset > mouseLeftRelease.Offset);
            var npcChatFocus2 = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldsfld
                                    && x.Operand is FieldReference
                                    && (x.Operand as FieldReference).Name == "npcChatFocus2"
                                    && x.Next.OpCode == OpCodes.Brfalse
                                    && x.Offset > mouseLeftRelease.Offset);
            var npcChatFocus3 = method.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldsfld
                                    && x.Operand is FieldReference
                                    && (x.Operand as FieldReference).Name == "npcChatFocus3"
                                    && x.Next.OpCode == OpCodes.Brfalse
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
    }
}

