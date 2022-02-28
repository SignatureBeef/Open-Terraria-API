using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Main
{
	/// <summary>
	/// This modification is to allow the Game.Initialize hooks to be ran by injecting callbacks into
	/// the start and end of the vanilla method.
	/// </summary>
	public class GameInitialize : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.5, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Game.Initialize...";

		public override void Run()
		{
			//Grab the Initialise method manually since it's protected
			var vanilla = this.SourceDefinition.Type("Terraria.Main").Method("Initialize");

			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Main.InitializeBegin(null));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.Main.InitializeEnd(null));

			vanilla.Wrap
			(
				beginCallback: cbkBegin,
				endCallback: cbkEnd,
				beginIsCancellable: false,
				noEndHandling: false,
				allowCallbackInstance: true
			);
		}
	}
}
