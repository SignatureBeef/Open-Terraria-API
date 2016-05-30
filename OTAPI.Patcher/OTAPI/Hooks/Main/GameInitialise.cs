using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Hooks.Main
{
    /// <summary>
    /// This modification is to allow the Game.Initialize hooks to be ran by injecting
    /// to the start and end of the vanilla method.
    /// </summary>
    public class GameInitialise : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
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
