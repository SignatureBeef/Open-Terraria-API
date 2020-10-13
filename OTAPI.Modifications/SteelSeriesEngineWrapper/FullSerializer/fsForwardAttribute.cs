using System;

namespace FullSerializer
{
	// Token: 0x02000002 RID: 2
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public sealed class fsForwardAttribute : Attribute
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public fsForwardAttribute(string memberName)
		{
			this.MemberName = memberName;
		}

		// Token: 0x04000001 RID: 1
		public string MemberName;
	}
}
