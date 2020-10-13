using System;

namespace FullSerializer.Internal
{
	// Token: 0x0200002C RID: 44
	public static class fsOption
	{
		// Token: 0x0600012D RID: 301 RVA: 0x000075FF File Offset: 0x000057FF
		public static fsOption<T> Just<T>(T value)
		{
			return new fsOption<T>(value);
		}
	}
}
