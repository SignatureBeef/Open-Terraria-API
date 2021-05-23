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
using ModFramework;
using ModFramework.Relinker;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

[Modification(ModType.PreMerge, "Implementing ITile", Dependencies = new[]{
        "ITileCollection", // this mod uses Terraria.Tile directly so it will emit references to it that this needs to clean up
        "ITileProperties", // properties required first, interfaces do not like instance fields
    })]
void ITile(ModFwModder modder, IRelinkProvider relinkProvider)
{
    var tile = modder.GetDefinition<Terraria.Tile>();
    var itile = tile.RemapAsInterface(relinkProvider);

    // allow Terraria.Tile to be overridden 
    modder.MakeVirtual(tile);

    // replace ctor/new instances and route to a hook
    {
        var createTile = modder.Module.ImportReference(modder.GetMethodDefinition(() => OTAPI.Callbacks.Tile.Create()));
        var createTileRef = modder.Module.ImportReference(modder.GetMethodDefinition(() => OTAPI.Callbacks.Tile.Create(null)));

        modder.OnRewritingMethodBody += (MonoModder modder, MethodBody body, Instruction instr, int instri) =>
        {
            if (instr.OpCode == OpCodes.Newobj && instr.Operand is MethodReference mref
                && mref.DeclaringType.FullName == "Terraria.Tile"
                && !body.Method.DeclaringType.Namespace.StartsWith("OTAPI")
            )
            {
                if (mref.Parameters.Count == 0)
                {
                    instr.OpCode = OpCodes.Call;
                    instr.Operand = createTile;
                }
                else if (mref.Parameters.Count == 1)
                {
                    instr.OpCode = OpCodes.Call;
                    instr.Operand = createTileRef;
                }
                else throw new NotImplementedException();
            }
        };
    }
}

namespace OTAPI.Callbacks
{
    public static partial class Tile
    {
        public static Terraria.Tile Create()
        {
            return Hooks.Tile.Create?.Invoke() ?? new Terraria.Tile();
        }

        public static Terraria.Tile Create(Terraria.Tile existing)
        {
            return Hooks.Tile.Create?.Invoke(existing) ?? new Terraria.Tile(existing);
        }
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Tile
        {
            public delegate Terraria.Tile CreateHandler(Terraria.Tile existing = null);
            public static CreateHandler Create;
        }
    }
}