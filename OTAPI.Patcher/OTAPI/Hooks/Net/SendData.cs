using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTAPI.Patcher.Modifications.Hooks.Net
{
    public class SendData : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking NetMessage.Send...");

            var vanilla = this.Context.Terraria.Types.NetMessage.Method("SendData");
            var callback = this.Context.OTAPI.Types.NetMessage.Method("SendData");

            //Few stack issues arose trying to inject a callack before for lock, so i'll resort to 
            //wrapping the method;

            vanilla.Wrap(callback, null, true);

            Console.WriteLine("Done");
        }
    }
}
//InjectCallback