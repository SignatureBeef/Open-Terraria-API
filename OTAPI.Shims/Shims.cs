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
using MonoMod;

/* Forward all shims to the Shims mod DLL. As part of MonoMod, any mod DLLs will be merged into the final assembly */

[assembly: AssemblyRedirector("Microsoft.Xna.Framework", "TerrariaServer")]
[assembly: AssemblyRedirector("Microsoft.Xna.Framework.Game", "TerrariaServer")]
[assembly: AssemblyRedirector("Microsoft.Xna.Framework.Graphics", "TerrariaServer")]
[assembly: AssemblyRedirector("Microsoft.Xna.Framework.Xact", "TerrariaServer")]
[assembly: AssemblyRedirector("System.Windows.Forms", "TerrariaServer")]
[assembly: AssemblyRedirector("System.Drawing.Graphics", "TerrariaServer")]
[assembly: AssemblyRedirector("ReLogic", "TerrariaServer")]
[assembly: AssemblyRedirector("Steamworks.NET", "TerrariaServer")]