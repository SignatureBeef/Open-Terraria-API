using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Modifications.Patches
{
    /// <summary>
    /// Makes all types public in the OTAPI assembly dll.
    /// </summary>
    public class MakeTypesPublic : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Making all types public...";
        public override void Run(OptionSet options)
        {
            foreach(var type in Context.Terraria.MainModue.Types)
                type.MakePublic(false);
        }
    }
}
