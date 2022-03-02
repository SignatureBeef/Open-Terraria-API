using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Patches
{
	/// <summary>
	/// Makes all types public in the OTAPI assembly dll.
	/// </summary>
	public class MakeTypesPublic : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.6, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Making all types public...";
		public override void Run()
		{
			foreach (var type in SourceDefinition.MainModule.Types)
				type.MakePublic();
		}
	}
}
