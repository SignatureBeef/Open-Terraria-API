using NDesk.Options;
using OTAPI.Patcher.Inject;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications
{
    /// <summary>
    /// Replaces all Xna references to use OTAPI.Xna.dll
    /// </summary>
    public class RemoveXna : Injection<OTAPIContext>
    {
        public override bool CanInject(OptionSet options)
        {
            return this.Context.OTAPI.MainModue.Types.Any(t => t.Namespace.StartsWith("Microsoft.Xna.Framework"));
        }

        public override void Inject(OptionSet options)
        {
            Console.Write("Removing Xna references...");

            //Context.OTAPI.MainModue.Resources.Clear();

            var xnaFramework = Context.OTAPI.MainModue.AssemblyReferences
                .Where(x => x.Name.StartsWith("Microsoft.Xna.Framework"))
                .ToArray();

            for (var x = 0; x < xnaFramework.Length; x++)
            {
                xnaFramework[x].Name = "OTAPI"; //TODO: Fix me, ILRepack is adding .dll to the asm name      Context.OTAPI.Assembly.Name.Name;
                xnaFramework[x].PublicKey = Context.OTAPI.Assembly.Name.PublicKey;
                xnaFramework[x].PublicKeyToken = Context.OTAPI.Assembly.Name.PublicKeyToken;
                xnaFramework[x].Version = Context.OTAPI.Assembly.Name.Version;
            }

            //Since we
            Context.AssemblyResolver.AssemblyResolve += AssemblyResolver_AssemblyResolve;

            Console.WriteLine("Done.");
        }

        private void AssemblyResolver_AssemblyResolve(object sender, ContextAssemblyResolver.AssemblyResolveEventArgs e)
        {
            if (e.Reference.Name == "OTAPI")
                e.AssemblyDefinition = this.Context.OTAPI.Assembly;
        }
    }
}
