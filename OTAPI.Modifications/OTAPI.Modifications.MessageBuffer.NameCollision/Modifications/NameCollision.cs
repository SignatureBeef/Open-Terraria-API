using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net.Player
{
	/// <summary>
	/// Injects a hook into the if statement that surrounds the name collision kick.
	/// </summary>
	public class NameCollision : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.2.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking NetMessage.GetData\\NameCollision";

		public override void Run()
		{
			int tmp = 0;
			var vanilla = this.Method(() => (new Terraria.MessageBuffer()).GetData(0, 0, out tmp));
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.MessageBuffer.NameCollision(null));

			//Luckily there is a if statement that we can inject our callbacks result into.
			//What we need to do is find a reference point and back track to this flag.
			//Currently this is doing by looking for "Name is too long." and back tracking to the 
			//first brfalse.s where we inject our callback.

			var flag = vanilla.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldstr && x.Operand.Equals("Net.NameTooLong"))
												.Previous(y => y.OpCode == OpCodes.Brfalse_S);

			//Our arguments requires the player reference (which is easier than grabbing a bunch of details the player object already has)
			var player = flag.Next(x => x.OpCode == OpCodes.Ldloc_S);

			var il = vanilla.Body.GetILProcessor();

			//These are in reverse order due to InsertAfter
			il.InsertAfter(flag, il.Create(OpCodes.Brfalse_S, flag.Operand as Instruction));
			il.InsertAfter(flag, il.Create(OpCodes.Call, vanilla.Module.Import(callback)));
			il.InsertAfter(flag, il.Create(OpCodes.Ldloc_S, player.Operand as VariableDefinition));
		}
	}
}
