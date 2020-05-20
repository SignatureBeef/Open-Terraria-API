using Mono.Cecil.Cil;
using MonoMod.Cil;
using OTAPI.Extensions;
using System;
using System.Collections.Generic;

namespace OTAPI.Modifications.Patchtime
{
    [Modification("Modifying Terraria.WorldGen.hardUpdateWorld")]
    [MonoMod.MonoModIgnore]
    class HardmodeTileUpdate
    {
        public HardmodeTileUpdate()
        {
            //IL.Terraria.WorldGen.hardUpdateWorld += il =>
            //{
            //    var c = new ILCursor(il);

            //    /*
            //        Look through for all instances of
            //            // Main.tile[num15, num16].type = 199;
            //            IL_0a1a: ldsfld class Terraria.Tile[0..., 0...] Terraria.Main::tile
            //            IL_0a1f: ldloc.s 23
            //            IL_0a21: ldloc.s 24
            //            IL_0a23: callvirt instance class Terraria.Tile class Terraria.Tile[0..., 0...]::Get(int32, int32)
            //            IL_0a28: ldc.i4 199
            //            IL_0a2d: stfld uint16 Terraria.Tile::'type'
            //            // SquareTileFrame(num15, num16);
            //            IL_0a32: ldloc.s 23
            //            IL_0a34: ldloc.s 24
            //            IL_0a36: ldc.i4.1
            //            IL_0a37: call void Terraria.WorldGen::SquareTileFrame(int32, int32, bool)
            //            // NetMessage.SendTileSquare(-1, num15, num16, 1);
            //            IL_0a3c: ldc.i4.m1
            //            IL_0a3d: ldloc.s 23
            //            IL_0a3f: ldloc.s 24
            //            IL_0a41: ldc.i4.1
            //            IL_0a42: ldc.i4.0
            //            IL_0a43: call void Terraria.NetMessage::SendTileSquare(int32, int32, int32, int32, valuetype Terraria.ID.TileChangeType)
            //    */

            //    //var matches = new List<FindPatternResult>();
            //    //Instruction searchFrom = null;
            //    //FindPatternResult match;
            //    //while ((match = il.Body.FindPattern(searchFrom,
            //    //    OpCodes.Ldsfld,
            //    //    OpCodes.Ldloc_S,
            //    //    OpCodes.Ldloc_S,
            //    //    OpCodes.Callvirt,
            //    //    OpCodes.Ldc_I4,
            //    //    OpCodes.Stfld,
            //    //    OpCodes.Ldloc_S,
            //    //    OpCodes.Ldloc_S,
            //    //    OpCodes.Ldc_I4_1,
            //    //    OpCodes.Call,
            //    //    OpCodes.Ldc_I4_M1,
            //    //    OpCodes.Ldloc_S,
            //    //    OpCodes.Ldloc_S,
            //    //    OpCodes.Ldc_I4_1,
            //    //    OpCodes.Ldc_I4_0,
            //    //    OpCodes.Call
            //    //)).first != null)
            //    //{
            //    //    Console.WriteLine($"Replace from {match.first.OpCode}({match.first.Offset}) => {match.last.OpCode}({match.last.Offset})");
            //    //    matches.Add(match);
            //    //    searchFrom = match.last;
            //    //}

            //    //foreach (var m in matches)
            //    //{
            //    //    var current = m.last;
            //    //    while (current.Next != m.last)
            //    //    {
            //    //        il.IL.Remove(current.Next);
            //    //    }
            //    //    il.IL.Remove(current.Next);

            //    //    current.Operand = null;
            //    //    current.OpCode = OpCodes.Ret;
            //    //}
            //};
        }
    }
}
