using Mono.Cecil;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Patcher.Modification
{
    /// <summary>
    /// ModificationRunner handles anything to do with running injections. 
    /// </summary>
    public class ModificationRunner
    {
        /// <summary>
        /// Injections to be performed.
        /// </summary>
        public List<IModification> Modifications { get; private set; }

        /// <summary>
        /// The global ModificationContext instance for this ModificationRunner.
        /// It is shared across all chidren injections.
        /// </summary>
        public ModificationContext Context { get; private set; }

        /// <summary>
        /// Creates a new InjectionRunner instance, auto populated with the supplied injection collection.
        /// </summary>
        /// <param name="injections"></param>
        public ModificationRunner(ModificationContext context, IEnumerable<IModification> injections)
        {
            //Set the global context
            this.Context = context;

            this.Modifications = injections.ToList();
            //Set new children modification contexts to the global context
            foreach (var mod in this.Modifications)
                mod.ModificationContext = this.Context;
        }

        /// <summary>
        /// Runs all modifications registered in the current instance.
        /// </summary>
        public void Run(OptionSet options)
        {
            foreach (var modification in this.Modifications)
            {
                if (modification.IsAvailable(options))
                    modification.Run(options);
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

        public void SaveAs(string filename, string assemblyName)
        {
            var expando = (IDictionary<string, object>)Context.Assemblies;
            foreach (var item in expando.Keys)
            {
                var asm = expando[item] as AssemblyDefinition;
                asm.Name.Name = assemblyName;
                asm.Write(filename);
            }
        }

        #region Statics
        /// <summary>
        /// Loads all public modifications from a given type into a new ModificationRunner.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        public static ModificationRunner LoadFromAssembly<TSource>(ModificationContext context)
        {
            return new ModificationRunner(context,
                typeof(TSource).Assembly.ExportedTypes
                    .Where(type => typeof(IModification).IsAssignableFrom(type) && !type.IsAbstract)
                    .Select(modificationType => (IModification)Activator.CreateInstance(modificationType))
            );
        }
        #endregion
    }
}
