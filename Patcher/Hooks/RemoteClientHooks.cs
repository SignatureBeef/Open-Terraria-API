using System;
using System.Linq;
using Mono.Cecil.Cil;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [ServerHook]
        private void HookRemoteClientReset()
        {
            var reset = Terraria.RemoteClient.Method("Reset");
            var callback = Terraria.Import(API.RemoteClientCallback.Method("OnReset"));

            var il = reset.Body.GetILProcessor();
            var first = reset.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Call, callback));
        }
    }
}

