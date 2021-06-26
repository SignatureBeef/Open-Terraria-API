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
using Microsoft.Xna.Framework;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using Terraria.Localization;

/// <summary>
/// @doc Creates Hooks.NetMessage.PlayerAnnounce. Allows plugins to intercept the vanilla join message.
/// </summary>
[Modification(ModType.PreMerge, "Hooking player announements (has join, has left)")]
void HookPlayerAnnouncements(ModFwModder modder)
{
    var syncOne = modder.GetReference(() => Terraria.NetMessage.SyncOnePlayer(0, 0, 0));
    var broadcast = modder.GetReference(() => Terraria.Chat.ChatHelper.BroadcastChatMessage(null, default, 0));
    var callback = modder.GetMethodDefinition(() => OTAPI.Callbacks.NetMessage.PlayerAnnounce(null, default, 0, 0, 0, 0));

    modder.OnRewritingMethodBody += (MonoModder modder, MethodBody body, Instruction instr, int instri) =>
    {
        if (instr.Operand is MethodReference mref
            && mref.FullName == broadcast.FullName
            && body.Method.FullName == syncOne.FullName
        )
        {
            foreach (var prm in body.Method.Parameters)
                body.GetILProcessor().InsertBefore(instr, Instruction.Create(OpCodes.Ldarg, prm));
            instr.Operand = callback;
        }
    };
}

namespace OTAPI.Callbacks
{
    public static partial class NetMessage
    {
        public static void PlayerAnnounce(NetworkText text, Color color, int excludedPlayer, int plr, int toWho, int fromWho)
        {
            var args = new Hooks.NetMessage.PlayerAnnounceEventArgs()
            {
                text = text,
                color = color,
                excludedPlayer = excludedPlayer,
                plr = plr,
                toWho = toWho,
                fromWh = fromWho,
            };
            var result = Hooks.NetMessage.InvokePlayerAnnounce(args);

            if (result != HookResult.Cancel)
                Terraria.Chat.ChatHelper.BroadcastChatMessage(args.text, args.color, args.excludedPlayer);
        }
    }
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NetMessage
        {
            public class PlayerAnnounceEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public int index { get; set; }
                public NetworkText text { get; set; }
                public Color color { get; set; }
                public int excludedPlayer { get; set; }
                public int plr { get; set; }
                public int toWho { get; set; }
                public int fromWh { get; set; }
            }
            public static event EventHandler<PlayerAnnounceEventArgs> PlayerAnnounce;

            public static HookResult? InvokePlayerAnnounce(PlayerAnnounceEventArgs args)
            {
                PlayerAnnounce?.Invoke(null, args);
                return args.Result;
            }
        }
    }
}