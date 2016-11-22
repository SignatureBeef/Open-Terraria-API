using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Player
{
	public class JumpHeight : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"Terraria, Version=1.3.4.3, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Player.UpdateJumpHeight";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.Player()).UpdateJumpHeight());

			var cbkBegin = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Player.UpdateJumpHeightBegin(null))
			);

			var cbkEnd = this.SourceDefinition.MainModule.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.Player.UpdateJumpHeightEnd(null))
			);

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
