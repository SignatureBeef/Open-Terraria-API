using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000076 RID: 118
	[fsObject(Converter = typeof(RateRangeConverter))]
	public class RateRange : AbstractRate
	{
		// Token: 0x0600021E RID: 542 RVA: 0x00009407 File Offset: 0x00007607
		private static RateRange _new()
		{
			return new RateRange();
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000940E File Offset: 0x0000760E
		private static RateRange Create(uint size)
		{
			RateRange rateRange = RateRange._new();
			rateRange.frequency = new Frequency[size];
			rateRange.repeatLimit = new RepeatLimit[size];
			return rateRange;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000942D File Offset: 0x0000762D
		public static RateRange Create(Frequency[] frequency, RepeatLimit[] repeatLimit = null)
		{
			RateRange rateRange = RateRange._new();
			rateRange.frequency = frequency;
			rateRange.repeatLimit = repeatLimit;
			return rateRange;
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00009444 File Offset: 0x00007644
		public static RateRange Create(FreqRepeatLimitPair[] pairs)
		{
			RateRange rateRange = RateRange.Create((uint)pairs.Length);
			uint num = 0U;
			foreach (FreqRepeatLimitPair freqRepeatLimitPair in pairs)
			{
				rateRange.frequency[(int)num].low = freqRepeatLimitPair.low;
				rateRange.frequency[(int)num].high = freqRepeatLimitPair.high;
				rateRange.frequency[(int)num].frequency = freqRepeatLimitPair.frequency;
				rateRange.repeatLimit[(int)num].low = freqRepeatLimitPair.low;
				rateRange.repeatLimit[(int)num].high = freqRepeatLimitPair.high;
				rateRange.repeatLimit[(int)num].repeatLimit = freqRepeatLimitPair.repeatLimit;
				num += 1U;
			}
			return rateRange;
		}

		// Token: 0x0400021D RID: 541
		public Frequency[] frequency;

		// Token: 0x0400021E RID: 542
		public RepeatLimit[] repeatLimit;
	}
}
