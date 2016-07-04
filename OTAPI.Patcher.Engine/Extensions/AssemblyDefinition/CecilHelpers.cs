using Mono.Cecil;
using System.Linq;

namespace OTAPI.Patcher.Engine.Extensions
{
	public static partial class CecilHelpers
	{
		public static TypeDefinition Type(this AssemblyDefinition assemblyDefinition, string name)
		{
			return assemblyDefinition.MainModule.Types.Single(x => x.FullName == name);
		}
	}
}
