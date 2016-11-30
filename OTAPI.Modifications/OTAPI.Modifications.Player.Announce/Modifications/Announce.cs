using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Extensions.ILProcessor;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Player
{
	/// <summary>
	/// This modification is to allow the NetMessage.greetPlayer hooks to be ran by injecting callbacks into
	/// the start and end of the vanilla method.
	/// </summary>
	public class Announce : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking NetMessage.SyncOnePlayer";
		public override void Run()
		{
			var vanilla = this.Type<Terraria.NetMessage>().Method("SyncOnePlayer");

			var callback = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.NetMessage.Announce(0))
			);

			var points = vanilla.Body.Instructions.Where(
				x => x.OpCode == OpCodes.Ldfld
				&& (x.Operand as FieldReference).Name == "IsAnnouncementCompleted"
			).ToArray();

			var processor = vanilla.Body.GetILProcessor();

			foreach (var point in points)
			{
				var beforeClient = point.Previous(
					x => x.OpCode == OpCodes.Ldsfld
					&& (x.Operand as FieldReference).Name == "Clients"
				);

				var injected = processor.InsertBefore(beforeClient,
					new { OpCodes.Ldarg_0 },
					new { OpCodes.Call, callback },
					new { OpCodes.Brtrue, beforeClient },
					new { OpCodes.Ret }
				);

				beforeClient.ReplaceTransfer(injected[0], vanilla);
				injected[2].Operand = beforeClient;
			}
		}
	}
}
