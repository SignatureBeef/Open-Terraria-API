using System;

namespace FullSerializer
{
	// Token: 0x02000017 RID: 23
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class fsPropertyAttribute : Attribute
	{
		// Token: 0x0600007B RID: 123 RVA: 0x000040CA File Offset: 0x000022CA
		public fsPropertyAttribute() : this(string.Empty)
		{
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000040D7 File Offset: 0x000022D7
		public fsPropertyAttribute(string name)
		{
			this.Name = name;
		}

		// Token: 0x0400002A RID: 42
		public string Name;

		// Token: 0x0400002B RID: 43
		public Type Converter;
	}
}
