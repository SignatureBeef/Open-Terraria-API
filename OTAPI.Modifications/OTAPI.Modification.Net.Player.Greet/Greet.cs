using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;

using System;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net.Player
{
    /// <summary>
    /// This modification is to allow the NetMessage.greetPlayer hooks to be ran by injecting callbacks into
    /// the start and end of the vanilla method.
    /// </summary>
    public class Greet : ModificationBase
    {
		public override string Description => "Hooking NetMessage.greetPlayer";
		public override void Run()
        {
			var vanilla = SourceDefinition.Type("Terraria.NetMessage")
				.Method("greetPlayer");

			var cbkBegin = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.NetMessage")
				.Method("GreetPlayerBegin", parameters: vanilla.Parameters);

            var cbkEnd = ModificationDefinition.Type("OTAPI.Core.Callbacks.Terraria.NetMessage")
				.Method("GreetPlayerEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
