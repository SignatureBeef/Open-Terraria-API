using Mono.Cecil;
using OTAPI.Patcher.Engine.Modification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Patcher.Engine.Extensions
{
	public static class ParameterTypeExtensions
	{
		public static string ToParamString(this IEnumerable<ParameterType> collection)
		{
			return "(" + String.Join(",", collection.Select(x => x.TypeName)) + ")";
		}

		public static IEnumerable<ParameterType> ToParameters(this IEnumerable<ParameterDefinition> collection)
		{
			foreach (var item in collection)
				yield return ParameterType.From(item);
		}

		public static IEnumerable<ParameterType> ToParameters(this IEnumerable<System.Reflection.ParameterInfo> collection)
		{
			foreach (var item in collection)
				yield return ParameterType.From(item);
		}
	}
}
