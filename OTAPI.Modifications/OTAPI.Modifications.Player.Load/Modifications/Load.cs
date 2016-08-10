using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Player
{
	public class Load : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"Terraria, Version=1.3.2.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Player.LoadPlayer";
		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.Player")
				.Method("LoadPlayer");

			string tmp = null;
			bool tmp2 = false;
			global::Terraria.IO.PlayerFileData data = null;
			var cbkBegin = //this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Player.LoadBegin(ref data, ref tmp, ref tmp2));
			//);

			//var cbkEnd = this.SourceDefinition.MainModule.Import(
			//	this.Method(() => OTAPI.Callbacks.Terraria.Player.LoadEnd(null, false))
			//);

			vanilla.InjectNonVoidBeginCallback(cbkBegin);
			//vanilla.Wrap
			//(
			//	beginCallback: cbkBegin,
			//	endCallback: cbkEnd,
			//	beginIsCancellable: true,
			//	noEndHandling: false,
			//	allowCallbackInstance: false,
			//	overrideReturnType: vanilla.ReturnType
			//);
		}
	}
}
