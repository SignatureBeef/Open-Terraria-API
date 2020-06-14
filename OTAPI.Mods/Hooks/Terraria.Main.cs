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
using Microsoft.Xna.Framework;

namespace OTAPI
{
    public static partial class Hooks
    {
        public static class Main
        {
            public delegate HookResult UpdateHandler(HookEvent @event, ref GameTime gameTime);
            public static UpdateHandler Update;

            public delegate HookResult InitializeHandler(HookEvent @event);
            public static InitializeHandler Initialize;
        }
    }
}
