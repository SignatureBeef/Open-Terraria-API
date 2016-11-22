using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Waterfall
{
	public class Find : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking WaterfallManager.FindWaterfalls";
		public override void Run()
		{
			//Grab the Update method
			var vanilla = this.Method(() => (new Terraria.WaterfallManager()).FindWaterfalls(false));

			bool tmp = false;
			var cbkBegin = this.Method(() => Callbacks.Terraria.Waterfall.FindBegin(null, ref tmp));
			var cbkEnd = this.Method(() => Callbacks.Terraria.Waterfall.FindEnd(null, tmp));

			vanilla.Wrap
			(
				beginCallback: cbkBegin,
				endCallback: cbkEnd,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: true
			);
		}
	}
}
