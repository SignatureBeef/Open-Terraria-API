﻿using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Linq;
using Terraria;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
	public class SetDefaultsById : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.3.2, Culture=neutral, PublicKeyToken=null",
			"Terraria, Version=1.3.4.4, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Hooking Npc.SetDefaults(int,float)...";
		public override void Run()
		{
			var vanilla = this.Method(() => (new Terraria.NPC()).SetDefaults(0, default));

			int tmp = 0;
			NPCSpawnParams spawnparams = default(NPCSpawnParams);
			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Npc.SetDefaultsByIdBegin(null, ref tmp, ref spawnparams));
			var cbkEnd = this.Method(() => OTAPI.Callbacks.Terraria.Npc.SetDefaultsByIdEnd(null, tmp, spawnparams));

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
