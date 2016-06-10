using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Projectile
{
    public class Update : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Projectile.Update(int)...");

            var vanilla = this.Context.Terraria.Types.Projectile.Methods.Single(
                x => x.Name == "Update"
                && x.Parameters.Single().ParameterType == this.Context.Terraria.MainModue.TypeSystem.Int32
            );


            var cbkBegin = this.Context.OTAPI.Types.Projectile.Method("UpdateBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Projectile.Method("UpdateEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);

            Console.WriteLine("Done");
        }
    }
}
