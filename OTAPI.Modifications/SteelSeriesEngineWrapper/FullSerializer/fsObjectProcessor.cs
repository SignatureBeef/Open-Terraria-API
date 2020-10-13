using System;

namespace FullSerializer
{
	// Token: 0x02000016 RID: 22
	public abstract class fsObjectProcessor
	{
		// Token: 0x06000074 RID: 116 RVA: 0x000040C1 File Offset: 0x000022C1
		public virtual bool CanProcess(Type type)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000040C8 File Offset: 0x000022C8
		public virtual void OnBeforeSerialize(Type storageType, object instance)
		{
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000040C8 File Offset: 0x000022C8
		public virtual void OnAfterSerialize(Type storageType, object instance, ref fsData data)
		{
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000040C8 File Offset: 0x000022C8
		public virtual void OnBeforeDeserialize(Type storageType, ref fsData data)
		{
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000040C8 File Offset: 0x000022C8
		public virtual void OnBeforeDeserializeAfterInstanceCreation(Type storageType, object instance, ref fsData data)
		{
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000040C8 File Offset: 0x000022C8
		public virtual void OnAfterDeserialize(Type storageType, object instance)
		{
		}
	}
}
