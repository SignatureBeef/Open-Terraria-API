using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000021 RID: 33
	public class fsGuidConverter : fsConverter
	{
		// Token: 0x060000E1 RID: 225 RVA: 0x000065D1 File Offset: 0x000047D1
		public override bool CanProcess(Type type)
		{
			return type == typeof(Guid);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000065E4 File Offset: 0x000047E4
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			serialized = new fsData(((Guid)instance).ToString());
			return fsResult.Success;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006611 File Offset: 0x00004811
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (data.IsString)
			{
				instance = new Guid(data.AsString);
				return fsResult.Success;
			}
			return fsResult.Fail("fsGuidConverter encountered an unknown JSON data type");
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006640 File Offset: 0x00004840
		public override object CreateInstance(fsData data, Type storageType)
		{
			return default(Guid);
		}
	}
}
