using NDesk.Options;
using OTAPI.Patcher.Extensions;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Hooks.Projectile
{
    public class AI : OTAPIModification<OTAPIContext>
    {
        public override void Run(OptionSet options)
        {
            Console.Write("Hooking Projectile.AI()...");

            var vanilla = this.Context.Terraria.Types.Projectile.Methods.Single(
                x => x.Name == "AI"
                && x.Parameters.Count() == 0
            );
            
            var cbkBegin = this.Context.OTAPI.Types.Projectile.Method("AIBegin", parameters: vanilla.Parameters);
            var cbkEnd = this.Context.OTAPI.Types.Projectile.Method("AIEnd", parameters: vanilla.Parameters);

            vanilla.Wrap(cbkBegin, cbkEnd, true);

            Console.WriteLine("Done");
        }
    }
}
