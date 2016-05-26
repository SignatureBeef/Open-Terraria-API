using Mono.Cecil;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Patcher.Inject
{
    /// <summary>
    /// InjectionRunner handles anything to do with running injections. 
    /// </summary>
    public class InjectionRunner
    {
        /// <summary>
        /// Injections to be performed.
        /// </summary>
        public List<IInjection> Injections { get; private set; }

        /// <summary>
        /// The global InjectionContext instance for this InjectionRunner.
        /// It is shared across all chidren injections.
        /// </summary>
        public InjectionContext Context { get; private set; }

        /// <summary>
        /// Creates a new InjectionRunner instance, auto populated with the supplied injection collection.
        /// </summary>
        /// <param name="injections"></param>
        public InjectionRunner(InjectionContext context, IEnumerable<IInjection> injections)
        {
            //Set the global context
            this.Context = context;

            this.Injections = injections.ToList();
            //Set new children injection contexts to the global context
            foreach (var injection in this.Injections)
                injection.InjectionContext = this.Context;
        }

        /// <summary>
        /// Runs all injections registered in the current instance.
        /// </summary>
        public void Run(OptionSet options)
        {
            foreach (var injection in this.Injections)
            {
                injection.Inject(options);
            }
        }

        public void SaveAll(string filename)
        {
            var expando = (IDictionary<string, object>)Context.Assemblies;
            foreach (var item in expando.Keys)
            {
                var asm = expando[item] as AssemblyDefinition;
                asm.Write(asm.MainModule.FullyQualifiedName);
            }
        }

        #region Statics
        /// <summary>
        /// Loads all public Injections from a given type into a new InjectionRunner.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static InjectionRunner LoadFromAssembly<TSource>(InjectionContext context)
        {
            return new InjectionRunner(context,
                typeof(TSource).Assembly.ExportedTypes
                    .Where(type => typeof(IInjection).IsAssignableFrom(type) && !type.IsAbstract)
                    .Select(injectionType => (IInjection)Activator.CreateInstance(injectionType))
            );
        }
        #endregion
    }
}
