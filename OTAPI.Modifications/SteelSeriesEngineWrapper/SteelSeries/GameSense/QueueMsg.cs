using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200004B RID: 75
	public abstract class QueueMsg
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000198 RID: 408
		// (set) Token: 0x06000199 RID: 409
		public abstract object data { get; set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600019A RID: 410
		public abstract Uri uri { get; }

		// Token: 0x0600019B RID: 411
		public abstract bool IsCritical();

		// Token: 0x0400015C RID: 348
		protected object _data;
	}
}
