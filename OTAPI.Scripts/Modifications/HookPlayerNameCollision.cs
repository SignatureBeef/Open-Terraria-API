using System.Linq;
using ModFramework;
using Mono.Cecil.Cil;
using MonoMod;

[Modification(ModType.PostPatch, "Hooking player name collisions")]
void HookPlayerNameCollision(MonoModder modder)
{
    int tmp;
    var csr = modder.GetILCursor(() => (new Terraria.MessageBuffer()).GetData(0, 0, out tmp));

    var flag = csr.Body.Instructions
        .Single(x => x.OpCode == OpCodes.Ldstr && x.Operand.Equals("Net.NameTooLong"))
        .Previous(y => y.OpCode == OpCodes.Brfalse_S);

    var player = flag.Next(x => x.OpCode == OpCodes.Ldloc_S);

    csr.Goto(flag, MonoMod.Cil.MoveType.After);

    csr.Emit(OpCodes.Ldloc_S, player.Operand as VariableDefinition);
    csr.EmitDelegate<PlayerNameCollisionCallback>(OTAPI.Callbacks.MessageBuffer.NameCollision);
    csr.Emit(OpCodes.Brfalse_S, flag.Operand as Instruction);
}

[MonoMod.MonoModIgnore]
public delegate bool PlayerNameCollisionCallback(Terraria.Player player);

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class MessageBuffer
        {
            public delegate HookResult NameCollisionHandler(Terraria.Player player);
            public static NameCollisionHandler NameCollision;
        }
    }
}

namespace OTAPI.Callbacks
{
    public static partial class MessageBuffer
    {
        public static bool NameCollision(Terraria.Player player)
        {
            return Hooks.MessageBuffer.NameCollision?.Invoke(player) != HookResult.Cancel;
        }
    }
}
