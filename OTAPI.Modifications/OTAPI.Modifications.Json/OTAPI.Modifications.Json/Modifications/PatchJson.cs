using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Patches
{
	/// <summary>
	/// Vanilla Terraria uses Newtonsoft.JSON 7.0.0.0. We rather a higher version, and we also rather use
	/// NuGet.
	/// </summary>
	public class PatchJson : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.3.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Patching Newtonsoft.Json";

		public override void Run()
		{
			var desired = typeof(Newtonsoft.Json.JsonConvert).Assembly.GetName().Version;

			//Update the references to match what is installed to OTAPI.Modifications.Json
			foreach (var reference in this.SourceDefinition.MainModule.AssemblyReferences)
			{
				if (reference.Name == "Newtonsoft.Json")
				{
					reference.Version = desired;
					break;
				}
			}

			//Remove the embedded Newtonsoft resource
			SourceDefinition.MainModule.Resources.Remove(
				SourceDefinition.MainModule.Resources.Single(x => x.Name == "Terraria.Libraries.JSON.NET.Newtonsoft.Json.dll")
			);
		}
	}
}
