using System;

namespace SteelSeries.GameSense
{
	// Token: 0x02000070 RID: 112
	public class ColorRanges : AbstractColor
	{
		// Token: 0x0600020E RID: 526 RVA: 0x000092B3 File Offset: 0x000074B3
		public override ColorEffect ColorEffectType()
		{
			return ColorEffect.Range;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x000092B6 File Offset: 0x000074B6
		private static ColorRanges _new()
		{
			return new ColorRanges();
		}

		// Token: 0x06000210 RID: 528 RVA: 0x000092BD File Offset: 0x000074BD
		public static ColorRanges Create(ColorRange[] ranges)
		{
			ColorRanges colorRanges = ColorRanges._new();
			colorRanges.ranges = ranges;
			return colorRanges;
		}

		// Token: 0x04000216 RID: 534
		public ColorRange[] ranges;
	}
}
