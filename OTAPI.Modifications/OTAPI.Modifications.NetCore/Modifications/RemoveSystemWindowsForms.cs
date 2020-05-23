using System.Linq;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Patches
{
	/// <summary>
	/// Replaces all System.Windows.Forms references to use OTAPI.Modifications.NetCore.dll
	/// </summary>
	public class RemoveSystemWindowsForms : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.0.4, Culture=neutral, PublicKeyToken=null",
			"ReLogic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Removing System.Windows.Forms references...";

		public override void Run()
		{
			foreach (var reference in SourceDefinition.MainModule.AssemblyReferences
				.Where(x => x.Name.StartsWith("System.Windows.Forms")))
			{
				reference.Name = "OTAPI.Modifications.NetCore";
				reference.PublicKey = ModificationDefinition.Name.PublicKey;
				reference.PublicKeyToken = ModificationDefinition.Name.PublicKeyToken;
				reference.Version = ModificationDefinition.Name.Version;
			}
		}
	}
}