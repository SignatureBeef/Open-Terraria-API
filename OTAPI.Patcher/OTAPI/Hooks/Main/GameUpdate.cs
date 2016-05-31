using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Hooks.Main
{
    /// <summary>
    /// This modification is to allow the Game.Update hooks to be ran by injecting callbacks into
    /// the start and end of the vanilla method.
    /// </summary>
    public class GameUpdate : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Game.Update...");

            //Grab the Update method
            var vanilla = this.Context.Terraria.Types.Main.Method("Update");
            //Wrap it with the API calls
            vanilla.InjectBeginEnd(this.Context.OTAPI.Types.Main, "Update");

            Console.WriteLine("Done");
        }
    }
}
