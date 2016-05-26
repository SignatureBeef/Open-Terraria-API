using NDesk.Options;
using OTAPI.Patcher.Inject;
using System;
using Mono.Cecil.Rocks;
using OTAPI.Patcher.Extensions;

namespace OTAPI.Patcher.Modifications
{
    /// <summary>
    /// 
    /// </summary>
    public class MakeTypesPublic : Injection<OTAPIContext>
    {
        public override void Inject(OptionSet options)
        {
            Console.Write("Making all types public...");

            foreach(var type in Context.OTAPI.MainModule.Types)
                type.MakePublic(false);


            Console.WriteLine("Done.");
        }
    }
}
