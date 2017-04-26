using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Main
{
	/// <summary>
	/// Adds a hook for checking if it's halloween
	/// </summary>
	public class Halloween : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.5.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Game.checkHalloween";

		public override void Run()
		{
			//Grab the methods
			var vanilla = this.Method(() => Terraria.Main.checkHalloween());
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.Main.Halloween());

			//Inject only the begin call
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
