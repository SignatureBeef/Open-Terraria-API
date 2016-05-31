using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Command
{
    /// <summary>
    /// This modification will allow the hook for starting a custom command thread to function.
    /// </summary>
    public class StartCommandThread : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking console listener creation...");
            var target = this.Context.Terraria.Types.Main.Method("startDedInput");
            var callback = this.Context.OTAPI.Types.Main.Methods.Single(x => x.Name == "startDedInput");

            target.Wrap(callback, beginIsCancellable: true);
            Console.WriteLine("Done");
        }
    }
}
