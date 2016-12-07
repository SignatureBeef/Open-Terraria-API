using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Command
{
	/// <summary>
	/// This modification will allow the hook for starting a custom command thread to function.
	/// </summary>
	public class StartCommandThread : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking console listener creation...";

		public override void Run()
		{
			var target = this.Method(() => Terraria.Main.startDedInput());
			var callback = this.Method(() => OTAPI.Callbacks.Terraria.Main.startDedInput());

			target.Wrap
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
