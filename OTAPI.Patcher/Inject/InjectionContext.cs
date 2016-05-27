using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace OTAPI.Patcher.Inject
{
    public class ContextAssemblyResolver : DefaultAssemblyResolver
    {
        public class AssemblyResolveEventArgs : EventArgs
        {
            public AssemblyNameReference Reference { get; set; }
            public AssemblyDefinition AssemblyDefinition { get; set; }
        }
        public event EventHandler<AssemblyResolveEventArgs> AssemblyResolve;

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            var args = new AssemblyResolveEventArgs()
            {
                Reference = name,
                AssemblyDefinition = null
            };
            if (AssemblyResolve != null) AssemblyResolve.Invoke(this, args);

            return args.AssemblyDefinition ?? base.Resolve(name);
        }
    }

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
