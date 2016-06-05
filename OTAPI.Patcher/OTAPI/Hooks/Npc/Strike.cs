using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Hooks.Npc
{
    public class Strike : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Npc.StrikeNPC...");

            var vanilla = this.Context.Terraria.Types.Npc.Method("StrikeNPC");
            var callback = this.Context.OTAPI.Types.Npc.Method("Strike");
            
            vanilla.InjectNonVoidCallback(callback);

            Console.WriteLine("Done");
        }
    }
}
