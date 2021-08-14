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
using Terraria;
using ImGuiNET;

bool active = true;
int npcs = 0;

var timer = new System.Timers.Timer();
timer.AutoReset = true;
timer.Interval = 2000;
timer.Elapsed += (s, e) =>
{
    npcs = Main.npc.Where(n => n.active).Count();
};
timer.Enabled = true;

void OnGUI(On.Terraria.Main.orig_OnExtGUI orig)
{
    orig();

    if (active)
    {
        ImGui.Begin("CSharp Script", ref active);
        ImGui.Text($"NPCs: {npcs}");
        ImGui.End();
    }
}

On.Terraria.Main.OnExtGUI += OnGUI;

Dispose = () =>
{
    On.Terraria.Main.OnExtGUI -= OnGUI;
    timer?.Dispose();
    timer = null;
};