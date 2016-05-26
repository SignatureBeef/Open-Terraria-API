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
        public dynamic Assemblies { get; set; }

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
    }
}
