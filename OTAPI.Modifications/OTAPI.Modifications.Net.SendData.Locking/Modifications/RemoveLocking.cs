using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net
{
	/// <summary>
	/// this modification will remove the locking in the NetMessage.SendData method as it
	/// frequently conflicts with the CheckBytes lock, causing heavy lag
	/// </summary>
	[Ordered(3)]
	public class AvoidLocking : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.1, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Removing locks in NetMessage.SendData...";

		public override void Run()
		{
			// what we need to do is remove the `lock(NetMessage.buffer[index]) { }`
			// In IL terms this is secretly a call to Monitor.Enter, and Monitor.Exit

			var sendData = this.Method(() => Terraria.NetMessage.SendData(0, 0, 0, Terraria.Localization.NetworkText.Empty, 0, 0, 0, 0, 0, 0, 0));

			var monitorEnter = sendData.Body.Instructions.Single(
				x => x.OpCode == OpCodes.Call
				&& (x.Operand as MethodReference).Name == "Enter"
			);

			var messageBuffer = monitorEnter.Previous(
				x => x.OpCode == OpCodes.Ldloc_1
				//&& (x.Operand as FieldReference).Name == "buffer"
			);

			var processor = sendData.Body.GetILProcessor();
			
			while (messageBuffer.Next != monitorEnter)
			{
				processor.Remove(messageBuffer.Next);
			}
			processor.Remove(messageBuffer.Next);
			processor.Remove(messageBuffer);

			//remove the the last leave.s instruction, up to and including the endfinally instruction
			var leaves = sendData.Body.Instructions.Single(
				x => x.OpCode == OpCodes.Call
				&& (x.Operand as MethodReference).Name == "Exit"
			).Previous(
				x => x.OpCode == OpCodes.Leave_S
			);
			var endfinally = sendData.Body.Instructions.Single(
				x => x.OpCode == OpCodes.Endfinally
			);

			// update other branches that reference the leave short that will be removed
			leaves.ReplaceTransfer(endfinally.Next, sendData);
			
			while (leaves.Next != endfinally)
			{
				processor.Remove(leaves.Next);
			}
			processor.Remove(leaves.Next);
			processor.Remove(leaves);

			foreach (var item in sendData.Body.ExceptionHandlers
				.Where(x => x.TryStart == messageBuffer)
				.ToArray())
			{
				sendData.Body.ExceptionHandlers.Remove(item);
			}
		}
	}
}
