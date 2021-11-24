using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Main
{
	/// <summary>
	/// Adds a hook for checking if it's christmas
	/// </summary>
	public class Christmas : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Game.checkXMas...";

		public override void Run()
		{
			//Grab the methods
			var vanilla = this.Method(() => Terraria.Main.checkXMas());
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.Main.Christmas());

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
