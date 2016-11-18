using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
    public class Spawn : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.1, Culture=neutral, PublicKeyToken=null"
		};
        public override string Description => "Hooking npc spawning...";

        public override void Run()
        {
            var vanilla = SourceDefinition.Type("Terraria.NPC").Method("NewNPC");
            int tmp = 0;
            var callback = this.Method(() => OTAPI.Callbacks.Terraria.Npc.Spawn(ref tmp));
            var importedCallback = SourceDefinition.MainModule.Import(callback);

            //This hook is to notify when a npc has been created, and is about to be spawned into
            //the world.
            //What we do is look in Terraria.NPC.NewNPC for the following code:
            //      Main.npc[num].target = Target;
            //      if (Type == 50)
            //
            //Between these two lines we inject a cancelable callback:
            //      Main.npc[num].target = Target;
            //      if(!OTAPI.Callbacks.Terraria.Npc.Spawn(ref num))
            //      {
            //          return index;
            //      }
            //      if (Type == 50)

            //Find the `if (Type == 50) offset instruction so we can use it
            //as our insertion point
            var insInsertionPoint = vanilla.Body.Instructions.Single(i =>
                i.OpCode == OpCodes.Ldarg_2
                && i.Next.OpCode == OpCodes.Ldc_I4_S
                && i.Next.Operand.Equals((sbyte)50)
                && i.Next.Next.OpCode == OpCodes.Bne_Un_S
            );

            //Get the il processor so we can alter il
            var processor = vanilla.Body.GetILProcessor();

            //Find the npc index, using the only ret instruction that doesn't
            //have an operand
            var insNpcIndex = vanilla.Body.Instructions.Single(i =>
                i.OpCode == OpCodes.Ret
                && i.Previous != null
                && i.Previous.Operand == null
            );

            //Expect ldloc.0 (as we will use it by reference later)
            if (insNpcIndex.Previous.OpCode != OpCodes.Ldloc_0)
                throw new NotImplementedException($"Expected ldloc.0 rather than {insNpcIndex.Previous.OpCode}");

            //Inject out callback il
            processor.InsertBefore(insInsertionPoint,
                //Load the npc index by reference
                new { OpCodes.Ldloca, Operand = vanilla.Body.Variables.First() },
                //Invoke our callback using the npc index that is on the stack as a reference
                new { OpCodes.Call, importedCallback },
                //If the callback returns true, then continue back to vanilla
                new { OpCodes.Brtrue_S, insInsertionPoint },
                //Otherwise, if false then return the modified npc index
                new { OpCodes.Ldloc_0 },
                new { OpCodes.Ret }
            );
        }
    }
}
