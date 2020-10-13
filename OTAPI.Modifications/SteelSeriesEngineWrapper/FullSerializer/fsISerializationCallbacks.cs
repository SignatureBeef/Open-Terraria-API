using System;

namespace FullSerializer
{
	// Token: 0x02000011 RID: 17
	public interface fsISerializationCallbacks
	{
		// Token: 0x0600004D RID: 77
		void OnBeforeSerialize(Type storageType);

		// Token: 0x0600004E RID: 78
		void OnAfterSerialize(Type storageType, ref fsData data);

		// Token: 0x0600004F RID: 79
		void OnBeforeDeserialize(Type storageType, ref fsData data);

		// Token: 0x06000050 RID: 80
		void OnAfterDeserialize(Type storageType);
	}
}
