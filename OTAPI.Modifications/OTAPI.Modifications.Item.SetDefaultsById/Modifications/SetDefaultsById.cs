using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Item
{
    public class SetDefaultsById : ModificationBase
    {
        public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
        {
            "TerrariaServer, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
        public override string Description => "Hooking Item.SetDefaults(int,bool)...";
        public override void Run()
        {
			var vanilla = this.Method(() => (new Terraria.Item()).SetDefaults(0, false));

			int tmpI = 0;
			bool tmpB = false;
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Item.SetDefaultsByIdBegin(null, ref tmpI, ref tmpB));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.Item.SetDefaultsByIdEnd(null, ref tmpI, ref tmpB));

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
