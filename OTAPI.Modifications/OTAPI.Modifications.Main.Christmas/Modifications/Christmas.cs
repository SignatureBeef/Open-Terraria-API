using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Hooks.Main
{
    /// <summary>
    /// Adds a hook for checking if it's christmas
    /// </summary>
    public class Christmas : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Game.checkXMas...");

            //Grab the methods
            var vanilla = this.Context.Terraria.Types.Main.Method("checkXMas");
            var callback = this.Context.OTAPI.Types.Main.Method("Christmas");

            //Inject only the begin call
            vanilla.Wrap(callback, null, true);

            Console.WriteLine("Done");
        }
    }
}
