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

using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

/// <summary>
/// @doc Creates Hooks.Main.Create. Allows plugins to extend and return a custom Terraria.Main instance.
/// </summary>
[Modification(ModType.PostPatch, "Hooking new Main calls", ModPriority.Last)]
[MonoMod.MonoModIgnore]
void HookMainCtor(MonoModder modder)
{
#if Terraria_1442_OrAbove
    var LaunchGame = modder.GetILCursor(() => Terraria.Program.RunGame());
#else
    var LaunchGame = modder.GetILCursor(() => Terraria.Program.LaunchGame(null, false));
#endif

    var createGame = modder.GetMethodDefinition(() => OTAPI.Hooks.Main.InvokeCreate());

    LaunchGame.GotoNext(MonoMod.Cil.MoveType.Before,
        // active = false;
        i => i.OpCode == OpCodes.Newobj
            && i.Operand is MethodReference mref
            && mref.DeclaringType.Name == "Main"
            && mref.Name == ".ctor"
    );

    LaunchGame.Next.OpCode = OpCodes.Call;
    LaunchGame.Next.Operand = createGame;
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Main
        {
            // i dont think a event is a good idea for this one
            public delegate Terraria.Main CreateHandler();
            public static CreateHandler Create;

            public static Terraria.Main InvokeCreate()
            {
                return Hooks.Main.Create?.Invoke() ?? new Terraria.Main();
            }
        }
    }
}