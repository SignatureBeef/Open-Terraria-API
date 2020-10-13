using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	// Token: 0x02000022 RID: 34
	public class fsIEnumerableConverter : fsConverter
	{
		// Token: 0x060000E8 RID: 232 RVA: 0x0000665B File Offset: 0x0000485B
		public override bool CanProcess(Type type)
		{
			return typeof(IEnumerable).IsAssignableFrom(type) && fsIEnumerableConverter.GetAddMethod(type) != null;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00005B1D File Offset: 0x00003D1D
		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(this.Serializer.Config, storageType).CreateInstance();
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00006680 File Offset: 0x00004880
		public override fsResult TrySerialize(object instance_, out fsData serialized, Type storageType)
		{
			IEnumerable enumerable = (IEnumerable)instance_;
			fsResult success = fsResult.Success;
			Type elementType = fsIEnumerableConverter.GetElementType(storageType);
			serialized = fsData.CreateList(fsIEnumerableConverter.HintSize(enumerable));
			List<fsData> asList = serialized.AsList;
			foreach (object instance in enumerable)
			{
				fsData item;
				fsResult result = this.Serializer.TrySerialize(elementType, instance, out item);
				success.AddMessages(result);
				if (!result.Failed)
				{
					asList.Add(item);
				}
			}
			if (this.IsStack(enumerable.GetType()))
			{
				asList.Reverse();
			}
			return success;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000673C File Offset: 0x0000493C
		private bool IsStack(Type type)
		{
			return type.Resolve().IsGenericType && type.Resolve().GetGenericTypeDefinition() == typeof(Stack<>);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00006768 File Offset: 0x00004968
		public override fsResult TryDeserialize(fsData data, ref object instance_, Type storageType)
		{
			IEnumerable enumerable = (IEnumerable)instance_;
			fsResult fsResult = fsResult.Success;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + base.CheckType(data, fsDataType.Array));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			Type elementType = fsIEnumerableConverter.GetElementType(storageType);
			MethodInfo addMethod = fsIEnumerableConverter.GetAddMethod(storageType);
			MethodInfo flattenedMethod = storageType.GetFlattenedMethod("get_Item");
			MethodInfo flattenedMethod2 = storageType.GetFlattenedMethod("set_Item");
			if (flattenedMethod2 == null)
			{
				fsIEnumerableConverter.TryClear(storageType, enumerable);
			}
			int num = fsIEnumerableConverter.TryGetExistingSize(storageType, enumerable);
			List<fsData> asList = data.AsList;
			for (int i = 0; i < asList.Count; i++)
			{
				fsData data2 = asList[i];
				object obj = null;
				if (flattenedMethod != null && i < num)
				{
					obj = flattenedMethod.Invoke(enumerable, new object[]
					{
						i
					});
				}
				fsResult result = this.Serializer.TryDeserialize(data2, elementType, ref obj);
				fsResult.AddMessages(result);
				if (!result.Failed)
				{
					if (flattenedMethod2 != null && i < num)
					{
						flattenedMethod2.Invoke(enumerable, new object[]
						{
							i,
							obj
						});
					}
					else
					{
						addMethod.Invoke(enumerable, new object[]
						{
							obj
						});
					}
				}
			}
			return fsResult;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000068A6 File Offset: 0x00004AA6
		private static int HintSize(IEnumerable collection)
		{
			if (collection is ICollection)
			{
				return ((ICollection)collection).Count;
			}
			return 0;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x000068C0 File Offset: 0x00004AC0
		private static Type GetElementType(Type objectType)
		{
			if (objectType.HasElementType)
			{
				return objectType.GetElementType();
			}
			Type @interface = fsReflectionUtility.GetInterface(objectType, typeof(IEnumerable<>));
			if (@interface != null)
			{
				return @interface.GetGenericArguments()[0];
			}
			return typeof(object);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000690C File Offset: 0x00004B0C
		private static void TryClear(Type type, object instance)
		{
			MethodInfo flattenedMethod = type.GetFlattenedMethod("Clear");
			if (flattenedMethod != null)
			{
				flattenedMethod.Invoke(instance, null);
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006938 File Offset: 0x00004B38
		private static int TryGetExistingSize(Type type, object instance)
		{
			PropertyInfo flattenedProperty = type.GetFlattenedProperty("Count");
			if (flattenedProperty != null)
			{
				return (int)flattenedProperty.GetGetMethod().Invoke(instance, null);
			}
			return 0;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00006970 File Offset: 0x00004B70
		private static MethodInfo GetAddMethod(Type type)
		{
			Type @interface = fsReflectionUtility.GetInterface(type, typeof(ICollection<>));
			if (@interface != null)
			{
				MethodInfo declaredMethod = @interface.GetDeclaredMethod("Add");
				if (declaredMethod != null)
				{
					return declaredMethod;
				}
			}
			MethodInfo result;
			if ((result = type.GetFlattenedMethod("Add")) == null)
			{
				result = (type.GetFlattenedMethod("Push") ?? type.GetFlattenedMethod("Enqueue"));
			}
			return result;
		}
	}
}
