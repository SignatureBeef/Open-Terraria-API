using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Linq;

namespace OTAPI.Patcher.Engine.Extensions
{
	public static partial class CecilHelpers
	{

		public static TypeDefinition TypeDefinition(this System.Type type, AssemblyDefinition definition)
		{
			return definition.Type(type.FullName);
		}

		public static MethodDefinition Method(this TypeDefinition typeDefinition, string name,
			bool? isStatic = null,
			Collection<ParameterDefinition> parameters = null
		)
		{
			return typeDefinition.Methods.Single(
				x => x.Name == name
				&& (isStatic == null || x.IsStatic == isStatic.Value)
				&& (parameters == null || x.HasSameParameters(parameters))
			);
		}

		public static FieldDefinition Field(this TypeDefinition typeDefinition, string name)
		{
			return typeDefinition.Fields.Single(x => x.Name == name);
		}

		public static PropertyDefinition Property(this TypeDefinition typeDefinition, string name)
		{
			return typeDefinition.Properties.Single(x => x.Name == name);
		}

		public static bool HasSameParameters(this MethodDefinition method, Collection<ParameterDefinition> parameters)
		{
			var src = method.IsStatic ? method.Parameters.Skip(1) : method.Parameters;
			return src.All(prm => parameters.Any(p => p.Name.Equals(prm.Name, System.StringComparison.CurrentCultureIgnoreCase)));
		}

		public static void ForEachNestedType(this TypeDefinition parent, Action<TypeDefinition> callback)
		{
			foreach (var type in parent.NestedTypes)
				callback(type);
		}

		/// <summary>
		/// Returns the default constructor, expecting only one in the type.
		/// </summary>
		public static MethodDefinition Constructor(this TypeDefinition type) => type.Method(".ctor");

		/// <summary>
		/// Returns the static constructor of the type, if any
		/// </summary>
		public static MethodDefinition StaticConstructor(this TypeDefinition type) => type.Method(".cctor");
	}
}
