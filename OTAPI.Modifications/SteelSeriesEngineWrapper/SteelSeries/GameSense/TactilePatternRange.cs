using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200007D RID: 125
	public class TactilePatternRange : AbstractTactilePattern
	{
		// Token: 0x0600024B RID: 587 RVA: 0x000092B3 File Offset: 0x000074B3
		public override TactilePatternType PatternType()
		{
			return TactilePatternType.Range;
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00009838 File Offset: 0x00007A38
		private static TactilePatternRange _new()
		{
			return new TactilePatternRange();
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000983F File Offset: 0x00007A3F
		public static TactilePatternRange Create(TactileEffectRange[] effects)
		{
			TactilePatternRange tactilePatternRange = TactilePatternRange._new();
			tactilePatternRange.pattern = effects;
			return tactilePatternRange;
		}

		// Token: 0x04000228 RID: 552
		public TactileEffectRange[] pattern;
	}
}
