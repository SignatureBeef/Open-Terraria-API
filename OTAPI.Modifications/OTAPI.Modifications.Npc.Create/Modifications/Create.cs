using Mono.Cecil;
using Mono.Cecil.Cil;
using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class Create : ModificationBase
	{
		public override string Description => "Hooking Npc.NewNPC...";

		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.NPC").Method("NewNPC");
			var callback = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Npc").Method("Create");


			var ctor = vanilla.Body.Instructions.Single(x => x.OpCode == OpCodes.Newobj
						   && x.Operand is MethodReference
						   && (x.Operand as MethodReference).DeclaringType.Name == "NPC");

			ctor.OpCode = OpCodes.Call;
			ctor.Operand = vanilla.Module.Import(callback);

			//Remove <npc>.SetDefault() as we do something custom
			var remFrom = ctor.Next;
			var il = vanilla.Body.GetILProcessor();
			while (remFrom.Next.Next.OpCode != OpCodes.Call) //Remove until TypeToNum
			{
				il.Remove(remFrom.Next);
			}

			//            //Add Type to our callback
			//            il.InsertBefore(ctor, il.Create(OpCodes.Ldarg_2));

			il.InsertBefore(ctor, il.Create(OpCodes.Ldloca, vanilla.Body.Variables.First())); //The first variable is the index
			for (var x = 0; x < vanilla.Parameters.Count; x++)
			{
				var opcode = callback.Parameters[x].ParameterType.IsByReference ? OpCodes.Ldarga : OpCodes.Ldarg;
				il.InsertBefore(ctor, il.Create(opcode, vanilla.Parameters[x]));
			}
		}
	}
}
