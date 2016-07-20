using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Core.Modifications
{
	public class AssemblyModification : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null"
		};

		public override string Description => "Modifying assembly info";

		public override void Run()
		{
			var desc = this.SourceDefinition.CustomAttributes.Single(x =>
				x.AttributeType.FullName == typeof(System.Reflection.AssemblyDescriptionAttribute).FullName
			);

			desc.ConstructorArguments.Clear();
			desc.ConstructorArguments.Add(new Mono.Cecil.CustomAttributeArgument(
				this.SourceDefinition.MainModule.TypeSystem.String, "OTAPI v2"
			));
		}
	}
}
