using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200006D RID: 109
	public class ColorGradient : AbstractColor_Nonrecursive
	{
		// Token: 0x060001EE RID: 494 RVA: 0x00008AFC File Offset: 0x00006CFC
		public override ColorEffect ColorEffectType()
		{
			return ColorEffect.Gradient;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x00008FB3 File Offset: 0x000071B3
		private static ColorGradient _new()
		{
			return new ColorGradient();
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x00008FBA File Offset: 0x000071BA
		public static ColorGradient Create(Gradient gradient)
		{
			ColorGradient colorGradient = ColorGradient._new();
			colorGradient.gradient = gradient;
			return colorGradient;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00008FC8 File Offset: 0x000071C8
		public static ColorGradient Create(RGB zero, RGB hundred)
		{
			ColorGradient colorGradient = ColorGradient._new();
			colorGradient.gradient = new Gradient(zero, hundred);
			return colorGradient;
		}

		// Token: 0x0400020F RID: 527
		public Gradient gradient;
	}
}
