using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Item
{
	public class CheckMaterial : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.6, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};

		public override string Description => "Hooking Item.checkMat()...";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.Item()).checkMat());

			bool tmp = false;
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Item.CheckMaterialBegin(null, ref tmp));

			vanilla.InjectNonVoidBeginCallback(cbkBegin);
		}
	}
}
