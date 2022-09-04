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
#pragma warning disable CS8321 // Local function is declared but never used

using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using System.Linq;
using System.Runtime.ConstrainedExecution;

#if tModLoaderServer_V1_3 || tModLoader_V1_4
System.Console.WriteLine("BroadcastThread not available in TML");
#else
/// <summary>
/// @doc Fix Terraria.Netplay.BroadcastThread infinite loop.
/// </summary>
[Modification(ModType.PostPatch, "Patching Netplay Broadcast")]
[MonoMod.MonoModIgnore]
void PatchNetplayBroadcast(MonoModder modder)
{
    const string Name_BroadcastThreadActive = nameof(Terraria.patch_Netplay.BroadcastThreadActive);
    var BroadcastThread = modder.GetILCursor(() => Terraria.Netplay.BroadcastThread());
    var BroadcastThreadActive = BroadcastThread.Method.DeclaringType.Fields.Single(m => m.Name == Name_BroadcastThreadActive);

    BroadcastThread.GotoNext(
        i => i.OpCode == OpCodes.Call
            && i.Operand is MethodReference methodReference
            && methodReference.Name == "Sleep"
            && i.Next.OpCode == OpCodes.Br_S
    );

    BroadcastThread.Index++;

    BroadcastThread.Emit(OpCodes.Ldsfld, BroadcastThreadActive);
    BroadcastThread.Next.OpCode = OpCodes.Brtrue;
    BroadcastThread.Index++;
    BroadcastThread.Emit(OpCodes.Ret);
}

namespace Terraria
{
    partial class patch_Netplay : Terraria.Netplay
    {
        /** Begin Fix - Thread abort */
        public static bool BroadcastThreadActive = false;
        public extern static void orig_BroadcastThread();
        public static void BroadcastThread()
        {
            BroadcastThreadActive = true;
            orig_BroadcastThread();
        }

        public extern static void orig_StopBroadCasting();
        public static void StopBroadCasting()
        {
            BroadcastThreadActive = false;
            Terraria.Netplay.broadcastThread.Join();
            Terraria.Netplay.broadcastThread = null;

            orig_StopBroadCasting();
        }
        /** End Fix - Thread abort */
    }
}
#endif
