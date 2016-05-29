using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Main
{
    public class StartCommandThread : Inject.Injection<OTAPIContext>
    {
        public override void Inject(OptionSet options)
        {
            Console.Write("Hooking console listener creation...");
            var target = this.Context.Terraria.Types.Main.Method("startDedInput");
            var callback = this.Context.OTAPI.Types.Main.Methods.Single(x => x.Name == "startDedInput");

            target.Wrap(callback, beginIsCancellable: true);
            Console.WriteLine("Done");
        }
    }
}
