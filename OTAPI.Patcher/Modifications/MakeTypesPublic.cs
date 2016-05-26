using NDesk.Options;
using OTAPI.Patcher.Inject;
using System;
using Mono.Cecil.Rocks;

namespace OTAPI.Patcher.Modifications
{
    /// <summary>
    /// 
    /// </summary>
    public class MakeTypesPublic : Injection<OTAPIContext>
    {
        public override void Inject(OptionSet options)
        {
            Console.WriteLine("Making all types public.");

            //Context.Terraria.MainModule.GetAllTypes()
        }
    }
}
