using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000024 RID: 36
	public class fsNullableConverter : fsConverter
	{
		// Token: 0x060000F9 RID: 249 RVA: 0x00006B6E File Offset: 0x00004D6E
		public override bool CanProcess(Type type)
		{
			return type.Resolve().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00006B94 File Offset: 0x00004D94
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			return this.Serializer.TrySerialize(Nullable.GetUnderlyingType(storageType), instance, out serialized);
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00006BA9 File Offset: 0x00004DA9
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			return this.Serializer.TryDeserialize(data, Nullable.GetUnderlyingType(storageType), ref instance);
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00006BBE File Offset: 0x00004DBE
		public override object CreateInstance(fsData data, Type storageType)
		{
			return storageType;
		}
	}
}
