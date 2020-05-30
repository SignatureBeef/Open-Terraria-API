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
using System.Linq.Expressions;
using Mono.Cecil;
using MonoMod.Cil;

namespace OTAPI
{
    public static class Extensions
    {
        public static ILCursor GetILCursor(this MonoMod.MonoModder modder, Expression<Action> reference)
            => new ILCursor(new ILContext(modder.Module.GetReference(reference)));
        public static MethodDefinition GetReference(this MonoMod.MonoModder modder, Expression<Action> reference)
            => modder.Module.GetReference(reference);

        public static MethodDefinition GetReference(this IMetadataTokenProvider token, Expression<Action> reference)
        {
            // find the expression method in the meta, matching on the parameter count/types
            var mce = reference.Body as MethodCallExpression;
            if (mce != null)
            {
                var module = (token as ModuleDefinition) ?? (token as AssemblyDefinition)?.MainModule;
                if (module != null)
                {
                    var type = module.Types.Single(t => t.FullName == mce.Method.DeclaringType.FullName);
                    if (type != null)
                    {
                        var targetParameters = mce.Method.GetParameters();
                        foreach (var method in type.Methods.Where(m => m.Name == mce.Method.Name))
                        {
                            if (method.Parameters.Count == targetParameters.Length)
                            {
                                if (method.Parameters.Count > 0)
                                {
                                    for (var i = 0; i < method.Parameters.Count; i++)
                                    {
                                        if (method.Parameters[i].ParameterType.FullName != targetParameters[i].ParameterType.FullName)
                                        {
                                            continue;
                                        }
                                    }
                                }

                                return method;
                            }
                        }
                    }
                }
            }

            throw new System.Exception($"Unable to find expression in assembly");
        }
    }
}