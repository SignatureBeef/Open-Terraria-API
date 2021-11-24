using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Item
{
	public class NetDefaults : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.2, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};

		public override string Description => "Hooking Item.NetDefaults(int)...";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.Item()).netDefaults(0));

			int tmp = 0;
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Item.NetDefaultsBegin(null, ref tmp));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.Item.NetDefaultsEnd(null, ref tmp));

			vanilla.Wrap
			(
				beginCallback: cbkBegin,
				endCallback: cbkEnd,
				beginIsCancellable: true,
				noEndHandling: false,
				allowCallbackInstance: true
			);
		}
	}
}
