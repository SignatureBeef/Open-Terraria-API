using System;
using System.Linq;
using System.Linq.Expressions;
using ModFramework;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Cil;

partial class Development
{
    [MonoModIgnore]
    class Task
    {
        public Expression<Action> Method { get; set; }
        public MechSpawnCallback Callback { get; set; }
    }

    [Modification(ModType.PreMerge, "Hooking statue spawning")]
    static void HookStatueSpawning(MonoModder modder)
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
                return Hooks.NPC.MechSpawn?.Invoke(x, y, type, num, num2, num3) != HookResult.Cancel;
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
                return Hooks.Item.MechSpawn?.Invoke(x, y, type, num, num2, num3) != HookResult.Cancel;
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
            public delegate HookResult MechSpawnHandler(float x, float y, int type, int num, int num2, int num3);
            public static MechSpawnHandler MechSpawn;
        }

        public static partial class Item
        {
            public delegate HookResult MechSpawnHandler(float x, float y, int type, int num, int num2, int num3);
            public static MechSpawnHandler MechSpawn;
        }
    }
}