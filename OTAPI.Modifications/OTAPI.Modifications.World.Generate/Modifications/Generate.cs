using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using Terraria.WorldBuilding;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World
{
    public class Generate : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking world generation...";
		public override void Run()
		{
			int tmpI = 0;
			GenerationProgress tmpGp = null;

			var generate = this.Method(() => Terraria.WorldGen.GenerateWorld(0, null));
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.World.Generate(ref tmpI, ref tmpGp));

			generate.Wrap
			(
				beginCallback: callback,
				endCallback: null,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: false
			);
		}
	}
}
