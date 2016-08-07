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
			"TerrariaServer, Version=1.3.2.1, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking NetMessage.greetPlayer";
		public override void Run()
		{
			var vanilla = SourceDefinition.Type("Terraria.NetMessage")
				.Method("greetPlayer");

			var cbkBegin = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.NetMessage")
				.Method("GreetPlayerBegin", parameters: vanilla.Parameters);

			var cbkEnd = ModificationDefinition.Type("OTAPI.Callbacks.Terraria.NetMessage")
				.Method("GreetPlayerEnd", parameters: vanilla.Parameters);

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
