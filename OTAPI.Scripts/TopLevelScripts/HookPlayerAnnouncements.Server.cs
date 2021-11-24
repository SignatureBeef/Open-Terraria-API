﻿/*
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

#if tModLoaderServer_V1_3
System.Console.WriteLine("Player announce not available in TML1.3");
#else
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
[Modification(ModType.PreMerge, "Hooking player announcements (has join, has left)")]
void HookPlayerAnnouncements(ModFwModder modder)
{
    var syncOne = modder.GetReference(() => Terraria.NetMessage.SyncOnePlayer(0, 0, 0));
    var broadcast = modder.GetReference(() => Terraria.Chat.ChatHelper.BroadcastChatMessage(null, default, 0));
    var callback = modder.GetMethodDefinition(() => OTAPI.Hooks.NetMessage.InvokePlayerAnnounce(null, default, 0, 0, 0, 0));

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
            var cursor = modder.GetILCursor(() => Terraria.NetMessage.SyncOnePlayer(0, 0, 0));
            cursor.Goto(instr.Next);
            cursor.RemoveRange(11);
        }
    };
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

                public int Index { get; set; }
                public NetworkText Text { get; set; }
                public Color Color { get; set; }
                public int ExcludedPlayer { get; set; }
                public int Plr { get; set; }
                public int ToWho { get; set; }
                public int FromWh { get; set; }
            }
            public static event EventHandler<PlayerAnnounceEventArgs> PlayerAnnounce;

            public static void InvokePlayerAnnounce(NetworkText text, Color color, int excludedPlayer, int plr, int toWho, int fromWho)
            {
                var args = new PlayerAnnounceEventArgs()
                {
                    Text = text,
                    Color = color,
                    ExcludedPlayer = excludedPlayer,
                    Plr = plr,
                    ToWho = toWho,
                    FromWh = fromWho,
                };
                PlayerAnnounce?.Invoke(null, args);

                if (args.Result != HookResult.Cancel)
                {
                    Terraria.Chat.ChatHelper.BroadcastChatMessage(args.Text, args.Color, args.ExcludedPlayer);
                    if (Terraria.Main.dedServ)
                    {
                        if (Terraria.Netplay.Clients[plr].State == 10)
                        {
                            Console.WriteLine(Terraria.Lang.mp[19].Format(Terraria.Netplay.Clients[plr].Name));
                        }
                        else
                        {
                            Console.WriteLine(Terraria.Lang.mp[20].Format(Terraria.Netplay.Clients[plr].Name));
                        }
                    }
                }
            }
        }
    }
}
#endif