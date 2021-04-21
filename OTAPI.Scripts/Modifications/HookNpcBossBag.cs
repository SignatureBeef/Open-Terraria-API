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
using System;
using System.Linq;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;

[Modification(ModType.PreMerge, "Hooking npc boss bags")]
void HookNpcBossBag(ModFramework.ModFwModder modder)
{
    // replace NewItem calls, and handle the -1 result to cancel the method from actioning.

    var csr = modder.GetILCursor(() => (new Terraria.NPC()).DropItemInstanced(default, default, 0, 0, false));
    var callback = csr.Module.ImportReference(
        modder.GetMethodDefinition(() => OTAPI.Callbacks.NPC.BossBag(0, 0, 0, 0, 0, 0, false, 0, false, false, null))
    );

    var instructions = csr.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
                                                        && x.Operand is MethodReference
                                                        && (x.Operand as MethodReference).Name == "NewItem"
                                                        && x.Next.OpCode == OpCodes.Stloc_0);

    if (instructions.Count() != 1) throw new NotSupportedException("Only one server NewItem call expected in DropBossBags.");

    var ins = instructions.First();

    ins.Operand = callback;

    csr.Goto(ins);
    csr.EmitAll(
        new { OpCodes.Ldarg_0 }
    );

    csr.Goto(ins.Next.Next);
    csr.EmitAll(
        new { OpCodes.Ldloc_0 },
        new { OpCodes.Ldc_I4_M1 },
        new { OpCodes.Ceq },
        new { OpCodes.Brfalse_S, Operand = ins.Next.Next },
        new { OpCodes.Ret }
    );
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public delegate HookResult BossBagHandler(Terraria.NPC npc,
                ref int X,
                ref int Y,
                ref int Width,
                ref int Height,
                ref int Type,
                ref int Stack,
                ref bool noBroadcast,
                ref int pfix,
                ref bool noGrabDelay,
                ref bool reverseLookup);
            public static BossBagHandler BossBag;
        }
    }
}

namespace OTAPI.Callbacks
{
    public static partial class NPC
    {
        public static int BossBag(
            int X,
            int Y,
            int Width,
            int Height,
            int Type,
            int Stack,
            bool noBroadcast,
            int pfix,
            bool noGrabDelay,
            bool reverseLookup,
            Terraria.NPC npc)
        {
            if (Hooks.NPC.BossBag?.Invoke(npc, ref X, ref Y, ref Width, ref Height, ref Type, ref Stack, ref noBroadcast, ref pfix, ref noGrabDelay, ref reverseLookup) == HookResult.Cancel)
                return -1;

            return Terraria.Item.NewItem(X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
        }
    }
}
