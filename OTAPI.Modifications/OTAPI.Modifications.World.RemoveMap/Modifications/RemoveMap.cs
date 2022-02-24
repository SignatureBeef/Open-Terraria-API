using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using Terraria.Map;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World
{
	public class RemoveMap : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Removing world map...";
		public override void Run()
		{
			var worldMap = this.Type<WorldMap>();

			foreach (var method in worldMap.Methods)
			{
				method.ClearBody();
			}
		}
	}
}
