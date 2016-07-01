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
		public override string Description => "Removing Xna references...";
        public override bool IsAvailable(OptionSet options) => this.IsServer();

        public override void Run(OptionSet options)
        {
            //Context.OTAPI.MainModue.Resources.Clear();

            var xnaFramework = Context.Terraria.MainModue.AssemblyReferences
                .Where(x => x.Name.StartsWith("Microsoft.Xna.Framework"))
                .ToArray();

            for (var x = 0; x < xnaFramework.Length; x++)
            {
                xnaFramework[x].Name = "OTAPI.Xna"; //TODO: Fix me, ILRepack is adding .dll to the asm name      Context.OTAPI.Assembly.Name.Name;
                xnaFramework[x].PublicKey = Context.Assemblies.Xna.Name.PublicKey;
                xnaFramework[x].PublicKeyToken = Context.Assemblies.Xna.Name.PublicKeyToken;
                xnaFramework[x].Version = Context.Assemblies.Xna.Name.Version;
            }

            //To resolve the "OTAPI" from above until the .dll can be corrected.
            Context.AssemblyResolver.AssemblyResolve += AssemblyResolver_AssemblyResolve;
        }

        private void AssemblyResolver_AssemblyResolve(object sender, ModificationAssemblyResolver.AssemblyResolveEventArgs e)
        {
            if (e.Reference.Name == "OTAPI.Xna")
                e.AssemblyDefinition = this.Context.Assemblies.Xna;
        }
    }
}
