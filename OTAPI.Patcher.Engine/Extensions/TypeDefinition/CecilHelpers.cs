using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
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
			Collection<ParameterDefinition> parameters = null,
			int skipMethodParameters = 0,
			int skipInputParameters = 0,
			bool acceptParamObjectTypes = false
		)
		{

			IEnumerable<ParameterDefinition> parametersClone = null;
			if (parameters != null)
			{
				if (skipInputParameters > 0)
				{
					parametersClone = parameters.ToArray().Skip(skipInputParameters);
				}
				else
				{
					parametersClone = parameters.ToArray();
				}
			}

			var matches = typeDefinition.Methods.Where(
				 x => x.Name == name
				 && (isStatic == null || x.IsStatic == isStatic.Value)
			);

			if (parameters != null)
			{
				matches = matches.Where(x =>
					(skipMethodParameters > 0 ? x.Parameters.Skip(skipMethodParameters) : x.Parameters)
						.CompareParameterTypes(parametersClone, acceptParamObjectTypes)
				);
			}

			if (matches.Count() == 0)
				throw new Exception($"Method `{name}` is not found in {typeDefinition.FullName}. Expected {parametersClone.ToParamString()}.");
			else if (matches.Count() > 1)
				throw new Exception($"Too many methods named `{name}` found in {typeDefinition.FullName}");

			return matches.Single();
		}

		public static FieldDefinition Field(this TypeDefinition typeDefinition, string name)
		{
			return typeDefinition.Fields.Single(x => x.Name == name);
		}

		public static PropertyDefinition Property(this TypeDefinition typeDefinition, string name)
		{
			return typeDefinition.Properties.Single(x => x.Name == name);
		}

		public static TypeDefinition NestedType(this TypeDefinition typeDefinition, string name)
		{
			return typeDefinition.NestedTypes.Single(x => x.Name == name);
		}

		public static bool CompareParameterTypes(this IEnumerable<ParameterDefinition> source,
			IEnumerable<ParameterDefinition> parameters,
			bool acceptParamObjectTypes = false)
		{
			if (source.Count() == parameters.Count())
			{
				for (var x = 0; x < source.Count(); x++)
				{
					var src = source.ElementAt(x).ParameterType;
					var ext = parameters.ElementAt(x).ParameterType;

					var referenceType = src as ByReferenceType;
					if (referenceType != null)
					{
						src = referenceType.ElementType;
					}

					if (src.Name != ext.Name)
					{
						if (!(acceptParamObjectTypes && src.Name == "Object"))
						{
							return false;
						}
					}
				}

				return true;
			}
			return false;
		}

		public static void ForEachNestedType(this TypeDefinition parent, Action<TypeDefinition> callback)
		{
			foreach (var type in parent.NestedTypes)
			{
				callback(type);

				type.ForEachNestedType(callback);
			}
		}

		/// <summary>
		/// Returns the default constructor, expecting only one in the type.
		/// </summary>
		public static MethodDefinition Constructor(this TypeDefinition type) => type.Method(".ctor");

		/// <summary>
		/// Returns the static constructor of the type, if any
		/// </summary>
		public static MethodDefinition StaticConstructor(this TypeDefinition type) => type.Method(".cctor");

		public static bool SignatureMatches(this TypeDefinition type, TypeDefinition compareTo)
		{
			var typeInstanceMethods = type.Methods.Where(m => !m.IsStatic && !m.IsGetter && !m.IsSetter);
			var compareToInstanceMethods = compareTo.Methods.Where(m => !m.IsStatic && !m.IsGetter && !m.IsSetter && (type.IsInterface && !m.IsConstructor));

			var missing = compareToInstanceMethods.Where(m => !typeInstanceMethods.Any(m2 => m2.Name == m.Name));

			if (typeInstanceMethods.Count() != compareToInstanceMethods.Count())
				return false;

			for (var x = 0; x < typeInstanceMethods.Count(); x++)
			{
				var typeMethod = typeInstanceMethods.ElementAt(x);
				var compareToMethod = compareToInstanceMethods.ElementAt(x);

				if (!typeMethod.SignatureMatches(compareToMethod))
					return false;
			}

			return true;
		}

		public static bool SignatureMatches(this MethodDefinition method, MethodDefinition compareTo, bool ignoreDeclaringType = true)
		{
			if (method.Name != compareTo.Name)
				return false;
			if (method.ReturnType.FullName != compareTo.ReturnType.FullName)
				return false;
			if (method.Parameters.Count != compareTo.Parameters.Count)
				return false;
			if (method.Overrides.Count != compareTo.Overrides.Count)
				return false;
			if (method.GenericParameters.Count != compareTo.GenericParameters.Count)
				return false;
			if (!method.DeclaringType.IsInterface && method.Attributes != compareTo.Attributes)
				return false;

			for (var x = 0; x < method.Parameters.Count; x++)
			{
				if (method.Parameters[x].ParameterType.FullName != compareTo.Parameters[x].ParameterType.FullName
					&& (ignoreDeclaringType && method.Parameters[x].ParameterType != method.DeclaringType)
					)
					return false;

				if (method.Parameters[x].Name != compareTo.Parameters[x].Name)
					return false;
			}

			for (var x = 0; x < method.Overrides.Count; x++)
			{
				if (method.Overrides[x].Name != compareTo.Overrides[x].Name)
					return false;
			}

			for (var x = 0; x < method.GenericParameters.Count; x++)
			{
				if (method.GenericParameters[x].Name != compareTo.GenericParameters[x].Name)
					return false;
			}

			return true;
		}
	}
}
