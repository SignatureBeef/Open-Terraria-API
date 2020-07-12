/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
#if TerrariaServer_V1_4
using ModFramework;
using Mono.Cecil.Cil;
using MonoMod;

namespace OTAPI.Modifications
{
    [Modification(ModType.PreMerge, "Hooking Terraria.MessageBuffer.GetData")]
    [MonoMod.MonoModIgnore]
    class HookClientGetData
    {
        public delegate bool GetDataHandler(global::Terraria.MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets);

        public HookClientGetData(MonoModder modder)
        {
            int temp = 0;
            var GetData = modder.GetILCursor(() => (new Terraria.MessageBuffer()).GetData(0, 0, out temp));

            GetData.GotoNext(
                //if (b >= 140) { return; }
                i => i.OpCode == OpCodes.Ldloc_0
                , i => i.OpCode == OpCodes.Ldc_I4
                , i => i.OpCode == OpCodes.Blt_S
                , i => i.OpCode == OpCodes.Ret
            );
            var maxPackets = (int)GetData.Instrs[GetData.Index + 1].Operand;

            GetData.RemoveRange(4);

            GetData.Emit(OpCodes.Ldarg_0);
            GetData.Emit(OpCodes.Ldloca, GetData.Body.Variables[0]); // packetID
            GetData.Emit(OpCodes.Ldloca, GetData.Body.Variables[1]); // readOffset

            foreach (var prm in GetData.Method.Parameters)
                GetData.Emit(prm.IsOut ? OpCodes.Ldarg : OpCodes.Ldarga, prm);

            GetData.Emit(OpCodes.Ldc_I4, maxPackets);
            GetData.EmitDelegate<GetDataHandler>(Callbacks.MessageBuffer.GetData);
            GetData.Emit(OpCodes.Brtrue, GetData.Instrs[GetData.Index]);
            GetData.Emit(OpCodes.Ret);
        }
    }
}
#endif