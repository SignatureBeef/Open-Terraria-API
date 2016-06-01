using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Npc
{
    public class SetDefaultsByName : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Npc.SetDefaults(string)...");

            var vanilla = this.Context.Terraria.Types.Npc.Methods.Single(
                x => x.Name == "SetDefaults"
                && x.Parameters.First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.String
            );
            
            var cbkBegin = this.Context.OTAPI.Types.Npc.Method("SetDefaultsByNameBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Npc.Method("SetDefaultsByNameEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);

            Console.WriteLine("Done");
        }
    }
}
