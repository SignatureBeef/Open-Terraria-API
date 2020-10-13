using System;
using System.Collections;

namespace FullSerializer.Internal
{
	// Token: 0x02000026 RID: 38
	public class fsReflectedConverter : fsConverter
	{
		// Token: 0x06000108 RID: 264 RVA: 0x00007003 File Offset: 0x00005203
		public override bool CanProcess(Type type)
		{
			return !type.Resolve().IsArray && !typeof(ICollection).IsAssignableFrom(type);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00007028 File Offset: 0x00005228
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = fsData.CreateDictionary();
			fsResult success = fsResult.Success;
			fsMetaType fsMetaType = fsMetaType.Get(this.Serializer.Config, instance.GetType());
			fsMetaType.EmitAotData();
			for (int i = 0; i < fsMetaType.Properties.Length; i++)
			{
				fsMetaProperty fsMetaProperty = fsMetaType.Properties[i];
				if (fsMetaProperty.CanRead)
				{
					fsData value;
					fsResult result = this.Serializer.TrySerialize(fsMetaProperty.StorageType, fsMetaProperty.OverrideConverterType, fsMetaProperty.Read(instance), out value);
					success.AddMessages(result);
					if (!result.Failed)
					{
						serialized.AsDictionary[fsMetaProperty.JsonName] = value;
					}
				}
			}
			return success;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x000070D0 File Offset: 0x000052D0
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + base.CheckType(data, fsDataType.Object));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			fsMetaType fsMetaType = fsMetaType.Get(this.Serializer.Config, storageType);
			fsMetaType.EmitAotData();
			for (int i = 0; i < fsMetaType.Properties.Length; i++)
			{
				fsMetaProperty fsMetaProperty = fsMetaType.Properties[i];
				fsData data2;
				if (fsMetaProperty.CanWrite && data.AsDictionary.TryGetValue(fsMetaProperty.JsonName, out data2))
				{
					object value = null;
					if (fsMetaProperty.CanRead)
					{
						value = fsMetaProperty.Read(instance);
					}
					fsResult result = this.Serializer.TryDeserialize(data2, fsMetaProperty.StorageType, fsMetaProperty.OverrideConverterType, ref value);
					fsResult.AddMessages(result);
					if (!result.Failed)
					{
						fsMetaProperty.Write(instance, value);
					}
				}
			}
			return fsResult;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00005B1D File Offset: 0x00003D1D
		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(this.Serializer.Config, storageType).CreateInstance();
		}
	}
}
