using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Projectile
{
    public class Update : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking Projectile.Update(int)...";
        public override void Run(OptionSet options)
        {
            var vanilla = this.Context.Terraria.Types.Projectile.Methods.Single(
                x => x.Name == "Update"
                && x.Parameters.Single().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Int32
            );
            
            var cbkBegin = this.Context.OTAPI.Types.Projectile.Method("UpdateBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Projectile.Method("UpdateEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
