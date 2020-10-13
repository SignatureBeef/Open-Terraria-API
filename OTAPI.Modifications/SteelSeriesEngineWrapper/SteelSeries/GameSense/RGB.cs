using System;

namespace SteelSeries.GameSense
{
	// Token: 0x02000060 RID: 96
	[Serializable]
	public struct RGB
	{
		// Token: 0x060001D5 RID: 469 RVA: 0x00008D76 File Offset: 0x00006F76
		public RGB(byte r, byte g, byte b)
		{
			this.red = r;
			this.green = g;
			this.blue = b;
		}

		// Token: 0x04000181 RID: 385
		public byte red;

		// Token: 0x04000182 RID: 386
		public byte green;

		// Token: 0x04000183 RID: 387
		public byte blue;
	}
}
