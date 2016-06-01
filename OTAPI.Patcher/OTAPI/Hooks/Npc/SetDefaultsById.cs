using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Npc
{
    public class SetDefaultsById : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Npc.SetDefaults(int,bool)...");

            var vanilla = this.Context.Terraria.Types.Npc.Methods.Single(
                x => x.Name == "SetDefaults"
                && x.Parameters.First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Int32
                && x.Parameters.Skip(1).First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Single
            );


            var cbkBegin = this.Context.OTAPI.Types.Npc.Method("SetDefaultsByIdBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Npc.Method("SetDefaultsByIdEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);

            Console.WriteLine("Done");
        }
    }
}
