using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [OTAPatch(SupportType.Server, "Wrapping Projectile.SetDefaults")]
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

        [OTAPatch(SupportType.Server, "Wrapping Projectile.AI")]
        private void HookProjectileAI()
        {
            var method = Terraria.Projectile.Method("AI");

            var apiMatch = API.ProjectileCallback.Methods.Where(x => x.Name.StartsWith("OnAI"));
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching AI Begin/End calls in the API");

            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));

            method.Wrap(cbkBegin, cbkEnd);
        }

        [OTAPatch(SupportType.ClientServer, "Hooking projectile creation")]
        private void HookProjectileCreation()
        {
            var method = Terraria.Projectile.Method("NewProjectile");
            var callback = API.ProjectileCallback.Method("GetProjectile");

            //Find the second Main.projectile reference
            var insIndex = method.Body.Instructions.Where(x => x.OpCode == OpCodes.Ldsfld
                               && x.Operand is FieldReference
                               && (x.Operand as FieldReference).Name == "projectile").Skip(1).First()
                .Next;

            var il = method.Body.GetILProcessor();

            il.Remove(insIndex.Previous);
            il.Remove(insIndex.Next);

            il.InsertAfter(insIndex, il.Create(OpCodes.Call, Terraria.Import(callback)));

            //Add all parameters
            foreach (var prm in method.Parameters.Reverse())
            {
                il.InsertAfter(insIndex, il.Create(OpCodes.Ldarg, prm));
            }
        }
    }
}

