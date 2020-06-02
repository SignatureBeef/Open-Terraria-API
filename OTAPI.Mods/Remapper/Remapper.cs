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

        private void ScanType(TypeDefinition type)
        {
            foreach (var method in type.Methods)
            {
                if (method.HasBody && Hooks.ContainsKey(typeof(Instruction)))
                {
                    foreach (var ins in method.Body.Instructions)
                        ApplyHook(ins, method);
                }

                if (Hooks.ContainsKey(typeof(MethodDefinition)))
                    ApplyHook(method);
            }

            if (Hooks.ContainsKey(typeof(FieldDefinition)))
                foreach (var field in type.Fields)
                    ApplyHook(field);

            if (Hooks.ContainsKey(typeof(PropertyDefinition)))
                foreach (var property in type.Properties)
                    ApplyHook(property);

            foreach (var nestedType in type.NestedTypes)
                ScanType(nestedType);
        }
    }
}