using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Main
{
	/// <summary>
	/// This modification is to allow the Game.Update hooks to be ran by injecting callbacks into
	/// the start and end of the vanilla method.
	/// </summary>
	public class GameUpdate : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.2.2, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Game.Update";
		public override void Run()
		{
			//Grab the Update method manually since it's protected
			var vanilla = this.SourceDefinition.Type("Terraria.Main").Method("Update");

			var tmp = new Microsoft.Xna.Framework.GameTime();
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Main.UpdateBegin(null, ref tmp));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.Main.UpdateEnd(null, ref tmp));

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
