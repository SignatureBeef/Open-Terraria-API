using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Command
{
	/// <summary>
	/// This modification will allow the hook for starting a custom command thread to function.
	/// </summary>
	public class StartCommandThread : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.1.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking console listener creation...";

		public override void Run()
		{
			var target = this.SourceDefinition.Type("Terraria.Main").Method("startDedInput");
			var callback = this.ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Main").Methods.Single(x => x.Name == "startDedInput");

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
