﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class Create : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.4.1, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.NewNPC...";

		public override void Run()
		{
			return;
			int tmpI = 0;
			float tmpF = 0;
			var vanilla = this.Method(() => Terraria.NPC.NewNPC(null, 0, 0, 0, 0, 0, 0, 0, 0, 0));
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.Npc.Create(
				ref tmpI, ref tmpI, ref tmpI, ref tmpI, ref tmpI, ref tmpF, ref tmpF, ref tmpF, ref tmpF, ref tmpI
			));

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
