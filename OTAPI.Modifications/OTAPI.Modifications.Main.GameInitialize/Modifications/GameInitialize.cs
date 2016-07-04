using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Modifications.Hooks.Main
{
	/// <summary>
	/// This modification is to allow the Game.Initialize hooks to be ran by injecting callbacks into
	/// the start and end of the vanilla method.
	/// </summary>
	public class GameInitialize : ModificationBase
	{
		public override string Description => "Hooking Game.Initialize...";

		public override void Run(OptionSet options)
		{
			//Grab the Initialise method
			var vanilla = this.SourceDefinition.Type("Terraria.Main").Method("Initialize");
			//Wrap it with the API calls
			vanilla.InjectBeginEnd(this.ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Main"), "Initialize");
		}
	}
}
