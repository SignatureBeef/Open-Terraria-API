using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Net.Player
{
    /// <summary>
    /// This modification is to allow the NetMessage.greetPlayer hooks to be ran by injecting callbacks into
    /// the start and end of the vanilla method.
    /// </summary>
    public class GreetPlayer : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking NetMessage.greetPlayer";
		public override void Run(OptionSet options)
        {
            var vanilla = this.Context.Terraria.Types.NetMessage.Method("greetPlayer");

            var cbkBegin = this.Context.OTAPI.Types.NetMessage.Method("GreetPlayerBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.NetMessage.Method("GreetPlayerEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);

            Console.WriteLine("Done");
        }
    }
}
