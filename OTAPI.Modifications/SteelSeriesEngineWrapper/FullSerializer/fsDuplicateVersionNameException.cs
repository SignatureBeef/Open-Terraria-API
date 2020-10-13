using System;

namespace FullSerializer
{
	// Token: 0x0200000D RID: 13
	public sealed class fsDuplicateVersionNameException : Exception
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00002E1D File Offset: 0x0000101D
		public fsDuplicateVersionNameException(Type typeA, Type typeB, string version) : base(string.Concat(new object[]
		{
			typeA,
			" and ",
			typeB,
			" have the same version string (",
			version,
			"); please change one of them."
		}))
		{
		}
	}
}
