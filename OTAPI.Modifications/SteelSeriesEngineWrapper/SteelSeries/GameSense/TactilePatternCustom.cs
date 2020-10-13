using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200007C RID: 124
	public class TactilePatternCustom : TactilePattern_Nonrecursive
	{
		// Token: 0x06000247 RID: 583 RVA: 0x00008AFC File Offset: 0x00006CFC
		public override TactilePatternType PatternType()
		{
			return TactilePatternType.Custom;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000981B File Offset: 0x00007A1B
		private static TactilePatternCustom _new()
		{
			return new TactilePatternCustom();
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00009822 File Offset: 0x00007A22
		public static TactilePatternCustom Create(TactileEffectCustom[] effects)
		{
			TactilePatternCustom tactilePatternCustom = TactilePatternCustom._new();
			tactilePatternCustom.pattern = effects;
			return tactilePatternCustom;
		}

		// Token: 0x04000227 RID: 551
		public TactileEffectCustom[] pattern;
	}
}
