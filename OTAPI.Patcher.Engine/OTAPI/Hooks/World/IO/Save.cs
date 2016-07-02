using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.World.IO
{
    public class Save : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking WorldFile.saveWorld(bool,bool)...";
        public override void Run(OptionSet options)
        {
            var vanilla = this.Context.Terraria.Types.WorldFile.Methods.Single(
                x => x.Name == "saveWorld"
                && x.Parameters.Count() == 2
                && x.Parameters[0].ParameterType == this.Context.Terraria.MainModue.TypeSystem.Boolean
                && x.Parameters[1].ParameterType == this.Context.Terraria.MainModue.TypeSystem.Boolean
            );


            var cbkBegin = this.Context.OTAPI.Types.WorldFile.Method("SaveWorldBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.WorldFile.Method("SaveWorldEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
