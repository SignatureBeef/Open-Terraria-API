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
using System.Linq.Expressions;
using ModFramework;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;

[Modification(ModType.PreMerge, "Hooking statue spawning")]
void HookMechSpawn(MonoModder modder)
{
    foreach (var task in new[]
    {
            new Task()
            {
                Method = () => Terraria.NPC.MechSpawn(0,0,0),
                Callback =   OTAPI.Callbacks.NPC.MechSpawn,
            },
            new Task()
            {
                Method = () => Terraria.Item.MechSpawn(0, 0, 0),
                Callback = OTAPI.Callbacks.Item.MechSpawn,
            },
        })
    {
        var csr = modder.GetILCursor(task.Method);

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

                csr.EmitDelegate(task.Callback);
            }
        }
    }
}

[MonoModIgnore]
class Task
{
    public Expression<Action> Method { get; set; }
    public MechSpawnCallback Callback { get; set; }
}

// this is merely the callback signature 
[MonoMod.MonoModIgnore]
public delegate bool MechSpawnCallback(bool result, float x, float y, int type, int num, int num2, int num3);

namespace OTAPI.Callbacks
{
    public static partial class NPC
    {
        public static bool MechSpawn(bool result, float x, float y, int type, int num, int num2, int num3)
        {
            if (result)
            {
                var args = new Hooks.NPC.MechSpawnEventArgs()
                {
                    x = x,
                    y = y,
                    type = type,
                    num = num,
                    num2 = num2,
                    num3 = num3,
                };
                return Hooks.NPC.InvokeMechSpawn(args) != HookResult.Cancel;
            }
            return result;
        }
    }

    public static partial class Item
    {
        public static bool MechSpawn(bool result, float x, float y, int type, int num, int num2, int num3)
        {
            if (result)
            {
                var args = new Hooks.Item.MechSpawnEventArgs()
                {
                    x = x,
                    y = y,
                    type = type,
                    num = num,
                    num2 = num2,
                    num3 = num3,
                };
                return Hooks.Item.InvokeMechSpawn(args) != HookResult.Cancel;
            }
            return result;
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

                public float x { get; set; }
                public float y { get; set; }
                public int type { get; set; }
                public int num { get; set; }
                public int num2 { get; set; }
                public int num3 { get; set; }
            }
            public static event EventHandler<MechSpawnEventArgs> MechSpawn;

            public static HookResult? InvokeMechSpawn(MechSpawnEventArgs args)
            {
                MechSpawn?.Invoke(null, args);
                return args.Result;
            }
        }

        public static partial class Item
        {
            public class MechSpawnEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public float x { get; set; }
                public float y { get; set; }
                public int type { get; set; }
                public int num { get; set; }
                public int num2 { get; set; }
                public int num3 { get; set; }
            }
            public static event EventHandler<MechSpawnEventArgs> MechSpawn;

            public static HookResult? InvokeMechSpawn(MechSpawnEventArgs args)
            {
                MechSpawn?.Invoke(null, args);
                return args.Result;
            }
        }
    }
}