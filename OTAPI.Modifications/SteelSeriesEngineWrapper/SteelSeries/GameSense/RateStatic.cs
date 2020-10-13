using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000078 RID: 120
	[fsObject(Converter = typeof(RateStaticConverter))]
	public class RateStatic : AbstractRate
	{
		// Token: 0x06000226 RID: 550 RVA: 0x00009557 File Offset: 0x00007757
		private static RateStatic _new()
		{
			return new RateStatic();
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000955E File Offset: 0x0000775E
		public static RateStatic Create(uint frequency, uint repeatLimit = 0U)
		{
			RateStatic rateStatic = RateStatic._new();
			rateStatic.frequency = frequency;
			rateStatic.repeatLimit = repeatLimit;
			return rateStatic;
		}

		// Token: 0x0400021F RID: 543
		public uint frequency;

		// Token: 0x04000220 RID: 544
		public uint repeatLimit;
	}
}
