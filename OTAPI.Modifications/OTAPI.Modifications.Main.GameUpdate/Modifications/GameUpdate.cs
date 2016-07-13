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
			"TerrariaServer, Version=1.3.1.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Game.Update";
		public override void Run()
		{
			//Grab the Update method
			var vanilla = this.SourceDefinition.Type("Terraria.Main").Method("Update");

			var cbkBegin = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Main").Method("UpdateBegin", parameters: vanilla.Parameters);
			var cbkEnd = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Main").Method("UpdateEnd", parameters: vanilla.Parameters);
			
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
