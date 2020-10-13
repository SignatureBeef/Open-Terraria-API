using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200003B RID: 59
	[Serializable]
	public struct FreqRepeatLimitPair
	{
		// Token: 0x0600017E RID: 382 RVA: 0x000082E6 File Offset: 0x000064E6
		public FreqRepeatLimitPair(uint low, uint high, uint frequency, uint repeatLimit)
		{
			this.low = low;
			this.high = high;
			this.frequency = frequency;
			this.repeatLimit = repeatLimit;
		}

		// Token: 0x0400006A RID: 106
		public uint low;

		// Token: 0x0400006B RID: 107
		public uint high;

		// Token: 0x0400006C RID: 108
		public uint frequency;

		// Token: 0x0400006D RID: 109
		public uint repeatLimit;
	}
}
