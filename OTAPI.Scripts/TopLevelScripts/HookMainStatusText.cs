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
System.Console.WriteLine("Main.statusText not available in TML1.3");
#else
using System;
using ModFramework;
using ModFramework.Relinker;
using Mono.Cecil;
using MonoMod;

/// <summary>
/// @doc A mod to create Hooks.Main.StatusTextUpdate. Allows plugins to receive writes to Main.statusText.
/// </summary>
[Modification(ModType.PreMerge, "Hooking Main.statusText")]
[MonoMod.MonoModIgnore]
void HookMainStatusText(MonoModder modder, IRelinkProvider relinkProvider)
{
    var field = modder.GetFieldDefinition(() => Terraria.Main.statusText);
    var property = field.RemapAsProperty(relinkProvider);

    // insert a method to allow plugins to intercept writes.
    // tried using monomod hooks on the setmethod but doesnt always work.
    var csr = modder.GetILCursor(property.SetMethod);
    csr.GotoNext(MonoMod.Cil.MoveType.Before, i => i.Operand is FieldReference fr && fr.Name.Contains("__BackingField"));

    csr.EmitDelegate(OTAPI.Hooks.Main.InvokeStatusTextChange);
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class Main
        {
            public class StatusTextChangeArgs : EventArgs
            {
                public string Value { get; set; }
            }
            public static event EventHandler<StatusTextChangeArgs> StatusTextChange;

            public static string InvokeStatusTextChange(string value)
            {
                var args = new Hooks.Main.StatusTextChangeArgs()
                {
                    Value = value
                };
                StatusTextChange?.Invoke(null, args);
                return args.Value;
            }
        }
    }
}
#endif