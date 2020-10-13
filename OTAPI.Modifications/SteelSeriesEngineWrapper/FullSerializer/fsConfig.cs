using System;
using System.Reflection;

namespace FullSerializer
{
	// Token: 0x02000005 RID: 5
	public class fsConfig
	{
		// Token: 0x04000005 RID: 5
		public Type[] SerializeAttributes = new Type[]
		{
			typeof(fsPropertyAttribute)
		};

		// Token: 0x04000006 RID: 6
		public Type[] IgnoreSerializeAttributes = new Type[]
		{
			typeof(NonSerializedAttribute),
			typeof(fsIgnoreAttribute)
		};

		// Token: 0x04000007 RID: 7
		public fsMemberSerialization DefaultMemberSerialization = fsMemberSerialization.Default;

		// Token: 0x04000008 RID: 8
		public Func<string, MemberInfo, string> GetJsonNameFromMemberName = (string name, MemberInfo info) => name;

		// Token: 0x04000009 RID: 9
		public bool SerializeNonAutoProperties;

		// Token: 0x0400000A RID: 10
		public bool SerializeNonPublicSetProperties = true;

		// Token: 0x0400000B RID: 11
		public string CustomDateTimeFormatString;

		// Token: 0x0400000C RID: 12
		public bool Serialize64BitIntegerAsString;

		// Token: 0x0400000D RID: 13
		public bool SerializeEnumsAsInteger;
	}
}
