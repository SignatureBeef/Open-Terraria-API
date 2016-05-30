using Mono.Cecil;
using System;

namespace OTAPI.Patcher.Modification
{
    /// <summary>
    /// Provides a way for modifications to easily resolve modifications 
    /// while the assembly is in the process of being saved to disk.
    /// </summary>
    public class ModificationAssemblyResolver : DefaultAssemblyResolver
    {
        public class AssemblyResolveEventArgs : EventArgs
        {
            public AssemblyNameReference Reference { get; set; }
            public AssemblyDefinition AssemblyDefinition { get; set; }
        }
        public event EventHandler<AssemblyResolveEventArgs> AssemblyResolve;

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            //Fire the event
            var args = new AssemblyResolveEventArgs()
            {
                Reference = name,
                AssemblyDefinition = null
            };
            if (AssemblyResolve != null) AssemblyResolve.Invoke(this, args);

            //Use the default behaviour as a fallback
            return args.AssemblyDefinition ?? base.Resolve(name);
        }
    }
}
