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
using Microsoft.CodeAnalysis.Scripting;
using OTAPI.Mods.Relinker;
using System;
using System.IO;

namespace CSharpScript
{
    public class ScriptGlobals
    {
        // undecided on this yet
        public void RegisterModification(OTAPI.ModType type, string name, Action callback)
        {
            Console.WriteLine($"Added mod: {type}, {name}");
            callback();
        }
    }

    [OTAPI.Modification(OTAPI.ModType.Read, "Loading CSharpScript interface")]
    public class CSharpScript
    {
        public CSharpScript()
        {
            System.Console.WriteLine($"[CSS] Starting runtime");

            if (Directory.Exists("csharp"))
            {
                foreach (var file in Directory.EnumerateFiles("csharp", "*.cs", SearchOption.AllDirectories))
                {
                    System.Console.WriteLine($"[CSS] Loading plugin: {file}");
                    try
                    {
                        var contents = File.ReadAllText(file);
                        var script = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(contents,
                            options: ScriptOptions.Default
                            .WithReferences(typeof(OTAPI.ModType).Assembly, typeof(IRelinkProvider).Assembly, typeof(MonoMod.MonoModder).Assembly),
                            globalsType: typeof(ScriptGlobals)
                        );

                        _ = script.RunAsync(new ScriptGlobals()).Result;
                    }
                    catch (CompilationErrorException e)
                    {
                        Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine($"[CSS] Load error: {ex}");
                    }
                }
            }
        }
    }
}
