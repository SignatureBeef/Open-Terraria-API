using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200007E RID: 126
	public class TactilePatternSimple : TactilePattern_Nonrecursive
	{
		// Token: 0x0600024F RID: 591 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override TactilePatternType PatternType()
		{
			return TactilePatternType.Simple;
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000984D File Offset: 0x00007A4D
		private static TactilePatternSimple _new()
		{
			return new TactilePatternSimple();
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00009854 File Offset: 0x00007A54
		public static TactilePatternSimple Create(TactileEffectSimple[] effects)
		{
			TactilePatternSimple tactilePatternSimple = TactilePatternSimple._new();
			tactilePatternSimple.pattern = effects;
			return tactilePatternSimple;
		}

		// Token: 0x04000229 RID: 553
		public TactileEffectSimple[] pattern;
	}
}
