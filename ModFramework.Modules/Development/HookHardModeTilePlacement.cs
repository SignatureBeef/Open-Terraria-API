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
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using System;
using System.Linq;

class ASD
{
    [Modification(ModType.PreMerge, "Hooking hardmode tile placements")]
    static void HardModeTilePlacement(MonoModder modder)
    {
        var csr = modder.GetILCursor(() => Terraria.WorldGen.hardUpdateWorld(0, 0), followRedirect: true);
        var callback = modder.GetMethodDefinition(() => OTAPI.Callbacks.WorldGen.HardmodeTilePlace(0, 0, 0, false, false, 0, 0));

        /* In this particular hardmode tile mod we replace all WorldGen.PlaceTile
         * calls to a custom callback version, then replace the Pop instruction
         * with cancelable IL.
         */

        var targets = csr.Body.Instructions.Where(instruction =>
            instruction.OpCode == OpCodes.Call
            && (instruction.Operand as MethodReference).Name == "PlaceTile"

            && instruction.Next.OpCode == OpCodes.Pop
        ).ToArray();

        if (targets.Length == 0)
            throw new Exception($"{nameof(Terraria.WorldGen.hardUpdateWorld)} is invalid");

        Console.WriteLine("targets: " + targets.Length);

        //var processor = vanilla.Body.GetILProcessor();

        foreach (var replacementPoint in targets)//.Take(1))
        {
            replacementPoint.Operand = callback;

            var newcsr = csr.Goto(replacementPoint, MoveType.After);

            var ins_pop = csr.Next;
            if (ins_pop.OpCode != OpCodes.Pop)
                throw new Exception($"{nameof(Terraria.WorldGen.hardUpdateWorld)} expected POP instruction");

            csr.GotoNext(MoveType.After, ins => ins.OpCode == OpCodes.Call && (ins.Operand as MethodReference).Name == "SendTileSquare");

            var continueOn = csr.Next;

            // change the POP instruction to SKIP the SendTileSquare if false was returned
            ins_pop.OpCode = OpCodes.Brfalse_S;
            ins_pop.Operand = continueOn;
        }
    }
}

namespace OTAPI.Callbacks
{
    public static partial class WorldGen
    {
        public static bool HardmodeTilePlace(int x, int y, int type, bool mute, bool forced, int plr, int style)
        {
            var result = Hooks.WorldGen.HardmodeTilePlace?.Invoke(ref x, ref y, ref type, ref mute, ref forced, ref plr, ref style);

            if (result == HardmodeTileUpdateResult.Cancel)
                return false;

            else if (result == HardmodeTileUpdateResult.Continue)
            {
                Terraria.WorldGen.PlaceTile(x, y, type, mute, forced, plr, style);
                if (Terraria.Main.netMode == 2)
                    Terraria.NetMessage.SendTileSquare(-1, x, y, 1);
            }

            return true;
        }
    }
}

namespace OTAPI
{
    public enum HardmodeTileUpdateResult
    {
        /// <summary>
        /// Continue to update the tile
        /// </summary>
        Continue,

        /// <summary>
        /// Cancel updating the tile
        /// </summary>
        Cancel,

        /// <summary>
        /// Continue vanilla code, but don't update the tile
        /// </summary>
        ContinueWithoutUpdate
    }

    public static partial class Hooks
    {
        public static partial class WorldGen
        {
            public delegate HardmodeTileUpdateResult HardmodeTilePlaceHandler(ref int x, ref int y, ref int type, ref bool mute, ref bool forced, ref int plr, ref int style);
            public static HardmodeTilePlaceHandler HardmodeTilePlace;
        }
    }
}