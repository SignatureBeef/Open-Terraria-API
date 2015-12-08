using System;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace OTA.Patcher
{
    public partial class Injector
    {
        [OTAPatch(SupportType.Server, "Hooking player kill messages")]
        void OnPlayerKilled() //OnEntityHurt
        {
            var mth = Terraria.Player.Methods.Single(x => x.Name == "KillMe");
            var hook = _asm.MainModule.Import(API.Player.Methods.Single(x => x.Name == "OnPlayerKilled"));

            var il = mth.Body.GetILProcessor();
            var first = mth.Body.Instructions.First();

            //Contruct the call to the API

            //Arguments
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Ldarga_S, mth.Parameters.Single(x => x.Name == "dmg")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarga_S, mth.Parameters.Single(x => x.Name == "hitDirection")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarga_S, mth.Parameters.Single(x => x.Name == "pvp")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarga_S, mth.Parameters.Single(x => x.Name == "deathText")));

            //Call
            il.InsertBefore(first, il.Create(OpCodes.Call, hook));
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ret));

            //Remove the Concat with the Entity name+death-message and leave the message
            //We will reproduce this in the API so plugins can have full control
            var matches = mth.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
                              && x.Operand is MethodReference
                              && (x.Operand as MethodReference).Name == "Concat")
                .Reverse() /* Remove IL from the bottom up */
                .ToArray();

            foreach (var match in matches)
            {
                il.Remove(match.Previous.Previous.Previous); //this
                il.Remove(match.Previous.Previous); //.name
                il.Remove(match); //call
            }
        }

        [OTAPatch(SupportType.Client | SupportType.Server, "Hooking player damage")]
        void OnPlayerHurt() //OnEntityHurt
        {
            //Routing all instances because i'm yet again in another rush
            //Anyone, feel free to swap out to pure IL directly in the deathMsg body ;)

            var mth = Terraria.Player.Methods.Single(x => x.Name == "Hurt");
            var hook = _asm.MainModule.Import(API.Player.Methods.Single(x => x.Name == "OnPlayerHurt"));

            var il = mth.Body.GetILProcessor();
            var first = mth.Body.Instructions.First();

            //Contruct the call to the API

            //Arguments
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_1));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_2));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, mth.Parameters.Single(x => x.Name == "pvp")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, mth.Parameters.Single(x => x.Name == "quiet")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, mth.Parameters.Single(x => x.Name == "deathText")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, mth.Parameters.Single(x => x.Name == "Crit")));
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_S, mth.Parameters.Single(x => x.Name == "cooldownCounter")));

            //Call
            il.InsertBefore(first, il.Create(OpCodes.Call, hook));
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ldc_R8, 0.0));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        [OTAPatch(SupportType.Server, "Routing player death messages")]
        void OnDeathMessage()
        {
            //Routing all instances because i'm yet again in another rush
            //Anyone, feel free to swap out to pure IL directly in the deathMsg body ;)

            //            var mth = Terraria.Lang.Methods.Single(x => x.Name == "deathMsg");
            var hook = _asm.MainModule.Import(API.VanillaHooks.Methods.Single(x => x.Name == "OnDeathMessage"));

            foreach (var type in _asm.MainModule.Types)
            {
                foreach (var mth in type.Methods)
                {
                    if (mth.Body != null)
                        foreach (var ins in mth.Body.Instructions)
                        {
                            var mr = ins.Operand as MethodReference;
                            if (mr != null && mr.Name == "deathMsg")
                            {
                                ins.Operand = hook;
                            }
                        }
                }
            }
        }

        [OTAPatch(SupportType.Server, "Hooking player server enter")]
        private void OnPlayerEntering()
        {
            var getData = Terraria.MessageBuffer.Methods.Single(x => x.Name == "GetData");
            var match = getData.Body.Instructions.First(x => x.Operand is String && x.Operand.Equals("Empty name."));
            var callback = API.VanillaHooks.Methods.Single(x => x.Name == "OnPlayerEntering");

            var il = getData.Body.GetILProcessor();

            //Find the second RETURN and insert our callback just before.
            int count = 0;
            while (count < 2)
            {
                match = match.Next;

                if (match.OpCode == OpCodes.Ret)
                    count++;
            }

            var playerObject = match.Previous;
            count = 0;
            while (count < 2)
            {
                playerObject = playerObject.Previous;

                if (playerObject.OpCode == OpCodes.Ldloc_S)
                    count++;
            }

            il.InsertBefore(match, il.Create(OpCodes.Ldloc, playerObject.Operand as VariableDefinition));
            il.InsertBefore(match, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        [OTAPatch(SupportType.Server, "Hooking player quit")]
        private void OnPlayerLeave()
        {
            var clientReset = Terraria.RemoteClient.Methods.Single(x => x.Name == "Reset");
            var callback = API.VanillaHooks.Methods.Single(x => x.Name == "OnPlayerLeave");

            //Find the first ldsfld where the player is being reset
            var ins = clientReset.Body.Instructions.First(x => x.OpCode == OpCodes.Ldsfld && x.Operand is FieldReference && (x.Operand as FieldReference).Name == "player");

            var il = clientReset.Body.GetILProcessor();

            //Insert the player reference on the stack
            il.InsertBefore(ins, il.Create(OpCodes.Ldsfld, ins.Operand as FieldReference)); //Static Player array
            il.InsertBefore(ins, il.Create(OpCodes.Ldarg_0)); //Current RemoteClient object
            il.InsertBefore(ins, il.Create(OpCodes.Ldfld, _asm.MainModule.Import(Terraria.RemoteClient.Fields.Single(x => x.Name == "Id")))); //Id (index) for use with the player array
            il.InsertBefore(ins, il.Create(OpCodes.Ldelem_Ref)); //Now load the array at the index

            //Call the hook with the player
            il.InsertBefore(ins, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
        }

        [OTAPatch(SupportType.Server, "Hooking player greet message")]
        private void OnGreetPlayer()
        {
            var greetPlayer = Terraria.NetMessage.Methods.Single(x => x.Name == "greetPlayer");
            var callback = API.VanillaHooks.Methods.Single(x => x.Name == "OnGreetPlayer");
            var il = greetPlayer.Body.GetILProcessor();

            var first = greetPlayer.Body.Instructions.First();

            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Call, _asm.MainModule.Import(callback)));
            il.InsertBefore(first, il.Create(OpCodes.Brtrue_S, first));
            il.InsertBefore(first, il.Create(OpCodes.Ret));
        }

        [OTAPatch(SupportType.Server, "Hooking player name collisions")]
        private void OnNameCollision()
        {
            var vanilla = Terraria.MessageBuffer.Method("GetData");
            var callback = Terraria.Import(API.Player.Method("OnNameCollision"));
            var il = vanilla.Body.GetILProcessor();

            //Find our targets
            var insNameIsTooLong = vanilla.Body.Instructions.Single(x => x.OpCode == OpCodes.Ldstr && x.Operand as string == "Name is too long.");
            var insBrFalseS = insNameIsTooLong.FindPreviousInstructionByOpCode(OpCodes.Brfalse_S); //Where we inject our code after
            var insPlayer = insNameIsTooLong.FindPreviousInstructionByOpCode(OpCodes.Ldloc_S).Operand as VariableDefinition; //For inserting the connecting player
            var insReturnTo = insBrFalseS.FindNextInstructionByOpCode(OpCodes.Ldc_I4_2); //Where to resume non-cancelled code

            il.InsertAfter(insBrFalseS, il.Create(OpCodes.Ret));
            il.InsertAfter(insBrFalseS, il.Create(OpCodes.Brtrue_S, insReturnTo));
            il.InsertAfter(insBrFalseS, il.Create(OpCodes.Call, callback));
            il.InsertAfter(insBrFalseS, il.Create(OpCodes.Ldfld, Terraria.MessageBuffer.Field("whoAmI")));
            il.InsertAfter(insBrFalseS, il.Create(OpCodes.Ldarg_0));
            il.InsertAfter(insBrFalseS, il.Create(OpCodes.Ldloc_S, insPlayer));
        }

        [OTAPatch(SupportType.Client, "Hooking player world enter")]
        private void OnClientEnterWorld()
        {
            var vanilla = Terraria.Player.Method("EnterWorld");
            var callback = Terraria.Import(API.Player.Method("OnClientEnterWorld"));
            var il = vanilla.Body.GetILProcessor();

            var first = vanilla.Body.Instructions.First();
            il.InsertBefore(first, il.Create(OpCodes.Ldarg_0));
            il.InsertBefore(first, il.Create(OpCodes.Call, callback));
        }
    }
}

