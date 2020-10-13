using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200005A RID: 90
	[Serializable]
	public struct Frequency
	{
		// Token: 0x060001D0 RID: 464 RVA: 0x00008CEF File Offset: 0x00006EEF
		public Frequency(uint low, uint high, uint frequency)
		{
			this.low = low;
			this.high = high;
			this.frequency = frequency;
		}

		// Token: 0x04000170 RID: 368
		public uint low;

		// Token: 0x04000171 RID: 369
		public uint high;

		// Token: 0x04000172 RID: 370
		public uint frequency;
	}
}
