using NDesk.Options;
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
		public override string Description => "Hooking Game.Update";
        public override void Run()
        {
            //Grab the Update method
            var vanilla = this.SourceDefinition.Type("Terraria.Main").Method("Update");
            //Wrap it with the API calls
            vanilla.InjectBeginEnd(this.ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.Main"), "Update");
        }
    }
}
