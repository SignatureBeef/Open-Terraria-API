using NDesk.Options;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Modifications.Hooks.Projectile
{
    public class SetDefaultsById : OTAPIModification<OTAPIContext>
    {
		public override string Description => "Hooking Projectile.SetDefaults(int)...";
        public override void Run(OptionSet options)
        {
            var vanilla = this.Context.Terraria.Types.Projectile.Methods.Single(
                x => x.Name == "SetDefaults"
                && x.Parameters.Single().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Int32
            );


            var cbkBegin = this.Context.OTAPI.Types.Projectile.Method("SetDefaultsByIdBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Projectile.Method("SetDefaultsByIdEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);
        }
    }
}
