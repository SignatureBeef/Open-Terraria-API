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
using System;
using System.IO;

namespace CSharpScript
{
    [OTAPI.Modification(OTAPI.ModType.Read, "Loading CSharpScript interface")]
    public class CSharpScript
    {
        public CSharpScript()
        {
            System.Console.WriteLine($"[CSS] Starting runtime");

            LoadPlugins();
        }

        void LoadPlugins()
        {
            if (Directory.Exists("csharp"))
            {
                foreach (var file in Directory.EnumerateFiles("csharp", "*.cs", SearchOption.AllDirectories))
                {
                    System.Console.WriteLine($"[CSS] Loading plugin: {file}");
                    try
                    {
                        var contents = File.ReadAllText(file);
                        var result = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.EvaluateAsync(contents).Result;
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
