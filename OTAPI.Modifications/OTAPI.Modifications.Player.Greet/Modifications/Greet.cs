using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Player
{
	/// <summary>
	/// This modification is to allow the NetMessage.greetPlayer hooks to be ran by injecting callbacks into
	/// the start and end of the vanilla method.
	/// </summary>
	public class Greet : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.2.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking NetMessage.greetPlayer";
		public override void Run()
		{
			var vanilla = this.Method(() => Terraria.NetMessage.greetPlayer(0));

			int tmp = 0;
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.NetMessage.GreetPlayerBegin(ref tmp));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.NetMessage.GreetPlayerEnd(0));

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
