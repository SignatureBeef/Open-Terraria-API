using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Item
{
	public class SetDefaultsByName : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Item.SetDefaults(string)...";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.Item()).SetDefaults(String.Empty));

			string tmp = null;
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Item.SetDefaultsByNameBegin(null, ref tmp));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.Item.SetDefaultsByNameEnd(null, ref tmp));

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
