using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Modifications.Mono.Modifications
{
	public class RemoveNatFromNetplay : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Removing NAT from Netplay";

		public override void Run()
		{
			var netplay = this.Type<Terraria.Netplay>();

			foreach (var method in new[] {
				this.Method(() => Terraria.Netplay.closePort()),
				netplay.Method("OpenPort")
			})
			{
				method.Body.Instructions.Clear();
				method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ret));
				method.Body.Variables.Clear();
				method.Body.ExceptionHandlers.Clear();
			}

			netplay.Fields.Remove(netplay.Field("mappings"));
			netplay.Fields.Remove(netplay.Field("upnpnat"));

			var typesToRemove = new List<TypeDefinition>();
			this.SourceDefinition.MainModule.ForEachType(type =>
			{
				if (type.Namespace.StartsWith("NATUPNPLib"))
				{
					typesToRemove.Add(type);
				}
			});
			foreach (var type in typesToRemove)
				this.SourceDefinition.MainModule.Types.Remove(type);
		}
	}
}
