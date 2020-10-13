using System;
using FullSerializer;
using SteelSeries.GameSense.DeviceZone;

namespace SteelSeries.GameSense
{
	// Token: 0x0200007A RID: 122
	[fsObject(Converter = typeof(TactileHandlerConverter))]
	[Serializable]
	public class TactileHandler : AbstractHandler
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600022C RID: 556 RVA: 0x000095AA File Offset: 0x000077AA
		public TactilePatternType TactilePatternType
		{
			get
			{
				return this._tactilePatternType;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600022D RID: 557 RVA: 0x000095B2 File Offset: 0x000077B2
		public RateMode RateMode
		{
			get
			{
				return this._rateMode;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600022E RID: 558 RVA: 0x000095BA File Offset: 0x000077BA
		// (set) Token: 0x0600022F RID: 559 RVA: 0x000095C7 File Offset: 0x000077C7
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

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000230 RID: 560 RVA: 0x000095D7 File Offset: 0x000077D7
		// (set) Token: 0x06000231 RID: 561 RVA: 0x000095E4 File Offset: 0x000077E4
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000232 RID: 562 RVA: 0x000095F4 File Offset: 0x000077F4
		// (set) Token: 0x06000233 RID: 563 RVA: 0x00009601 File Offset: 0x00007801
		public TactilePatternRange pattern_range
		{
			get
			{
				return this.pattern as TactilePatternRange;
			}
			set
			{
				this.pattern = value;
				this._tactilePatternType = TactilePatternType.Range;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000234 RID: 564 RVA: 0x00009611 File Offset: 0x00007811
		// (set) Token: 0x06000235 RID: 565 RVA: 0x0000961E File Offset: 0x0000781E
		public RateStatic rate_static
		{
			get
			{
				return this.rate as RateStatic;
			}
			set
			{
				this.rate = value;
				this._rateMode = RateMode.Static;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000236 RID: 566 RVA: 0x0000962E File Offset: 0x0000782E
		// (set) Token: 0x06000237 RID: 567 RVA: 0x0000963B File Offset: 0x0000783B
		public RateRange rate_range
		{
			get
			{
				return this.rate as RateRange;
			}
			set
			{
				this.rate = value;
				this._rateMode = RateMode.Range;
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000964B File Offset: 0x0000784B
		private static TactileHandler _new()
		{
			return new TactileHandler();
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00009652 File Offset: 0x00007852
		private static TactileHandler Create(AbstractTactileDevice_Zone dz, TactileMode mode)
		{
			TactileHandler tactileHandler = TactileHandler._new();
			tactileHandler.deviceZone = dz;
			tactileHandler.mode = mode;
			return tactileHandler;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00009667 File Offset: 0x00007867
		public static TactileHandler Create(AbstractTactileDevice_Zone dz, TactileMode mode, TactileEffectSimple[] pattern)
		{
			TactileHandler tactileHandler = TactileHandler.Create(dz, mode);
			tactileHandler.pattern_simple = TactilePatternSimple.Create(pattern);
			return tactileHandler;
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000967C File Offset: 0x0000787C
		public static TactileHandler Create(AbstractTactileDevice_Zone dz, TactileMode mode, TactileEffectCustom[] pattern)
		{
			TactileHandler tactileHandler = TactileHandler.Create(dz, mode);
			tactileHandler.pattern_custom = TactilePatternCustom.Create(pattern);
			return tactileHandler;
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00009691 File Offset: 0x00007891
		public static TactileHandler Create(AbstractTactileDevice_Zone dz, TactileMode mode, TactileEffectRange[] pattern)
		{
			TactileHandler tactileHandler = TactileHandler.Create(dz, mode);
			tactileHandler.pattern_range = TactilePatternRange.Create(pattern);
			return tactileHandler;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x000096A6 File Offset: 0x000078A6
		public static TactileHandler Create(AbstractTactileDevice_Zone dz, TactileMode mode, TactileEffectSimple[] pattern, RateStatic rate)
		{
			TactileHandler tactileHandler = TactileHandler.Create(dz, mode, pattern);
			tactileHandler.rate_static = rate;
			return tactileHandler;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x000096B7 File Offset: 0x000078B7
		public static TactileHandler Create(AbstractTactileDevice_Zone dz, TactileMode mode, TactileEffectCustom[] pattern, RateStatic rate)
		{
			TactileHandler tactileHandler = TactileHandler.Create(dz, mode, pattern);
			tactileHandler.rate_static = rate;
			return tactileHandler;
		}

		// Token: 0x0600023F RID: 575 RVA: 0x000096C8 File Offset: 0x000078C8
		public static TactileHandler Create(AbstractTactileDevice_Zone dz, TactileMode mode, TactileEffectRange[] pattern, RateStatic rate)
		{
			TactileHandler tactileHandler = TactileHandler.Create(dz, mode, pattern);
			tactileHandler.rate_static = rate;
			return tactileHandler;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x000096D9 File Offset: 0x000078D9
		public static TactileHandler Create(AbstractTactileDevice_Zone dz, TactileMode mode, TactileEffectSimple[] pattern, RateRange rate)
		{
			TactileHandler tactileHandler = TactileHandler.Create(dz, mode, pattern);
			tactileHandler.rate_range = rate;
			return tactileHandler;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x000096EA File Offset: 0x000078EA
		public static TactileHandler Create(AbstractTactileDevice_Zone dz, TactileMode mode, TactileEffectCustom[] pattern, RateRange rate)
		{
			TactileHandler tactileHandler = TactileHandler.Create(dz, mode, pattern);
			tactileHandler.rate_range = rate;
			return tactileHandler;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x000096FB File Offset: 0x000078FB
		public static TactileHandler Create(AbstractTactileDevice_Zone dz, TactileMode mode, TactileEffectRange[] pattern, RateRange rate)
		{
			TactileHandler tactileHandler = TactileHandler.Create(dz, mode, pattern);
			tactileHandler.rate_range = rate;
			return tactileHandler;
		}

		// Token: 0x04000221 RID: 545
		private TactilePatternType _tactilePatternType;

		// Token: 0x04000222 RID: 546
		private RateMode _rateMode;

		// Token: 0x04000223 RID: 547
		public AbstractTactileDevice_Zone deviceZone;

		// Token: 0x04000224 RID: 548
		public TactileMode mode;

		// Token: 0x04000225 RID: 549
		public AbstractTactilePattern pattern;

		// Token: 0x04000226 RID: 550
		public AbstractRate rate;
	}
}
