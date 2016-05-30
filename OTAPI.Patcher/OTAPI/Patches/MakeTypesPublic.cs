using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modification;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Patches
{
    /// <summary>
    /// Makes all types public in the OTAPI assembly dll.
    /// </summary>
    public class MakeTypesPublic : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Making all types public...");

            foreach(var type in Context.OTAPI.MainModue.Types)
                type.MakePublic(false);

            Console.WriteLine("Done.");
        }
    }
}
