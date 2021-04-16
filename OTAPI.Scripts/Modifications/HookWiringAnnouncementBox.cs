using System;
using System.Linq;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;

[Modification(ModType.PreMerge, "Hooking wiring announce box")]
void HookWiringAnnounceBox(MonoModder modder)
{
    var csr = modder.GetILCursor(() => Terraria.Wiring.HitWireSingle(0, 0));

    if (csr.Method.Parameters.Count != 2)
        throw new NotSupportedException("Expected 2 parameters for the callback");

    var insertionPoint = csr.Body.Instructions.First(
        x => x.OpCode == OpCodes.Ldsfld
        && (x.Operand as FieldReference).Name == "AnnouncementBoxRange"
    );

    var signVariable = csr.Body.Instructions.First(
        x => x.OpCode == OpCodes.Call
        && (x.Operand as MethodReference).Name == "ReadSign"
    ).Next.Operand;

    csr.Goto(insertionPoint, MonoMod.Cil.MoveType.Before);

    var injectedInstructions = csr.EmitAll(
        new { OpCodes.Ldarg_0 },
        new { OpCodes.Ldarg_1 },
        new { OpCodes.Ldloc_S, Operand = signVariable as VariableDefinition }
    );

    csr.EmitDelegate<AnnouncementBoxCallback>(OTAPI.Callbacks.Wiring.AnnouncementBox);

    insertionPoint.ReplaceTransfer(injectedInstructions.First(), csr.Method);

    csr.EmitAll(
        new { OpCodes.Brtrue_S, insertionPoint },
        new { OpCodes.Ret }
    );
}

[MonoMod.MonoModIgnore]
public delegate bool AnnouncementBoxCallback(int x, int y, int signId);

namespace OTAPI.Callbacks
{
    public static partial class Wiring
    {
        public static bool AnnouncementBox(int x, int y, int signId)
            => Hooks.Wiring.AnnouncementBox?.Invoke(x, y, signId) != HookResult.Cancel;
    }
}


namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Wiring
        {
            public delegate HookResult AnnouncementBoxHandler(int x, int y, int signId);
            public static AnnouncementBoxHandler AnnouncementBox;
        }
    }
}
