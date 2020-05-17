using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Patches
{
	/// <summary>
	/// Changes the architecture of the server assembly from x86 to match OTAPI
	/// </summary>
	public class ChangeArchitecture : ModificationBase
	{
		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Changing architecture to AnyCPU (64-bit preferred)";

		public override void Run()
		{
			return;
			SourceDefinition.MainModule.Architecture = Mono.Cecil.TargetArchitecture.I386;
			SourceDefinition.MainModule.Attributes = Mono.Cecil.ModuleAttributes.ILOnly;
		}
	}
}
