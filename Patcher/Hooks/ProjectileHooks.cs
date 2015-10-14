using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [ServerHook]
        private void HookProjectileSetDefaults()
        {
            var method = Terraria.Projectile.Method("SetDefaults");

            var apiMatch = API.ProjectileCallback.Methods.Where(x => x.Name.StartsWith("OnSetDefaults"));
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching SetDefault Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            var wrapped = method.Wrap(cbkBegin, cbkEnd);

            //Change the Pop call to update the Type parameter
            var insPop = wrapped.Body.Instructions.Single(x => x.OpCode == OpCodes.Pop);
            var il = wrapped.Body.GetILProcessor();
            il.Replace(insPop, il.Create(OpCodes.Starg, wrapped.Parameters.Single(x => x.Name == "Type")));
        }

        [ServerHook]
        private void HookProjectileAI()
        {
            var method = Terraria.Projectile.Method("AI");

            var apiMatch = API.ProjectileCallback.Methods.Where(x => x.Name.StartsWith("OnAI"));
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching AI Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            method.Wrap(cbkBegin, cbkEnd);
        }
    }
}

