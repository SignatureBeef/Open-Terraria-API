using Mono.Cecil;

namespace OTAPI.Patcher.Engine.Modification
{
	public struct ParameterType
	{
		public string Name { get; set; }
		public string TypeName { get; set; }

		public object Type { get; set; }

		public static ParameterType From(TypeReference type)
		{
			return new ParameterType()
			{
				Name = type.Name,
				TypeName = type.FullName,
				Type = type
			};
		}

		public static ParameterType From(ParameterDefinition parameter)
		{
			return new ParameterType()
			{
				Name = parameter.ParameterType.Name,
				TypeName = parameter.ParameterType.FullName,
				Type = parameter.ParameterType
			};
		}

		public static ParameterType From(System.Reflection.ParameterInfo parameter)
		{
			return new ParameterType()
			{
				Name = parameter.ParameterType.Name,
				TypeName = parameter.ParameterType.FullName,
				Type = parameter.ParameterType
			};
		}
	}
}
