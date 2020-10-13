using System;

namespace FullSerializer
{
	// Token: 0x02000015 RID: 21
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class fsObjectAttribute : Attribute
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00004095 File Offset: 0x00002295
		public fsObjectAttribute()
		{
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000040A4 File Offset: 0x000022A4
		public fsObjectAttribute(string versionString, params Type[] previousModels)
		{
			this.VersionString = versionString;
			this.PreviousModels = previousModels;
		}

		// Token: 0x04000025 RID: 37
		public Type[] PreviousModels;

		// Token: 0x04000026 RID: 38
		public string VersionString;

		// Token: 0x04000027 RID: 39
		public fsMemberSerialization MemberSerialization = fsMemberSerialization.Default;

		// Token: 0x04000028 RID: 40
		public Type Converter;

		// Token: 0x04000029 RID: 41
		public Type Processor;
	}
}
