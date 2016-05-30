using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Main
{
    public class GameInitialise : Inject.Injection<OTAPIContext>
    {
        public override void Inject(OptionSet options)
        {
            Console.Write("Hooking Game.Initialise...");

            //Grab the Initialise method
            var vanilla = this.Context.Terraria.Types.Main.Method("Initialize");
            //Wrap it with the API calls
            vanilla.InjectBeginEnd(this.Context.OTAPI.Types.Main, "Initialize");

            Console.WriteLine("Done");
        }
    }
}
