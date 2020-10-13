using System;
using System.Collections.Generic;
using System.Reflection;
using FullSerializer.Internal;

namespace FullSerializer
{
	// Token: 0x02000008 RID: 8
	public class fsConverterRegistrar
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002874 File Offset: 0x00000A74
		static fsConverterRegistrar()
		{
			foreach (FieldInfo fieldInfo in typeof(fsConverterRegistrar).GetDeclaredFields())
			{
				if (fieldInfo.Name.StartsWith("Register_"))
				{
					fsConverterRegistrar.Converters.Add(fieldInfo.FieldType);
				}
			}
			foreach (MethodInfo methodInfo in typeof(fsConverterRegistrar).GetDeclaredMethods())
			{
				if (methodInfo.Name.StartsWith("Register_"))
				{
					methodInfo.Invoke(null, null);
				}
			}
		}

		// Token: 0x0400000F RID: 15
		public static List<Type> Converters = new List<Type>();
	}
}
