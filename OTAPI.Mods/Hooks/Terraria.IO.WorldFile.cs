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

namespace OTAPI
{
    public enum HookEvent
    {
        Before,
        After
    }

    public static partial class Hooks
    {
        public static partial class IO
        {
            public static class WorldFile
            {
                public delegate HookResult LoadWorldHandler(HookEvent @event, ref bool loadFromCloud, Action<bool> originalMethod);
                public static LoadWorldHandler LoadWorld;

                public delegate HookResult SaveWorldHandler(HookEvent @event, ref bool useCloudSaving, ref bool resetTime, Action<bool, bool> originalMethod);
                public static SaveWorldHandler SaveWorld;
            }
        }
    }
}
