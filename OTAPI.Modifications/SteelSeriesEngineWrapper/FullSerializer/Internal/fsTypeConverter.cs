using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000027 RID: 39
	public class fsTypeConverter : fsConverter
	{
		// Token: 0x0600010D RID: 269 RVA: 0x000071AE File Offset: 0x000053AE
		public override bool CanProcess(Type type)
		{
			return typeof(Type).IsAssignableFrom(type);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool RequestCycleSupport(Type type)
		{
			return false;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool RequestInheritanceSupport(Type type)
		{
			return false;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000071C0 File Offset: 0x000053C0
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			Type type = (Type)instance;
			serialized = new fsData(type.FullName);
			return fsResult.Success;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000071E8 File Offset: 0x000053E8
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (!data.IsString)
			{
				return fsResult.Fail("Type converter requires a string");
			}
			instance = fsTypeCache.GetType(data.AsString);
			if (instance == null)
			{
				return fsResult.Fail("Unable to find type " + data.AsString);
			}
			return fsResult.Success;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00006BBE File Offset: 0x00004DBE
		public override object CreateInstance(fsData data, Type storageType)
		{
			return storageType;
		}
	}
}
