using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000029 RID: 41
	public class fsSerializationCallbackProcessor : fsObjectProcessor
	{
		// Token: 0x0600011B RID: 283 RVA: 0x000073A1 File Offset: 0x000055A1
		public override bool CanProcess(Type type)
		{
			return typeof(fsISerializationCallbacks).IsAssignableFrom(type);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000073B3 File Offset: 0x000055B3
		public override void OnBeforeSerialize(Type storageType, object instance)
		{
			if (instance == null)
			{
				return;
			}
			((fsISerializationCallbacks)instance).OnBeforeSerialize(storageType);
		}

		// Token: 0x0600011D RID: 285 RVA: 0x000073C5 File Offset: 0x000055C5
		public override void OnAfterSerialize(Type storageType, object instance, ref fsData data)
		{
			if (instance == null)
			{
				return;
			}
			((fsISerializationCallbacks)instance).OnAfterSerialize(storageType, ref data);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000073D8 File Offset: 0x000055D8
		public override void OnBeforeDeserializeAfterInstanceCreation(Type storageType, object instance, ref fsData data)
		{
			if (!(instance is fsISerializationCallbacks))
			{
				throw new InvalidCastException(string.Concat(new object[]
				{
					"Please ensure the converter for ",
					storageType,
					" actually returns an instance of it, not an instance of ",
					instance.GetType()
				}));
			}
			((fsISerializationCallbacks)instance).OnBeforeDeserialize(storageType, ref data);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00007428 File Offset: 0x00005628
		public override void OnAfterDeserialize(Type storageType, object instance)
		{
			if (instance == null)
			{
				return;
			}
			((fsISerializationCallbacks)instance).OnAfterDeserialize(storageType);
		}
	}
}
