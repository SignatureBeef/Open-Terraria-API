using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000063 RID: 99
	[fsObject(Converter = typeof(TactileEffectRangeConverter))]
	[Serializable]
	public class TactileEffectRange
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001DA RID: 474 RVA: 0x00008DF9 File Offset: 0x00006FF9
		public TactilePatternType TactilePatternType
		{
			get
			{
				return this._tactilePatternType;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001DB RID: 475 RVA: 0x00008E01 File Offset: 0x00007001
		// (set) Token: 0x060001DC RID: 476 RVA: 0x00008E0E File Offset: 0x0000700E
		public TactilePatternSimple pattern_simple
		{
			get
			{
				return this.pattern as TactilePatternSimple;
			}
			set
			{
				this.pattern = value;
				this._tactilePatternType = TactilePatternType.Simple;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001DD RID: 477 RVA: 0x00008E1E File Offset: 0x0000701E
		// (set) Token: 0x060001DE RID: 478 RVA: 0x00008E2B File Offset: 0x0000702B
		public TactilePatternCustom pattern_custom
		{
			get
			{
				return this.pattern as TactilePatternCustom;
			}
			set
			{
				this.pattern = value;
				this._tactilePatternType = TactilePatternType.Custom;
			}
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00008E3B File Offset: 0x0000703B
		private TactileEffectRange(uint low, uint high)
		{
			this.low = low;
			this.high = high;
			this._tactilePatternType = TactilePatternType.Simple;
			this.pattern = null;
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00008E5F File Offset: 0x0000705F
		public TactileEffectRange(uint low, uint high, TactileEffectSimple[] pattern) : this(low, high)
		{
			this.pattern_simple = TactilePatternSimple.Create(pattern);
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00008E75 File Offset: 0x00007075
		public TactileEffectRange(uint low, uint high, TactileEffectCustom[] pattern) : this(low, high)
		{
			this.pattern_custom = TactilePatternCustom.Create(pattern);
		}

		// Token: 0x04000187 RID: 391
		private TactilePatternType _tactilePatternType;

		// Token: 0x04000188 RID: 392
		public uint low;

		// Token: 0x04000189 RID: 393
		public uint high;

		// Token: 0x0400018A RID: 394
		public TactilePattern_Nonrecursive pattern;
	}
}
