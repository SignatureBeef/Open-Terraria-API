using System;

namespace FullSerializer
{
	// Token: 0x0200000E RID: 14
	public sealed class fsMissingVersionConstructorException : Exception
	{
		// Token: 0x0600004A RID: 74 RVA: 0x00002E54 File Offset: 0x00001054
		public fsMissingVersionConstructorException(Type versionedType, Type constructorType) : base(versionedType + " is missing a constructor for previous model type " + constructorType)
		{
		}
	}
}
