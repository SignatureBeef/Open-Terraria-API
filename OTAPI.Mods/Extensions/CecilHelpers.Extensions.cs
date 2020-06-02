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
    public static partial class Extensions
    {
        public static ILCursor GetILCursor(this MonoMod.MonoModder modder, Expression<Action> reference)
            => new ILCursor(new ILContext(modder.Module.GetReference<MethodDefinition>(reference)));

        public static FieldDefinition GetReference<TReturn>(this MonoMod.MonoModder modder, Expression<Func<TReturn>> reference)
            => modder.Module.GetReference<FieldDefinition>(reference);
        public static MethodDefinition GetReference(this MonoMod.MonoModder modder, Expression<Action> reference)
            => modder.Module.GetReference<MethodDefinition>(reference);
        public static TypeDefinition GetReference<TType>(this MonoMod.MonoModder modder)
        {
            var target = typeof(TType).FullName;
            return modder.Module.Types.Single(t => t.FullName == target);
        }

        public static TReturn GetReference<TReturn>(this IMetadataTokenProvider token, LambdaExpression reference)
        {
            var module = (token as ModuleDefinition) ?? (token as AssemblyDefinition)?.MainModule;
            if (module != null)
            {
                // find the expression method in the meta, matching on the parameter count/types
                var mce = reference.Body as MethodCallExpression;
                if (mce != null)
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

                                return (TReturn)(object)method;
                            }
                        }
                    }
                }
            }

            var me = reference.Body as MemberExpression;
            if (me != null)
            {
                var type = module.Types.Single(t => t.FullName == me.Member.DeclaringType.FullName);
                if (type != null)
                {
                    return (TReturn)(object)type.Fields.Single(f => f.Name == me.Member.Name);
                }
            }

            throw new System.Exception($"Unable to find expression in assembly");
        }

		public static MethodReference GetCoreLibMethod(this ModuleDefinition module, string @namespace, string type, string method)
		{
			var type_ref = new TypeReference(@namespace, type,
				module.TypeSystem.String.Module,
				module.TypeSystem.CoreLibrary
			);
			return new MethodReference(method, module.TypeSystem.Void, type_ref);
		}
    }
}