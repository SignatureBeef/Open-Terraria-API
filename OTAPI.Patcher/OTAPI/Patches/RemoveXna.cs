using NDesk.Options;
using OTAPI.Patcher.Modification;
using OTAPI.Patcher.Modifications.Helpers;
using System;
using System.Linq;

namespace OTAPI.Patcher.Modifications.Patches
{
    /// <summary>
    /// Replaces all Xna references to use OTAPI.Xna.dll
    /// </summary>
    public class RemoveXna : OTAPIModification<OTAPIContext>
    {
        public override bool IsAvailable(OptionSet options) => this.IsServer();

        public override void Run(OptionSet options)
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

        private void AssemblyResolver_AssemblyResolve(object sender, ModificationAssemblyResolver.AssemblyResolveEventArgs e)
        {
            if (e.Reference.Name == "OTAPI")
                e.AssemblyDefinition = this.Context.OTAPI.Assembly;
        }
    }
}
