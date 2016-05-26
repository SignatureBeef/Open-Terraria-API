using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace OTAPI.Patcher.Inject
{
    /// <summary>
    /// Defines the bare minimum InjectionContext.
    /// </summary>
    public class InjectionContext
    {
        /// <summary>
        /// Assembilies registered from a dynamic source, such as launch arguments.
        /// </summary>
        public dynamic Assemblies { get; set; }

        /// <summary>
        /// Loads the given AssemblyDefinitions from disk.
        /// </summary>
        /// <typeparam name="TInjectionContext"></typeparam>
        /// <param name="assembliesMap"></param>
        /// <returns></returns>
        public static TInjectionContext LoadFromAssemblies<TInjectionContext>(Dictionary<string, string> assembliesMap)
            where TInjectionContext : InjectionContext, new()
        {
            var ctx = new TInjectionContext();
            var assemblies = (IDictionary<String, Object>)new ExpandoObject();

            foreach (var asm in assembliesMap.Keys)
            {
                var definition = AssemblyDefinition.ReadAssembly(assembliesMap[asm]);
                assemblies.Add(asm, definition);
            }

            ctx.Assemblies = assemblies;

            return ctx;
        }

        /// <summary>
        /// Loads the given AssemblyDefinitions from disk.
        /// </summary>
        /// <typeparam name="TInjectionContext"></typeparam>
        /// <param name="assembliesMap"></param>
        /// <returns></returns>
        public static TInjectionContext LoadFromAssemblies<TInjectionContext>(params string[] assembliesMap)
            where TInjectionContext : InjectionContext, new()
        {
            var ctx = new TInjectionContext();
            var assemblies = (IDictionary<String, Object>)new ExpandoObject();

            foreach (var asm in assembliesMap)
            {
                var definition = AssemblyDefinition.ReadAssembly(asm);
                assemblies.Add(definition.Name.Name, definition);
            }

            ctx.Assemblies = assemblies;

            return ctx;
        }
    }
}
