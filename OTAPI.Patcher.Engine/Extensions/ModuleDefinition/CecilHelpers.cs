using Mono.Cecil;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Extensions
{
	public static partial class CecilHelpers
	{
		/// <summary>
		/// Returns a type from the current module by its fullName
		/// </summary>
		public static TypeDefinition Type(this ModuleDefinition moduleDefinition, string fullName)
		{
			return moduleDefinition.Types.Single(x => x.FullName == fullName);
		}

		/// <summary>
		/// Enumerates all methods in the current module
		/// </summary>
		public static void ForEachMethod(this ModuleDefinition module, Action<MethodDefinition> callback)
		{
			module.ForEachType(type =>
			{
				foreach (var mth in type.Methods)
				{
					callback.Invoke(mth);
				}
			});
		}

		/// <summary>
		/// Enumerates all instructions in all methods across each type of the assembly
		/// </summary>
		public static void ForEachInstruction(this ModuleDefinition module, Action<MethodDefinition, Mono.Cecil.Cil.Instruction> callback)
		{
			module.ForEachMethod(method =>
			{
				if (method.HasBody)
				{
					foreach (var ins in method.Body.Instructions.ToArray())
						callback.Invoke(method, ins);
				}
			});
		}

		/// <summary>
		/// Enumerates over each type in the assembly, including nested types
		/// </summary>
		public static void ForEachType(this ModuleDefinition module, Action<TypeDefinition> callback)
		{
			foreach (var type in module.Types)
			{
				callback(type);

				//Enumerate nested types
				type.ForEachNestedType(callback);
			}
		}
	}
}
