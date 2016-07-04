using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
    public class Strike : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking Npc.StrikeNPC...";

        public override void Run()
        {
            var vanilla = this.Context.Terraria.Types.Npc.Method("StrikeNPC");
            var callback = this.Context.OTAPI.Types.Npc.Method("Strike");
            
            vanilla.InjectNonVoidCallback(callback);
        }
    }
}
