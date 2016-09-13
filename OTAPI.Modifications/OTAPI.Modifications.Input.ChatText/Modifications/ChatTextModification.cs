using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Modifications.Input.ChatText.Modifications
{
	public class ChatTextModification : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"Terraria, Version=1.3.3.1, Culture=neutral, PublicKeyToken=null"
		};

		public override string Description => "Hooking chat messages";

		public override void Run()
		{
			var method = this.SourceDefinition.Type("Terraria.Main").Method("Update");
			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Main.OnChatTextSend())
			);

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
	}
}
