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
#if !tModLoaderServer_V1_3
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
        
[Modification(ModType.PreMerge, "Hooking Terraria.GameContent.ItemDropRules.CommonCode.DropItemFromNPC")]
void HookNpcLoot(MonoModder modder)
{
    var NewNPC = modder.GetILCursor(() => Terraria.GameContent.ItemDropRules.CommonCode.DropItemFromNPC(default, default, default, default));

    NewNPC.GotoNext(
        i => i.OpCode == OpCodes.Call && i.Operand is MethodReference methodReference && methodReference.Name == "NewItem" && methodReference.DeclaringType.FullName == "Terraria.Item"
    );

    NewNPC.Emit(OpCodes.Ldarg_0); // NPC instance
    NewNPC.Next.Operand = modder.GetMethodDefinition(() => OTAPI.Callbacks.NPC.DropLoot(default, default, default, default, default, default, default, default, default, default, default));
}

public delegate bool NpcSpawnHandler(ref int index);

namespace OTAPI.Callbacks
{
    public static class NPC
    {
        public static int DropLoot(
                int X, int Y, int Width, int Height, int Type,
                int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup,
                Terraria.NPC instance)
        {
            int itemIndex = 0;
            if (Hooks.NPC.DropLoot?.Invoke(HookEvent.Before, instance, ref itemIndex, ref X, ref Y, ref Width, ref Height, ref Type, ref Stack, ref noBroadcast, ref pfix, ref noGrabDelay, ref reverseLookup) != HookResult.Cancel)
            {
                itemIndex = Terraria.Item.NewItem(X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
                Hooks.NPC.DropLoot?.Invoke(HookEvent.After, instance, ref itemIndex, ref X, ref Y, ref Width, ref Height, ref Type, ref Stack, ref noBroadcast, ref pfix, ref noGrabDelay, ref reverseLookup);
            }
            return itemIndex;
        }
    }
}
namespace OTAPI
{
    public static partial class Hooks
    {
        public static class NPC
        {
            public delegate HookResult DropLootHandler(HookEvent @event, Terraria.NPC instance, ref int itemIndex,
                ref int X, ref int Y, ref int Width, ref int Height, ref int Type,
                ref int Stack, ref bool noBroadcast, ref int pfix, ref bool noGrabDelay, ref bool reverseLookup
            );
            public static DropLootHandler DropLoot;
        }
    }
}


#endif