using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Sockets
{
	public class Modification : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => $"Merging in sockets";//{nameof(Sockets.Modification.)}";

		public override void Run()
		{
			//Literally does nothing, merely used to for the patcher engine to acknowledge that
			//we need to be merged into the source assembly
		}
	}
}
