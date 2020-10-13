using System;

namespace SteelSeries.GameSense
{
	// Token: 0x02000071 RID: 113
	public class ColorStatic : AbstractColor_Nonrecursive
	{
		// Token: 0x06000212 RID: 530 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override ColorEffect ColorEffectType()
		{
			return ColorEffect.Static;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x000092CB File Offset: 0x000074CB
		private static ColorStatic _new()
		{
			return new ColorStatic();
		}

		// Token: 0x06000214 RID: 532 RVA: 0x000092D2 File Offset: 0x000074D2
		public static ColorStatic Create(byte r, byte g, byte b)
		{
			ColorStatic colorStatic = ColorStatic._new();
			colorStatic.red = r;
			colorStatic.green = g;
			colorStatic.blue = b;
			return colorStatic;
		}

		// Token: 0x04000217 RID: 535
		public byte red;

		// Token: 0x04000218 RID: 536
		public byte green;

		// Token: 0x04000219 RID: 537
		public byte blue;
	}
}
