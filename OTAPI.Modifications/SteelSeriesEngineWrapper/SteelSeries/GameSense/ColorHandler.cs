using System;
using FullSerializer;
using SteelSeries.GameSense.DeviceZone;

namespace SteelSeries.GameSense
{
	// Token: 0x0200006E RID: 110
	[fsObject(Converter = typeof(ColorHandlerConverter))]
	public class ColorHandler : AbstractHandler
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x00008FE4 File Offset: 0x000071E4
		public ColorEffect ColorEffect
		{
			get
			{
				return this._colorEffect;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x00008FEC File Offset: 0x000071EC
		public RateMode RateMode
		{
			get
			{
				return this._rateMode;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x00008FF4 File Offset: 0x000071F4
		// (set) Token: 0x060001F6 RID: 502 RVA: 0x00009001 File Offset: 0x00007201
		public ColorStatic color_static
		{
			get
			{
				return this.color as ColorStatic;
			}
			set
			{
				this.color = value;
				this._colorEffect = ColorEffect.Static;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x00009011 File Offset: 0x00007211
		// (set) Token: 0x060001F8 RID: 504 RVA: 0x0000901E File Offset: 0x0000721E
		public ColorGradient color_gradient
		{
			get
			{
				return this.color as ColorGradient;
			}
			set
			{
				this.color = value;
				this._colorEffect = ColorEffect.Gradient;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x0000902E File Offset: 0x0000722E
		// (set) Token: 0x060001FA RID: 506 RVA: 0x0000903B File Offset: 0x0000723B
		public ColorRanges color_range
		{
			get
			{
				return this.color as ColorRanges;
			}
			set
			{
				this.color = value;
				this._colorEffect = ColorEffect.Range;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060001FB RID: 507 RVA: 0x0000904B File Offset: 0x0000724B
		// (set) Token: 0x060001FC RID: 508 RVA: 0x00009058 File Offset: 0x00007258
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

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001FD RID: 509 RVA: 0x00009068 File Offset: 0x00007268
		// (set) Token: 0x060001FE RID: 510 RVA: 0x00009075 File Offset: 0x00007275
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

		// Token: 0x060001FF RID: 511 RVA: 0x00009085 File Offset: 0x00007285
		private static ColorHandler _new()
		{
			return new ColorHandler();
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000908C File Offset: 0x0000728C
		private static ColorHandler Create(AbstractIlluminationDevice_Zone dz, IlluminationMode mode)
		{
			ColorHandler colorHandler = ColorHandler._new();
			colorHandler.deviceZone = dz;
			colorHandler.mode = mode;
			colorHandler._rateMode = RateMode.None;
			return colorHandler;
		}

		// Token: 0x06000201 RID: 513 RVA: 0x000090A8 File Offset: 0x000072A8
		public static ColorHandler Create(AbstractIlluminationDevice_Zone dz, IlluminationMode mode, ColorStatic color)
		{
			ColorHandler colorHandler = ColorHandler.Create(dz, mode);
			colorHandler.color_static = color;
			return colorHandler;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x000090B8 File Offset: 0x000072B8
		public static ColorHandler Create(AbstractIlluminationDevice_Zone dz, IlluminationMode mode, ColorStatic color, RateStatic rate)
		{
			ColorHandler colorHandler = ColorHandler.Create(dz, mode);
			colorHandler.color_static = color;
			colorHandler.rate_static = rate;
			return colorHandler;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x000090CF File Offset: 0x000072CF
		public static ColorHandler Create(AbstractIlluminationDevice_Zone dz, IlluminationMode mode, ColorStatic color, RateRange rate)
		{
			ColorHandler colorHandler = ColorHandler.Create(dz, mode);
			colorHandler.color_static = color;
			colorHandler.rate_range = rate;
			return colorHandler;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x000090E6 File Offset: 0x000072E6
		public static ColorHandler Create(AbstractIlluminationDevice_Zone dz, IlluminationMode mode, ColorGradient color)
		{
			ColorHandler colorHandler = ColorHandler.Create(dz, mode);
			colorHandler.color_gradient = color;
			return colorHandler;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x000090F6 File Offset: 0x000072F6
		public static ColorHandler Create(AbstractIlluminationDevice_Zone dz, IlluminationMode mode, ColorGradient color, RateStatic rate)
		{
			ColorHandler colorHandler = ColorHandler.Create(dz, mode);
			colorHandler.color_gradient = color;
			colorHandler.rate_static = rate;
			return colorHandler;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000910D File Offset: 0x0000730D
		public static ColorHandler Create(AbstractIlluminationDevice_Zone dz, IlluminationMode mode, ColorGradient color, RateRange rate)
		{
			ColorHandler colorHandler = ColorHandler.Create(dz, mode);
			colorHandler.color_gradient = color;
			colorHandler.rate_range = rate;
			return colorHandler;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00009124 File Offset: 0x00007324
		public static ColorHandler Create(AbstractIlluminationDevice_Zone dz, IlluminationMode mode, ColorRanges color)
		{
			ColorHandler colorHandler = ColorHandler.Create(dz, mode);
			colorHandler.color_range = color;
			return colorHandler;
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00009134 File Offset: 0x00007334
		public static ColorHandler Create(AbstractIlluminationDevice_Zone dz, IlluminationMode mode, ColorRanges color, RateStatic rate)
		{
			ColorHandler colorHandler = ColorHandler.Create(dz, mode);
			colorHandler.color_range = color;
			colorHandler.rate_static = rate;
			return colorHandler;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000914B File Offset: 0x0000734B
		public static ColorHandler Create(AbstractIlluminationDevice_Zone dz, IlluminationMode mode, ColorRanges color, RateRange rate)
		{
			ColorHandler colorHandler = ColorHandler.Create(dz, mode);
			colorHandler.color_range = color;
			colorHandler.rate_range = rate;
			return colorHandler;
		}

		// Token: 0x04000210 RID: 528
		private ColorEffect _colorEffect;

		// Token: 0x04000211 RID: 529
		private RateMode _rateMode;

		// Token: 0x04000212 RID: 530
		public AbstractIlluminationDevice_Zone deviceZone;

		// Token: 0x04000213 RID: 531
		public IlluminationMode mode;

		// Token: 0x04000214 RID: 532
		public AbstractColor color;

		// Token: 0x04000215 RID: 533
		public AbstractRate rate;
	}
}
