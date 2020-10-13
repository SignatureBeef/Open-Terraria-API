using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000020 RID: 32
	public class fsForwardConverter : fsConverter
	{
		// Token: 0x060000DB RID: 219 RVA: 0x00006476 File Offset: 0x00004676
		public fsForwardConverter(fsForwardAttribute attribute)
		{
			this._memberName = attribute.MemberName;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000648A File Offset: 0x0000468A
		public override bool CanProcess(Type type)
		{
			throw new NotSupportedException("Please use the [fsForward(...)] attribute.");
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00006498 File Offset: 0x00004698
		private fsResult GetProperty(object instance, out fsMetaProperty property)
		{
			fsMetaProperty[] properties = fsMetaType.Get(this.Serializer.Config, instance.GetType()).Properties;
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].MemberName == this._memberName)
				{
					property = properties[i];
					return fsResult.Success;
				}
			}
			property = null;
			return fsResult.Fail("No property named \"" + this._memberName + "\" on " + instance.GetType().CSharpName());
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00006518 File Offset: 0x00004718
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = fsData.Null;
			fsResult fsResult = fsResult.Success;
			fsMetaProperty fsMetaProperty;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + this.GetProperty(instance, out fsMetaProperty));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			object instance2 = fsMetaProperty.Read(instance);
			return this.Serializer.TrySerialize(fsMetaProperty.StorageType, instance2, out serialized);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000656C File Offset: 0x0000476C
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			fsMetaProperty fsMetaProperty;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + this.GetProperty(instance, out fsMetaProperty));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			object value = null;
			fsResult = (fsResult2 = fsResult + this.Serializer.TryDeserialize(data, fsMetaProperty.StorageType, ref value));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			fsMetaProperty.Write(instance, value);
			return fsResult;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00005B1D File Offset: 0x00003D1D
		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(this.Serializer.Config, storageType).CreateInstance();
		}

		// Token: 0x04000048 RID: 72
		private string _memberName;
	}
}
