using OTAPI.Patcher.Engine.Modification;

using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Patches
{
	/// <summary>
	/// Replaces all Xna references to use OTAPI.Xna.dll
	/// </summary>
	public class RemoveXna : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null",
            "ReLogic, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
        };
		public override string Description => "Removing Xna references...";

		public override void Run()
		{
			//Context.OTAPI.MainModue.Resources.Clear();

			var xnaFramework = SourceDefinition.MainModule.AssemblyReferences
				.Where(x => x.Name.StartsWith("Microsoft.Xna.Framework"))
				.ToArray();

			for (var x = 0; x < xnaFramework.Length; x++)
			{
				xnaFramework[x].Name = "OTAPI.Modifications.Xna"; //TODO: Fix me, ILRepack is adding .dll to the asm name      Context.OTAPI.Assembly.Name.Name;
				xnaFramework[x].PublicKey = ModificationDefinition.Name.PublicKey;
				xnaFramework[x].PublicKeyToken = ModificationDefinition.Name.PublicKeyToken;
				xnaFramework[x].Version = ModificationDefinition.Name.Version;
			}

			////To resolve the "OTAPI" from above until the .dll can be corrected.
			//Context.AssemblyResolver.AssemblyResolve += AssemblyResolver_AssemblyResolve;
		}
	}
}
