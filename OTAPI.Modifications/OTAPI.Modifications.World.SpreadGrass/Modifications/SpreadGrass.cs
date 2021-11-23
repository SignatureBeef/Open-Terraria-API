using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World
{
	public class SpreadGrass : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking world grass spreading...";
		public override void Run()
		{
			int tmpi = 0;
			bool tmpb = false;
			byte tmp8 = 0;
			var vanilla = this.Method(() => Terraria.WorldGen.SpreadGrass(0, 0, 0, 0, false, 0));
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.World.SpreadGrass(ref tmpi, ref tmpi, ref tmpi, ref tmpi, ref tmpb, ref tmp8));

			vanilla.Wrap
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
