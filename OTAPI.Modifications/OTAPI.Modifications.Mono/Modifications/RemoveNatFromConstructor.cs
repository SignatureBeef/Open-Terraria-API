using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Modifications.Mono.Modifications
{
	/// <summary>
	/// The upnp that terraria uses is not mono compatible. 
	/// While it's possible to replace its functionality with one that is,
	/// it's not really worth the drama of implementing it.
	/// 
	/// This modification will remove the NAT code from the Terraria.Netplay
	/// constructor, thus allowing mono to continue.
	/// </summary>
	public class RemoveNatFromConstructor : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Removing upnp";

		public override void Run()
		{
			//Used to find the NAT instruction offset, and luckily it's also the first instruction
			//so we can remove all instruction, up until (and including) the mappings field.
			const String NatGuid = "AE1E00AA-3FD5-403C-8A27-2BBDC30CD0E1";
			
			//Get the Netplay static constructor method
			var constructor = this.SourceDefinition.MainModule.Type("Terraria.Netplay").StaticConstructor();

			//Get the il processor so we can alter il
			var processor = constructor.Body.GetILProcessor();

			//Find the instruction referencing the NAT guid string, which is the first
			//instruction that is to be removed.
			var startInstruction = constructor.Body.Instructions.Single(ins => 
				ins.OpCode == OpCodes.Ldstr 
				&& (string)ins.Operand == NatGuid
			);

			//Continuously remove instructions until a certain condition
			for (;;)
			{
				processor.Remove(startInstruction.Next);

				//If we are at the mappings field, remove it and exit the loop
				if (startInstruction.Next.OpCode == OpCodes.Stsfld && (startInstruction.Next.Operand as FieldReference).Name == "mappings")
				{
					processor.Remove(startInstruction.Next);
					break;
				}
			}

			//We are finished with the startInstruction, it can now be removed
			processor.Remove(startInstruction);
		}
	}
}
