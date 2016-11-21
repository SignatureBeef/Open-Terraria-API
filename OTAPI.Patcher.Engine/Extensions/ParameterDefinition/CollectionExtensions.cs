using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OTAPI.Patcher.Engine.Extensions
{
	public static class CollectionExtensions
	{
		public static string ToParamString(this IEnumerable<ParameterDefinition> collection)
		{
			return "(" + String.Join(",", collection.Select(x => x.ParameterType.Name)) + ")";
		}
	}
}
