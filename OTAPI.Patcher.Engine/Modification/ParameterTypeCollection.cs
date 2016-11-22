using Mono.Cecil;
using Mono.Collections.Generic;
using System.Collections.Generic;

namespace OTAPI.Patcher.Engine.Modification
{
	public class ParameterTypeCollection : List<ParameterType>
	{
		public static explicit operator ParameterTypeCollection(Collection<ParameterDefinition> collection)
		{
			var output = new ParameterTypeCollection();

			foreach (var item in collection)
				output.Add(ParameterType.From(item));

			return output;
		}

		public static explicit operator ParameterTypeCollection(System.Reflection.ParameterInfo[] collection)
		{
			var output = new ParameterTypeCollection();

			foreach (var item in collection)
				output.Add(ParameterType.From(item));

			return output;
		}
	}
}
