using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace OTAPI.Patcher.Inject
{
    /// <summary>
    /// Defines the bare minimum InjectionContext. It's purpose is to dynamically contain loaded assemblies
    /// that are later used by an inheritor with helper methods.
    /// </summary>
    public abstract class InjectionContext
    {
        /// <summary>
        /// Assembilies registered from a dynamic source, such as launch arguments.
        /// </summary>
        public dynamic Assemblies { get; set; }

        public ContextAssemblyResolver AssemblyResolver { get; } = new ContextAssemblyResolver();

        private static AssemblyDefinition ReadAssembly(InjectionContext context, string path)
        {
            var rp = new ReaderParameters(ReadingMode.Immediate) { AssemblyResolver = context.AssemblyResolver };
            return AssemblyDefinition.ReadAssembly(path, rp);
        }

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
                var definition = ReadAssembly(ctx, assembliesMap[asm]);
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
                var definition = ReadAssembly(ctx, asm);
                assemblies.Add(definition.Name.Name, definition);
            }

            ctx.Assemblies = assemblies;

            return ctx;
        }
    }
}
