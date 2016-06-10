using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Hooks.Main
{
    /// <summary>
    /// Adds a hook for checking if it's halloween
    /// </summary>
    public class Halloween : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Game.checkHalloween...");

            //Grab the methods
            var vanilla = this.Context.Terraria.Types.Main.Method("checkHalloween");
            var callback = this.Context.OTAPI.Types.Main.Method("Halloween");

            //Inject only the begin call
            vanilla.Wrap(callback, null, true);

            Console.WriteLine("Done");
        }
    }
}
