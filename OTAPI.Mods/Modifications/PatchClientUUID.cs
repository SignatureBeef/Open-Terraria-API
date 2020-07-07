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
using Mono.Cecil.Cil;
using MonoMod;

namespace OTAPI.Modifications
{
    [Modification(ModType.PostPatch, "Patching in Client UUID")]
    [MonoMod.MonoModIgnore]
    class PatchClientUUID
    {
        const int PacketID = 68;

        public PatchClientUUID(MonoModder modder)
        {
            int messageType = 0;
            var GetData = modder.GetILCursor(() => new Terraria.MessageBuffer().GetData(0, 0, out messageType));
            var Callback = modder.GetMethodDefinition(() => Callbacks.MessageBuffer.ReadClientUUID(default, default, default, default, ref messageType));

            GetData.GotoNext(i => i.OpCode == OpCodes.Switch);

            var instructions = (Instruction[])GetData.Next.Operand;

            var packet = instructions[PacketID - 1];

            GetData.Goto(packet);

            /*
             *  Replace the ReadString call with the callback. Reference il:
             
	            // reader.ReadString();
	            IL_655e: ldarg.0        == GetData.Next
	            IL_655f: ldfld class [mscorlib]System.IO.BinaryReader Terraria.MessageBuffer::reader
	            IL_6564: callvirt instance string [mscorlib]System.IO.BinaryReader::ReadString()
	            IL_6569: pop
	            IL_656a: ret
            */

            GetData.Index++;
            GetData.Emit(OpCodes.Ldarg_0);

            GetData.Index++;

            foreach (var parameter in GetData.Method.Parameters)
            {
                GetData.Emit(OpCodes.Ldarg, parameter);
            }

            System.Diagnostics.Debug.Assert(GetData.Next.OpCode == OpCodes.Callvirt);
            GetData.Next.OpCode = OpCodes.Call;
            GetData.Next.Operand = GetData.Module.ImportReference(Callback);

            GetData.Index++;
            System.Diagnostics.Debug.Assert(GetData.Next.OpCode == OpCodes.Pop);
            GetData.Next.OpCode = OpCodes.Nop;
        }
    }
}
