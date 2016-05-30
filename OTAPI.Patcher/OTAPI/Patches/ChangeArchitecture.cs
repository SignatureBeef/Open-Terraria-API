using NDesk.Options;
using OTAPI.Patcher.Modification;
using OTAPI.Patcher.Modifications.Helpers;

namespace OTAPI.Patcher.Modifications.Patches
{
    /// <summary>
    /// Changes the architecture of the server from x86 to match the OTAPI 
    /// </summary>
    public class ChanegArchitecture : OTAPIModification<OTAPIContext>
    {
        public override bool IsAvailable(OptionSet options) => this.IsServer();

        public override void Run(OptionSet options)
        {
            this.Context.Terraria.MainModue.Architecture = Mono.Cecil.TargetArchitecture.I386;
            this.Context.Terraria.MainModue.Attributes = Mono.Cecil.ModuleAttributes.ILOnly;
        }
    }
}
