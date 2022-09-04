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
#pragma warning disable CS0436 // Type conflicts with imported type

#if tModLoader_V1_4
System.Console.WriteLine("Hardmode patch a not available in TML");
#else
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using System;
using System.Linq;

/// <summary>
/// @doc Creates Hooks.WorldGen.HardmodeTilePlace. Allows plugins to intercept hard mode tile placements.
/// </summary>
[Modification(ModType.PreMerge, "Hooking hardmode tile placements")]
[MonoMod.MonoModIgnore]
void HardModeTilePlacement(MonoModder modder)
{
    var csr = modder.GetILCursor(() => Terraria.WorldGen.hardUpdateWorld(0, 0));
    var callback = modder.GetMethodDefinition(() => OTAPI.Hooks.WorldGen.InvokeHardmodeTilePlace(0, 0, 0, false, false, 0, 0));

    var targets = csr.Body.Instructions.Where(instruction =>
        instruction.OpCode == OpCodes.Call
        && (instruction.Operand as MethodReference).Name == "PlaceTile"

        && instruction.Next.OpCode == OpCodes.Pop
    ).ToArray();

    if (targets.Length == 0)
        throw new Exception($"{nameof(Terraria.WorldGen.hardUpdateWorld)} is invalid");

    foreach (var replacementPoint in targets)
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
            public class HardmodeTilePlaceEventArgs : EventArgs
            {
                public HardmodeTileUpdateResult? Result { get; set; }

                public int X { get; set; }
                public int Y { get; set; }
                public int Type { get; set; }
                public bool Mute { get; set; }
                public bool Forced { get; set; }
                public int Plr { get; set; }
                public int Style { get; set; }
            }
            public static event EventHandler<HardmodeTilePlaceEventArgs> HardmodeTilePlace;

            public static bool InvokeHardmodeTilePlace(int x, int y, int type, bool mute, bool forced, int plr, int style)
            {
                var args = new Hooks.WorldGen.HardmodeTilePlaceEventArgs()
                {
                    X = x,
                    Y = y,
                    Type = type,
                    Mute = mute,
                    Forced = forced,
                    Plr = plr,
                    Style = style,
                };

                HardmodeTilePlace?.Invoke(null, args);

                if (args.Result == HardmodeTileUpdateResult.Cancel)
                    return false;

                else if (args.Result == HardmodeTileUpdateResult.Continue)
                    Terraria.WorldGen.PlaceTile(args.X, args.Y, args.Type, args.Mute, args.Forced, args.Plr, args.Style);

                return true;
            }
        }
    }
}
#endif