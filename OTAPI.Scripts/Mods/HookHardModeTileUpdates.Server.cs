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
System.Console.WriteLine("Hardmode patch b not available in TML");
#else
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;
using System;
using System.Linq;

/// <summary>
/// @doc Creates Hooks.WorldGen.HardmodeTileUpdate. Allows plugins to intercept hard mode tile updates.
/// </summary>
[Modification(ModType.PreMerge, "Hooking hardmode tile updates")]
[MonoMod.MonoModIgnore]
void HardModeTileUpdates(MonoModder modder)
{
    var csr = modder.GetILCursor(() => Terraria.WorldGen.hardUpdateWorld(0, 0));

    // from v1.4.5, direct tile type updates are now wrapped inside the WorldGen.Convert method
    var targets = csr.Body.Instructions.Where(ins =>
        ins.OpCode == OpCodes.Call && 
        ins.Operand is MethodReference methodRef &&
        methodRef.DeclaringType.FullName == "Terraria.WorldGen" &&
        methodRef.Name == "Convert"
        ).ToArray();

    foreach (var match in targets)
    {
        csr.Goto(match);

        // trace back to find the tile position being pushed onto on the stack as an argument for WorldGen.Convert method
        // we will use this to correctly find the correct branch to insert our hook
        csr.FindPrev(out ILCursor[] tileLoadCursors, 
            xIns => xIns.OpCode == OpCodes.Ldloc_S, 
            yIns => yIns.OpCode == OpCodes.Ldloc_S
            );

        if (tileLoadCursors.Length != 2)
            throw new Exception($"{nameof(Terraria.WorldGen.hardUpdateWorld)} unable to find tile position for WorldGen.Convert");

        var convertArgsStart = tileLoadCursors[0].Next;

        // move back to the tile position being pushed onto the stack, we will wrap this up until WorldGen.Convert
        csr.GotoPrev(MoveType.After, ins =>
            ins.OpCode.FlowControl == FlowControl.Cond_Branch && 
            ins.Operand != convertArgsStart // this is to also include the WorldGen.genRand.Next branch
            );

        // store this for later. otherwise we need to cycle back to here, so its easier to just store it upfront
        var startOfBranch = csr.Next;

        // find the instruction to where our cancel code should end up.
        csr.FindNext(out ILCursor[] continuation,  ins =>
            ins.OpCode == OpCodes.Call &&
            ins.Operand is MethodReference methodRef &&
            methodRef.DeclaringType.FullName == "Terraria.WorldGen" && 
            methodRef.Name == "Convert"
            );

        if (continuation.Length != 1)
            throw new Exception($"{nameof(Terraria.WorldGen.hardUpdateWorld)} unable to determine continuation branch.");

        var continueBranch = continuation[0].Next.Next;

        // nop as a placeholder. it must take the transfer from other branches otherwise the if conditions won't align correctly.
        var nop = csr.Emit(OpCodes.Nop).Prev;
        startOfBranch.ReplaceTransfer(nop, csr.Method);

        // add the x, y, biomeConversionType, walls, tiles (params)
        var param = convertArgsStart;
        while (param.OpCode.FlowControl != FlowControl.Call)
        {
            csr.Emit(param.OpCode, param.Operand);
            param = param.Next;
        }

        csr.EmitDelegate(OTAPI.Hooks.WorldGen.InvokeHardmodeTileUpdate);
        csr.Emit(OpCodes.Brfalse_S, continueBranch);
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class WorldGen
        {
            public class HardmodeTileUpdateEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public int X { get; set; }
                public int Y { get; set; }
                public int BiomeConversionType { get; set; }
                public bool Walls { get; set; }
                public bool Tiles { get; set; }
            }
            public static event EventHandler<HardmodeTileUpdateEventArgs> HardmodeTileUpdate;

            public static bool InvokeHardmodeTileUpdate(int x, int y, int biomeConversionType, bool walls, bool tiles)
            {
                var args = new HardmodeTileUpdateEventArgs()
                {
                    X = x,
                    Y = y,
                    BiomeConversionType = biomeConversionType,
                    Walls = walls,
                    Tiles = tiles,
                };
                HardmodeTileUpdate?.Invoke(null, args);
                return args.Result != HookResult.Cancel;
            }
        }
    }
}
#endif