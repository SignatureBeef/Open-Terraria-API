using Mono.Cecil;
using Mono.Cecil.Cil;
using OTAPI.Patcher.Engine.Extensions;
using OTAPI.Patcher.Engine.Modification;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Modifications.Mono.Modifications
{
	/// <summary>
	/// Since we use the Windows TerrariaServer.exe we need to handle windows specific code
	/// In this case we need to replace all calls to 'FileOperationAPIWrapper.MoveToRecycleBin'
	/// and redirect them to our own callback that will handle mono+.net
	///
	/// UPDATE: This PInvoke is apparently quite broken on .net core, so I have decided to remove it for all platforms.
	/// - Kevin
	/// </summary>
	public class HandlePlatformIO : ModificationBase
	{
		public override IEnumerable<string> AssemblyTargets => new[]
		{
			"TerrariaServer, Version=1.4.1.2, Culture=neutral, PublicKeyToken=null"
		};
		public override string Description => "Handling platform dependent code";

		public override void Run()
		{
			var find = this.Method(() => Terraria.Utilities.FileOperationAPIWrapper.MoveToRecycleBin(null));
			var callback = find.Module.Import(
				this.Method(() => OTAPI.Callbacks.Terraria.IO.DeleteFile(null))
			);

			this.SourceDefinition.MainModule.ForEachInstruction((method, ins) =>
			{
				var call_method = ins.Operand as MethodReference;
				if (call_method != null && call_method.FullName == find.FullName)
				{
					ins.Operand = callback;
				}
			});
		}
	}
}
