using NDesk.Options;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Patches
{
	/// <summary>
	/// Changes the architecture of the server from x86 to match the OTAPI 
	/// </summary>
	public class ChanegArchitecture : ModificationBase
	{
		public override string Description => "Changing architecture to x86";

        public override void Run()
		{
			SourceDefinition.MainModule.Architecture = Mono.Cecil.TargetArchitecture.I386;
			SourceDefinition.MainModule.Attributes = Mono.Cecil.ModuleAttributes.ILOnly;
		}
	}
}
