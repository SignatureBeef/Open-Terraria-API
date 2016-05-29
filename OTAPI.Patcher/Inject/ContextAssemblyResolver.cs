using Mono.Cecil;
using System;

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
}
