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

            var apiMatch = this.Context.OTAPI.Types.Main.Methods.Where(x => x.Name.StartsWith("startDedInput"));
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching startDedInput Begin/End calls in OTAPI");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            target.Wrap(cbkBegin, cbkEnd, true);
            Console.WriteLine("Done");
        }
    }
}
