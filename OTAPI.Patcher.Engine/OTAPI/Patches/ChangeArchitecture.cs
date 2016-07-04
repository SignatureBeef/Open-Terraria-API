using NDesk.Options;
using OTAPI.Patcher.Engine.Modification;
using OTAPI.Patcher.Engine.Modifications.Helpers;

namespace OTAPI.Patcher.Engine.Modifications.Patches
{
	/// <summary>
	/// Changes the architecture of the server from x86 to match the OTAPI 
	/// </summary>
	public class ChanegArchitecture : ModificationBase
	{
		/// <summary>
		/// Determines if current context is running in server mode.
		/// This patch is only applicable to server assemblies.
		/// </summary>
		/// <returns>True if the server assemblies are detected, otherwise False.</returns>
		public override bool IsAvailable() => this.IsServer();

		public override string Description => "Changing architecture to x86";

        public override void Run()
		{
			SourceDefinition.MainModule.Architecture = Mono.Cecil.TargetArchitecture.I386;
			SourceDefinition.MainModule.Attributes = Mono.Cecil.ModuleAttributes.ILOnly;
		}
	}
}
