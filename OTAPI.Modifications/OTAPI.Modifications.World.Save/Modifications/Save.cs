using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World.IO
{
	public class Save : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.5.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking WorldFile.saveWorld(bool,bool)...";
		public override void Run()
		{
			var vanilla = this.Method(() => Terraria.IO.WorldFile.saveWorld(false, false));

			bool tmp = false;
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.WorldFile.SaveWorldBegin(ref tmp, ref tmp));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.WorldFile.SaveWorldEnd(tmp, tmp));

			vanilla.Wrap
			(
				beginCallback: cbkBegin,
				endCallback: cbkEnd,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: false
			);
		}
	}
}
