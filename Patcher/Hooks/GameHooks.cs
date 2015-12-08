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
    }
}

