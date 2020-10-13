using System;

namespace SteelSeries.GameSense
{
	// Token: 0x02000059 RID: 89
	[Serializable]
	public struct Gradient
	{
		// Token: 0x060001CE RID: 462 RVA: 0x00008CBE File Offset: 0x00006EBE
		public Gradient(RGB zero, RGB hundred)
		{
			this.zero = zero;
			this.hundred = hundred;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00008CCE File Offset: 0x00006ECE
		public Gradient(byte r0, byte g0, byte b0, byte r1, byte g1, byte b1)
		{
			this.zero = new RGB(r0, g0, b0);
			this.hundred = new RGB(r1, g1, b1);
		}

		// Token: 0x0400016E RID: 366
		public RGB zero;

		// Token: 0x0400016F RID: 367
		public RGB hundred;
	}
}
