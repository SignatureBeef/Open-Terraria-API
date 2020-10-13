using System;

namespace FullSerializer.Internal
{
	// Token: 0x0200002E RID: 46
	public struct fsVersionedType
	{
		// Token: 0x06000145 RID: 325 RVA: 0x0000793C File Offset: 0x00005B3C
		public object Migrate(object ancestorInstance)
		{
			return Activator.CreateInstance(this.ModelType, new object[]
			{
				ancestorInstance
			});
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00007954 File Offset: 0x00005B54
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"fsVersionedType [ModelType=",
				this.ModelType,
				", VersionString=",
				this.VersionString,
				", Ancestors.Length=",
				this.Ancestors.Length,
				"]"
			});
		}

		// Token: 0x06000147 RID: 327 RVA: 0x000079AE File Offset: 0x00005BAE
		public static bool operator ==(fsVersionedType a, fsVersionedType b)
		{
			return a.ModelType == b.ModelType;
		}

		// Token: 0x06000148 RID: 328 RVA: 0x000079C1 File Offset: 0x00005BC1
		public static bool operator !=(fsVersionedType a, fsVersionedType b)
		{
			return a.ModelType != b.ModelType;
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000079D4 File Offset: 0x00005BD4
		public override bool Equals(object obj)
		{
			return obj is fsVersionedType && this.ModelType == ((fsVersionedType)obj).ModelType;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000079F6 File Offset: 0x00005BF6
		public override int GetHashCode()
		{
			return this.ModelType.GetHashCode();
		}

		// Token: 0x04000053 RID: 83
		public fsVersionedType[] Ancestors;

		// Token: 0x04000054 RID: 84
		public string VersionString;

		// Token: 0x04000055 RID: 85
		public Type ModelType;
	}
}
