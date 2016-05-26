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
        public IEnumerable<Injection> Injections { get; private set; }

        /// <summary>
        /// The global InjectionContext instance for this InjectionRunner.
        /// It is shared across all chidren injections.
        /// </summary>
        public InjectionContext Context { get; private set; }

        /// <summary>
        /// Creates a new InjectionRunner instance, auto populated with the supplied injection collection.
        /// </summary>
        /// <param name="injections"></param>
        public InjectionRunner(InjectionContext context, IEnumerable<Injection> injections)
        {
            //Set the global context
            this.Context = context;

            //Set new children injection contexts to the global context
            foreach (var injection in injections)
                injection.Context = this.Context;
            this.Injections = injections;
        }

        /// <summary>
        /// Loads all public Injections from a given type into a new InjectionRunner.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static InjectionRunner LoadFromAssembly<TSource>(InjectionContext context)
        {
            return new InjectionRunner(context,
                typeof(TSource).Assembly.ExportedTypes
                    .Where(type => typeof(Injection).IsAssignableFrom(type) && !type.IsAbstract)
                    .Select(injectionType => (Injection)Activator.CreateInstance(injectionType))
            );
        }
    }
}
