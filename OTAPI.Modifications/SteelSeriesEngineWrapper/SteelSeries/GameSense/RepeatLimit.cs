using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x0200005E RID: 94
	[fsObject(Converter = typeof(RepeatLimitConverter))]
	[Serializable]
	public struct RepeatLimit
	{
		// Token: 0x060001D1 RID: 465 RVA: 0x00008D06 File Offset: 0x00006F06
		public RepeatLimit(uint low, uint high, uint repeatLimit)
		{
			this.low = low;
			this.high = high;
			this.repeatLimit = repeatLimit;
		}

		// Token: 0x0400017E RID: 382
		public uint low;

		// Token: 0x0400017F RID: 383
		public uint high;

		// Token: 0x04000180 RID: 384
		public uint repeatLimit;
	}
}
