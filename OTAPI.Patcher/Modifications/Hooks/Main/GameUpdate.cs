using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Main
{
    public class GameUpdate : Inject.Injection<OTAPIContext>
    {
        public override void Inject(OptionSet options)
        {
            Console.Write("Hooking GameUpdate...");

            //Grab the Update method
            var updateServer = this.Context.Terraria.Types.Main.Method("Update");
            //Wrap it with the API calls
            updateServer.InjectBeginEnd(this.Context.OTAPI.Types.Main, "Update");

            Console.WriteLine("Done");
        }
    }
}
