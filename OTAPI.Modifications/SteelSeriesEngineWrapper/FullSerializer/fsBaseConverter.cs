using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer.Internal;

namespace FullSerializer
{
	// Token: 0x02000004 RID: 4
	public abstract class fsBaseConverter
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000024B4 File Offset: 0x000006B4
		public virtual object CreateInstance(fsData data, Type storageType)
		{
			if (this.RequestCycleSupport(storageType))
			{
				throw new InvalidOperationException(string.Concat(new object[]
				{
					"Please override CreateInstance for ",
					base.GetType().FullName,
					"; the object graph for ",
					storageType,
					" can contain potentially contain cycles, so separated instance creation is needed"
				}));
			}
			return storageType;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002506 File Offset: 0x00000706
		public virtual bool RequestCycleSupport(Type storageType)
		{
			return !(storageType == typeof(string)) && (storageType.Resolve().IsClass || storageType.Resolve().IsInterface);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002536 File Offset: 0x00000736
		public virtual bool RequestInheritanceSupport(Type storageType)
		{
			return !storageType.Resolve().IsSealed;
		}

		// Token: 0x0600000C RID: 12
		public abstract fsResult TrySerialize(object instance, out fsData serialized, Type storageType);

		// Token: 0x0600000D RID: 13
		public abstract fsResult TryDeserialize(fsData data, ref object instance, Type storageType);

		// Token: 0x0600000E RID: 14 RVA: 0x00002548 File Offset: 0x00000748
		protected fsResult FailExpectedType(fsData data, params fsDataType[] types)
		{
			object[] array = new object[7];
			array[0] = base.GetType().Name;
			array[1] = " expected one of ";
			array[2] = string.Join(", ", (from t in types
			select t.ToString()).ToArray<string>());
			array[3] = " but got ";
			array[4] = data.Type;
			array[5] = " in ";
			array[6] = data;
			return fsResult.Fail(string.Concat(array));
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000025D4 File Offset: 0x000007D4
		protected fsResult CheckType(fsData data, fsDataType type)
		{
			if (data.Type != type)
			{
				return fsResult.Fail(string.Concat(new object[]
				{
					base.GetType().Name,
					" expected ",
					type,
					" but got ",
					data.Type,
					" in ",
					data
				}));
			}
			return fsResult.Success;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002641 File Offset: 0x00000841
		protected fsResult CheckKey(fsData data, string key, out fsData subitem)
		{
			return this.CheckKey(data.AsDictionary, key, out subitem);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002654 File Offset: 0x00000854
		protected fsResult CheckKey(Dictionary<string, fsData> data, string key, out fsData subitem)
		{
			if (!data.TryGetValue(key, out subitem))
			{
				return fsResult.Fail(string.Concat(new object[]
				{
					base.GetType().Name,
					" requires a <",
					key,
					"> key in the data ",
					data
				}));
			}
			return fsResult.Success;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000026A8 File Offset: 0x000008A8
		protected fsResult SerializeMember<T>(Dictionary<string, fsData> data, Type overrideConverterType, string name, T value)
		{
			fsData value2;
			fsResult result = this.Serializer.TrySerialize(typeof(T), overrideConverterType, value, out value2);
			if (result.Succeeded)
			{
				data[name] = value2;
			}
			return result;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000026E8 File Offset: 0x000008E8
		protected fsResult DeserializeMember<T>(Dictionary<string, fsData> data, Type overrideConverterType, string name, out T value)
		{
			fsData data2;
			if (!data.TryGetValue(name, out data2))
			{
				value = default(T);
				return fsResult.Fail("Unable to find member \"" + name + "\"");
			}
			object obj = null;
			fsResult result = this.Serializer.TryDeserialize(data2, typeof(T), overrideConverterType, ref obj);
			value = (T)((object)obj);
			return result;
		}

		// Token: 0x04000004 RID: 4
		public fsSerializer Serializer;
	}
}
