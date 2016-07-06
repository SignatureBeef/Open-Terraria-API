using Mono.Cecil;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Extensions
{
	public static partial class CecilHelpers
	{
		public static TypeDefinition Type(this AssemblyDefinition assemblyDefinition, string name)
		{
			return assemblyDefinition.MainModule.Types.Single(x => x.FullName == name);
		}

		/// <summary>
		/// Returns the TypeDefintion of the type specified
		/// </summary>
		public static TypeDefinition Type<T>(this AssemblyDefinition assemblyDefinition)
		{
			var type = typeof(T);

			if (type.Assembly.FullName == assemblyDefinition.FullName)
				return assemblyDefinition.Type(type.FullName);

			throw new TypeAccessException($"{type.AssemblyQualifiedName} is not a part of the current assembly definition");
		}
	}
}
