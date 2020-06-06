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
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Collections.Generic;

namespace OTAPI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RemapHookAttribute : Attribute { }

    public class Remapper
    {
        public ModuleDefinition Module { get; set; }

        public List<object> Tasks { get; set; } = new List<object>();

        public Remapper(ModuleDefinition module)
        {
            this.Module = module;
            Tasks.Add(this);
        }

        public void Remap()
        {
            foreach (var task in Tasks)
                System.Console.WriteLine($"[OTAPI] Remap task: {task.GetType().FullName}");

            var discoverer = new TokenDiscoverer();
            discoverer.Scan(Module, Tasks);
        }
    }

    /// TokenDiscoverer iterates over every token in the assembly while executing hooks
    /// so that other services can utilise it without having to do the same thing twice
    public class TokenDiscoverer
    {
        struct MethodInstance
        {
            public System.Reflection.MethodInfo Method { get; set; }
            public object Instance { get; set; }
        }

        Dictionary<Type, List<MethodInstance>> Hooks { get; set; } = new Dictionary<Type, List<MethodInstance>>();

        public void DiscoverHooks(List<object> modifications)
        {
            foreach (var mod in modifications)
            {
                var modType = mod.GetType();
                foreach (var method in modType.GetMethods())
                {
                    foreach (var attr in method.CustomAttributes)
                    {
                        if (attr.AttributeType == typeof(RemapHookAttribute))
                        {
                            var fieldType = method.GetParameters().First().ParameterType;
                            if (!Hooks.ContainsKey(fieldType))
                            {
                                Hooks.Add(fieldType, new List<MethodInstance>());
                            }

                            Hooks[fieldType].Add(new MethodInstance()
                            {
                                Method = method,
                                Instance = mod,
                            });
                        }
                    }
                }
            }
        }

        public void Scan(ModuleDefinition module, List<object> modifications)
        {
            DiscoverHooks(modifications);

            foreach (var type in module.Types)
                ScanType(type);
        }

        private void ApplyHook<TType>(TType arg, params object[] args)
        {
            var methodArgs = new object[] { arg }.Concat(args).ToArray();
            foreach (var hook in Hooks[typeof(TType)])
                hook.Method.Invoke(hook.Instance, methodArgs);
        }


        [MonoMod.MonoModCustomAttribute("TESTING")]
        public void asd(MethodDefinition method)
        {

        }

        private void ScanType(TypeDefinition type)
        {
            if (Hooks.ContainsKey(typeof(FieldDefinition)))
                foreach (var field in type.Fields.ToArray())
                    ApplyHook(field);

            if (Hooks.ContainsKey(typeof(PropertyDefinition)))
                foreach (var property in type.Properties.ToArray())
                    ApplyHook(property);

            foreach (var nestedType in type.NestedTypes.ToArray())
                ScanType(nestedType);

            // leave instructions last in case fields/properties are remapped
            foreach (var method in type.Methods.ToArray())
            {
                if (method.HasBody && Hooks.ContainsKey(typeof(Instruction)))
                {
                    foreach (var ins in method.Body.Instructions.ToArray())
                        ApplyHook(ins, method);
                }

                if (Hooks.ContainsKey(typeof(MethodDefinition)))
                    ApplyHook(method);
            }
        }
    }
}