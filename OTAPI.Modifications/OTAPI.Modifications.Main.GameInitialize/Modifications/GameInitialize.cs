using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Hooks.Main
{
	/// <summary>
	/// This modification is to allow the Game.Initialize hooks to be ran by injecting callbacks into
	/// the start and end of the vanilla method.
	/// </summary>
	public class GameInitialize : OTAPIModification<OTAPIContext>
	{
		public override string Description => "Hooking Game.Initialize...";

		public override void Run(OptionSet options)
		{
			//Grab the Initialise method
			var vanilla = this.Context.Terraria.Types.Main.Method("Initialize");
			//Wrap it with the API calls
			vanilla.InjectBeginEnd(this.Context.OTAPI.Types.Main, "Initialize");
		}
	}
}
