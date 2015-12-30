using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [OTAPatch(SupportType.ClientServer, "Wrapping Item.SetDefaults")]
        private void HookItemSetDefaults()
        {
            var setDefaults = Terraria.Item.Methods.Where(x => x.Name.StartsWith("SetDefault")).ToArray();
            foreach (var method in setDefaults)
            {
        
                //                method.Body.MaxStackSize = 6;
        
                var apiMatch = API.ItemCallback.MatchInstanceMethodByParameters("Terraria.Item", method.Parameters, "OnSetDefault");
                if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching SetDefault Begin/End calls in the API");
        
                var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
                var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));
        
                method.Wrap(cbkBegin, cbkEnd, true);
                //                method.Body.OptimizeMacros();
                //                method.Body.ComputeOffsets();
            }
        }

        [OTAPatch(SupportType.ClientServer, "Wrapping Item.netDefaults")]
        private void HookItemNetDefaults()
        {
            var setDefaults = Terraria.Item.Methods.Single(x => x.Name == "netDefaults");
        
            var apiMatch = API.ItemCallback.MatchInstanceMethodByParameters("Terraria.Item", setDefaults.Parameters, "OnNetDefaults");
            if (apiMatch.Count() != 2) throw new InvalidOperationException("There is no matching netDefaults Begin/End calls in the API");
        
            var cbkBegin = apiMatch.Single(x => x.Name.EndsWith("Begin"));
            var cbkEnd = apiMatch.Single(x => x.Name.EndsWith("End"));
        
            setDefaults.Wrap(cbkBegin, cbkEnd, true);
        }

        [OTAPatch(SupportType.Client, "Hooking Item creation")]
        private void HookItemCreation()
        {
            var method = Terraria.Item.Method("NewItem");
            var replacement = API.ItemCallback.Method("OnNewItem");

            var ctor = method.Body.Instructions.Where(x => x.OpCode == OpCodes.Newobj
                           && x.Operand is MethodReference
                           && (x.Operand as MethodReference).DeclaringType.Name == "Item").Skip(1).First();

            ctor.OpCode = OpCodes.Call;
            ctor.Operand = Terraria.Import(replacement);

            //Remove <npc>.SetDefault() as we do something custom
            var remFrom = ctor.Next;
            var il = method.Body.GetILProcessor();
            while (remFrom.Next.OpCode != OpCodes.Callvirt) //Remove until SetDefaults
            {
                il.Remove(remFrom.Next);
            }
            il.Remove(remFrom.Next);

            //Add Type to our callback
            il.InsertBefore(ctor, il.Create(OpCodes.Ldarg, method.Parameters[4]));
        }
    }
}

