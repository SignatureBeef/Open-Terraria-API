using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000039 RID: 57
	[fsObject(Converter = typeof(ColorRangeConverter))]
	[Serializable]
	public class ColorRange
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000173 RID: 371 RVA: 0x000081E3 File Offset: 0x000063E3
		public RangeColorEffect ColorEffect
		{
			get
			{
				return this._effect;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000174 RID: 372 RVA: 0x000081EB File Offset: 0x000063EB
		// (set) Token: 0x06000175 RID: 373 RVA: 0x000081F8 File Offset: 0x000063F8
		public ColorStatic color_static
		{
			get
			{
				return this.color as ColorStatic;
			}
			set
			{
				this.color = value;
				this._effect = RangeColorEffect.Static;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000176 RID: 374 RVA: 0x00008208 File Offset: 0x00006408
		// (set) Token: 0x06000177 RID: 375 RVA: 0x00008215 File Offset: 0x00006415
		public ColorGradient color_gradient
		{
			get
			{
				return this.color as ColorGradient;
			}
			set
			{
				this.color = value;
				this._effect = RangeColorEffect.Gradient;
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00008225 File Offset: 0x00006425
		public ColorRange(uint low, uint high, ColorStatic color)
		{
			this.low = low;
			this.high = high;
			this.color_static = color;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00008242 File Offset: 0x00006442
		public ColorRange(uint low, uint high, ColorGradient color)
		{
			this.low = low;
			this.high = high;
			this.color_gradient = color;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000825F File Offset: 0x0000645F
		public ColorRange(uint low, uint high, RGB zero, RGB hundred)
		{
			this.low = low;
			this.high = high;
			this.color_gradient = ColorGradient.Create(zero, hundred);
		}

		// Token: 0x04000066 RID: 102
		private RangeColorEffect _effect;

		// Token: 0x04000067 RID: 103
		public uint low;

		// Token: 0x04000068 RID: 104
		public uint high;

		// Token: 0x04000069 RID: 105
		public AbstractColor_Nonrecursive color;
	}
}
