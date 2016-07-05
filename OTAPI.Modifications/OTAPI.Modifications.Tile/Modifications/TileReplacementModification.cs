using NDesk.Options;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Modification.Tile.Modifications
{
	public class TileReplacementModification : ModificationBase
	{
		public override string Description => "Patching tiles";

		public void CheckSignatures()
		{

		}

		public override void Run(OptionSet options)
		{
			//Ensure our custom tile is the same signature as terraria's
			CheckSignatures();


		}
	}
}
