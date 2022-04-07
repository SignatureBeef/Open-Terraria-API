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

using System.Linq;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;

/// <summary>
/// @doc Implements Terraria.Tile.Initialise in place of Terraria.Tile.ctor. This allows the port to an interface work much easier.
/// </summary>
[Modification(ModType.PreMerge, "Creating Tile.Initialise")]
[MonoMod.MonoModIgnore]
static void ModifyVanillaTile(ModFramework.ModFwModder modder)
{
    //Get the Terraria.Tile method definition
    var terrariaTile = modder.GetDefinition<Terraria.Tile>();

    //Create and add the new Tile method that will have the real constructor instructions.
    var mInitialise = new MethodDefinition("Initialise",
        MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.Virtual,
        modder.Module.TypeSystem.Void
    );
    terrariaTile.Methods.Add(mInitialise);

    //Find the tile reference that Terraria uses to create tiles with
    var ctor = terrariaTile.Methods.Single(x => x.Name == ".ctor" && !x.HasParameters);

    //Get the IL processor instance so we can modify the tile constructors il
    var il = ctor.Body.GetILProcessor();

    //Get all constructor instructions, skipping the first two as they are for
    //the underlying object. ie object.ctor
    var instructions = ctor.Body.Instructions.Skip(2)
        .ToArray(); //Get a clone

    //Remove the instructions from the constructor
    foreach (var instruction in instructions.Reverse())
        ctor.Body.Instructions.Remove(instruction);

    //Add the instructions into the new method
    foreach (var instruction in instructions)
        mInitialise.Body.Instructions.Add(instruction);

    //Add the new method call into the Tile construction
    ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
    ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, mInitialise));

    //Add the return instruction as per method requirements. We removed the existing one and put
    //it in the new method, so we need to put a new one back into the constructor too.
    ctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
}