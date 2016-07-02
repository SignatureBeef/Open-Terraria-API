using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;

namespace OTAPI.Patcher.Engine.Modifications.Patches
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
