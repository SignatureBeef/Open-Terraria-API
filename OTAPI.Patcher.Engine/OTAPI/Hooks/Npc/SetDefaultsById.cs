using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Npc
{
    public class SetDefaultsById : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking Npc.SetDefaults(int,bool)...";
        public override void Run()
        {
            var vanilla = this.Context.Terraria.Types.Npc.Methods.Single(
                x => x.Name == "SetDefaults"
                && x.Parameters.First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Int32
                && x.Parameters.Skip(1).First().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Single
            );


            var cbkBegin = this.Context.OTAPI.Types.Npc.Method("SetDefaultsByIdBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Npc.Method("SetDefaultsByIdEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
