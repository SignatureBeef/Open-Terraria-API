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
#pragma warning disable CS8321 // Local function is declared but never used
#pragma warning disable CS0436 // Type conflicts with imported type

#if tModLoaderServer_V1_3
System.Console.WriteLine("Get data not available in TML1.3");
#else
using ModFramework;
using MonoMod;
using MonoMod.Cil;
using System;

[Modification(ModType.PreMerge, "Hooking Terraria.Main.Initialize")]
void HookGameInitialize(MonoModder modder)
{
    var initialize = modder.GetILCursor(() => new Terraria.Main().Initialize());

    initialize.EmitDelegate<InitializeCallback>(OTAPI.Hooks.Main.InitializeBegin);
    initialize.GotoNext(MoveType.After);
    initialize.EmitDelegate<InitializeCallback>(OTAPI.Hooks.Main.InitializeEnd);
}

// this is merely the callback signature 
[MonoMod.MonoModIgnore]
public delegate void InitializeCallback();

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Main
        {
            public class PreInitializeEventArgs : EventArgs
            {

            }

            public static event EventHandler<PreInitializeEventArgs> PreInitialize;

            public static void InitializeBegin()
            {
                PreInitialize.Invoke(null, new PreInitializeEventArgs());
            }

            public class PostInitializeEventArgs : EventArgs
            {

            }

            public static event EventHandler<PostInitializeEventArgs> PostInitialize;

            public static void InitializeEnd()
            {
                PostInitialize.Invoke(null, new PostInitializeEventArgs());
            }
        }
    }
}

#endif