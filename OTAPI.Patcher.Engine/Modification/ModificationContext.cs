using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace OTAPI.Patcher.Engine.Modification
{
    /// <summary>
    /// Defines the bare minimum InjectionContext. It's purpose is to dynamically contain loaded assemblies
    /// that are later used by an inheritor with helper methods.
    /// </summary>
    public abstract class ModificationContext
    {
        /// <summary>
        /// Assembilies registered from a dynamic source, such as launch arguments.
        /// </summary>
        public dynamic Assemblies { get; set; }

        public ModificationAssemblyResolver AssemblyResolver { get; } = new ModificationAssemblyResolver();

        private static AssemblyDefinition ReadAssembly(ModificationContext context, string path)
        {
            var rp = new ReaderParameters(ReadingMode.Immediate) { AssemblyResolver = context.AssemblyResolver };
            return AssemblyDefinition.ReadAssembly(path, rp);
        }

        /// <summary>
        /// Loads the given AssemblyDefinitions from disk.
        /// </summary>
        /// <typeparam name="TModificationContext"></typeparam>
        /// <param name="assembliesMap"></param>
        /// <returns></returns>
        public static TModificationContext LoadFromAssemblies<TModificationContext>(Dictionary<string, string> assembliesMap)
            where TModificationContext : ModificationContext, new()
        {
            var ctx = new TModificationContext();
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
            where TInjectionContext : ModificationContext, new()
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
