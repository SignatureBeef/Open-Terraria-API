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

using System;
using System.Linq;
using System.Linq.Expressions;
using ModFramework;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;

/// <summary>
/// @doc Creates Hooks.NPC.MechSpawn & Hooks.Item.MechSpawn. Allows plugins to react to Mech spawn events.
/// </summary>
[Modification(ModType.PreMerge, "Hooking statue spawning")]
[MonoModIgnore]
void HookMechSpawn(MonoModder modder)
{
    HookMethod(modder, () => Terraria.NPC.MechSpawn(0, 0, 0), OTAPI.Hooks.NPC.InvokeMechSpawn);
    HookMethod(modder, () => Terraria.Item.MechSpawn(0, 0, 0), OTAPI.Hooks.Item.InvokeMechSpawn);
}

[MonoModIgnore]
void HookMethod(MonoModder modder, Expression<Action> method, Func<bool, float, float, int, int, int, int, bool> Callback)
{
    var csr = modder.GetILCursor(method);

    //while (csr.TryFindNext(out ILCursor[] matches, ins => ins.OpCode == OpCodes.Ret))
    var matches = csr.Body.Instructions.Where(x => x.OpCode == OpCodes.Ret).ToArray();
    {
        foreach (var ret in matches)
        {
            csr.Goto(ret, MoveType.Before);

            // add the method params to the stack.
            foreach (var prm in csr.Method.Parameters)
                csr.Emit(OpCodes.Ldarg, prm);

            var www = csr.Body.Variables.Where(x => x.VariableType == csr.Module.TypeSystem.Int32).Take(3);
            if (www.Count() != 3)
                throw new Exception($"{csr.Method.FullName} was expected to contain 3 integer variables.");

            foreach (var lv in www)
                csr.Emit(OpCodes.Ldloc, lv);

            csr.EmitDelegate(Callback);
        }
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public class MechSpawnEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public float X { get; set; }
                public float Y { get; set; }
                public int Type { get; set; }
                public int Num { get; set; }
                public int Num2 { get; set; }
                public int Num3 { get; set; }
            }
            public static event EventHandler<MechSpawnEventArgs> MechSpawn;

            public static bool InvokeMechSpawn(bool result, float x, float y, int type, int num, int num2, int num3)
            {
                if (result)
                {
                    var args = new MechSpawnEventArgs()
                    {
                        X = x,
                        Y = y,
                        Type = type,
                        Num = num,
                        Num2 = num2,
                        Num3 = num3,
                    };
                    MechSpawn?.Invoke(null, args);
                    return args.Result != HookResult.Cancel;
                }
                return result;
            }
        }

        public static partial class Item
        {
            public class MechSpawnEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public float X { get; set; }
                public float Y { get; set; }
                public int Type { get; set; }
                public int Num { get; set; }
                public int Num2 { get; set; }
                public int Num3 { get; set; }
            }
            public static event EventHandler<MechSpawnEventArgs> MechSpawn;

            public static bool InvokeMechSpawn(bool result, float x, float y, int type, int num, int num2, int num3)
            {
                if (result)
                {
                    var args = new MechSpawnEventArgs()
                    {
                        X = x,
                        Y = y,
                        Type = type,
                        Num = num,
                        Num2 = num2,
                        Num3 = num3,
                    };
                    MechSpawn?.Invoke(null, args);
                    return args.Result != HookResult.Cancel;
                }
                return result;
            }
        }
    }
}