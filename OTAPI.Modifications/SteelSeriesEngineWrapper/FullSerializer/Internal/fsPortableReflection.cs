using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FullSerializer.Internal
{
	// Token: 0x0200002D RID: 45
	public static class fsPortableReflection
	{
		// Token: 0x0600012E RID: 302 RVA: 0x00007607 File Offset: 0x00005807
		public static bool HasAttribute(MemberInfo element, Type attributeType)
		{
			return fsPortableReflection.GetAttribute(element, attributeType, true) != null;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00007614 File Offset: 0x00005814
		public static bool HasAttribute<TAttribute>(MemberInfo element)
		{
			return fsPortableReflection.HasAttribute(element, typeof(TAttribute));
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00007628 File Offset: 0x00005828
		public static Attribute GetAttribute(MemberInfo element, Type attributeType, bool shouldCache)
		{
			fsPortableReflection.AttributeQuery key = new fsPortableReflection.AttributeQuery
			{
				MemberInfo = element,
				AttributeType = attributeType
			};
			Attribute attribute;
			if (!fsPortableReflection._cachedAttributeQueries.TryGetValue(key, out attribute))
			{
				attribute = (Attribute)element.GetCustomAttributes(attributeType, true).FirstOrDefault<object>();
				if (shouldCache)
				{
					fsPortableReflection._cachedAttributeQueries[key] = attribute;
				}
			}
			return attribute;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00007681 File Offset: 0x00005881
		public static TAttribute GetAttribute<TAttribute>(MemberInfo element, bool shouldCache) where TAttribute : Attribute
		{
			return (TAttribute)((object)fsPortableReflection.GetAttribute(element, typeof(TAttribute), shouldCache));
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00007699 File Offset: 0x00005899
		public static TAttribute GetAttribute<TAttribute>(MemberInfo element) where TAttribute : Attribute
		{
			return fsPortableReflection.GetAttribute<TAttribute>(element, true);
		}

		// Token: 0x06000133 RID: 307 RVA: 0x000076A4 File Offset: 0x000058A4
		public static PropertyInfo GetDeclaredProperty(this Type type, string propertyName)
		{
			PropertyInfo[] declaredProperties = type.GetDeclaredProperties();
			for (int i = 0; i < declaredProperties.Length; i++)
			{
				if (declaredProperties[i].Name == propertyName)
				{
					return declaredProperties[i];
				}
			}
			return null;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000076DC File Offset: 0x000058DC
		public static MethodInfo GetDeclaredMethod(this Type type, string methodName)
		{
			MethodInfo[] declaredMethods = type.GetDeclaredMethods();
			for (int i = 0; i < declaredMethods.Length; i++)
			{
				if (declaredMethods[i].Name == methodName)
				{
					return declaredMethods[i];
				}
			}
			return null;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00007714 File Offset: 0x00005914
		public static ConstructorInfo GetDeclaredConstructor(this Type type, Type[] parameters)
		{
			foreach (ConstructorInfo constructorInfo in type.GetDeclaredConstructors())
			{
				ParameterInfo[] parameters2 = constructorInfo.GetParameters();
				if (parameters.Length == parameters2.Length)
				{
					return constructorInfo;
				}
			}
			return null;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00007771 File Offset: 0x00005971
		public static ConstructorInfo[] GetDeclaredConstructors(this Type type)
		{
			return type.GetConstructors(fsPortableReflection.DeclaredFlags);
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00007780 File Offset: 0x00005980
		public static MemberInfo[] GetFlattenedMember(this Type type, string memberName)
		{
			List<MemberInfo> list = new List<MemberInfo>();
			while (type != null)
			{
				MemberInfo[] declaredMembers = type.GetDeclaredMembers();
				for (int i = 0; i < declaredMembers.Length; i++)
				{
					if (declaredMembers[i].Name == memberName)
					{
						list.Add(declaredMembers[i]);
					}
				}
				type = type.Resolve().BaseType;
			}
			return list.ToArray();
		}

		// Token: 0x06000138 RID: 312 RVA: 0x000077E0 File Offset: 0x000059E0
		public static MethodInfo GetFlattenedMethod(this Type type, string methodName)
		{
			while (type != null)
			{
				MethodInfo[] declaredMethods = type.GetDeclaredMethods();
				for (int i = 0; i < declaredMethods.Length; i++)
				{
					if (declaredMethods[i].Name == methodName)
					{
						return declaredMethods[i];
					}
				}
				type = type.Resolve().BaseType;
			}
			return null;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000782F File Offset: 0x00005A2F
		public static IEnumerable<MethodInfo> GetFlattenedMethods(this Type type, string methodName)
		{
			while (type != null)
			{
				MethodInfo[] methods = type.GetDeclaredMethods();
				int num;
				for (int i = 0; i < methods.Length; i = num)
				{
					if (methods[i].Name == methodName)
					{
						yield return methods[i];
					}
					num = i + 1;
				}
				type = type.Resolve().BaseType;
				methods = null;
			}
			yield break;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00007848 File Offset: 0x00005A48
		public static PropertyInfo GetFlattenedProperty(this Type type, string propertyName)
		{
			while (type != null)
			{
				PropertyInfo[] declaredProperties = type.GetDeclaredProperties();
				for (int i = 0; i < declaredProperties.Length; i++)
				{
					if (declaredProperties[i].Name == propertyName)
					{
						return declaredProperties[i];
					}
				}
				type = type.Resolve().BaseType;
			}
			return null;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00007898 File Offset: 0x00005A98
		public static MemberInfo GetDeclaredMember(this Type type, string memberName)
		{
			MemberInfo[] declaredMembers = type.GetDeclaredMembers();
			for (int i = 0; i < declaredMembers.Length; i++)
			{
				if (declaredMembers[i].Name == memberName)
				{
					return declaredMembers[i];
				}
			}
			return null;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x000078CF File Offset: 0x00005ACF
		public static MethodInfo[] GetDeclaredMethods(this Type type)
		{
			return type.GetMethods(fsPortableReflection.DeclaredFlags);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x000078DC File Offset: 0x00005ADC
		public static PropertyInfo[] GetDeclaredProperties(this Type type)
		{
			return type.GetProperties(fsPortableReflection.DeclaredFlags);
		}

		// Token: 0x0600013E RID: 318 RVA: 0x000078E9 File Offset: 0x00005AE9
		public static FieldInfo[] GetDeclaredFields(this Type type)
		{
			return type.GetFields(fsPortableReflection.DeclaredFlags);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x000078F6 File Offset: 0x00005AF6
		public static MemberInfo[] GetDeclaredMembers(this Type type)
		{
			return type.GetMembers(fsPortableReflection.DeclaredFlags);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00007903 File Offset: 0x00005B03
		public static MemberInfo AsMemberInfo(Type type)
		{
			return type;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00007906 File Offset: 0x00005B06
		public static bool IsType(MemberInfo member)
		{
			return member is Type;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00007911 File Offset: 0x00005B11
		public static Type AsType(MemberInfo member)
		{
			return (Type)member;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00007903 File Offset: 0x00005B03
		public static Type Resolve(this Type type)
		{
			return type;
		}

		// Token: 0x04000050 RID: 80
		public static Type[] EmptyTypes = new Type[0];

		// Token: 0x04000051 RID: 81
		private static IDictionary<fsPortableReflection.AttributeQuery, Attribute> _cachedAttributeQueries = new Dictionary<fsPortableReflection.AttributeQuery, Attribute>(new fsPortableReflection.AttributeQueryComparator());

		// Token: 0x04000052 RID: 82
		private static BindingFlags DeclaredFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		// Token: 0x020000BA RID: 186
		private struct AttributeQuery
		{
			// Token: 0x04000253 RID: 595
			public MemberInfo MemberInfo;

			// Token: 0x04000254 RID: 596
			public Type AttributeType;
		}

		// Token: 0x020000BB RID: 187
		private class AttributeQueryComparator : IEqualityComparer<fsPortableReflection.AttributeQuery>
		{
			// Token: 0x060002A4 RID: 676 RVA: 0x00009CA3 File Offset: 0x00007EA3
			public bool Equals(fsPortableReflection.AttributeQuery x, fsPortableReflection.AttributeQuery y)
			{
				return x.MemberInfo == y.MemberInfo && x.AttributeType == y.AttributeType;
			}

			// Token: 0x060002A5 RID: 677 RVA: 0x00009CCB File Offset: 0x00007ECB
			public int GetHashCode(fsPortableReflection.AttributeQuery obj)
			{
				return obj.MemberInfo.GetHashCode() + 17 * obj.AttributeType.GetHashCode();
			}
		}
	}
}
