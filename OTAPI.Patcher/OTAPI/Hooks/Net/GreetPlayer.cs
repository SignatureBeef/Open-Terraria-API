using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Hooks.Net
{
    /// <summary>
    /// This modification is to allow the NetMessage.greetPlayer hooks to be ran by injecting callbacks into
    /// the start and end of the vanilla method.
    /// </summary>
    public class GreetPlayer : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking NetMessage.greetPlayer...");

            var vanilla = this.Context.Terraria.Types.NetMessage.Method("greetPlayer");

            var cbkBegin = this.Context.OTAPI.Types.NetMessage.Method("GreetPlayerBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.NetMessage.Method("GreetPlayerEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);

            Console.WriteLine("Done");
        }
    }
}
