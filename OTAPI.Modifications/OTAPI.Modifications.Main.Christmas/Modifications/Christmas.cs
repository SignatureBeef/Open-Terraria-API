using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Main
{
	/// <summary>
	/// Adds a hook for checking if it's christmas
	/// </summary>
	public class Christmas : ModificationBase
	{
		public override string Description => "Hooking Game.checkXMas...";

		public override void Run()
		{
			//Grab the methods
			var vanilla = this.SourceDefinition.Type("Terraria.Main").Method("checkXMas");
			var callback = this.ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Main").Method("Christmas");

			//Inject only the begin call
			vanilla.Wrap(callback, null, true);
		}
	}
}
