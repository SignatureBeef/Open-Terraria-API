using System;

namespace SteelSeries.GameSense.DeviceZone
{
	// Token: 0x0200009F RID: 159
	public class RGBPerkeyZoneCustom : AbstractIlluminationDevice_CustomZone
	{
		// Token: 0x06000279 RID: 633 RVA: 0x00009A1D File Offset: 0x00007C1D
		public RGBPerkeyZoneCustom() : base("rgb-per-key-zones", null)
		{
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00009A2B File Offset: 0x00007C2B
		public RGBPerkeyZoneCustom(byte[] hidcodes) : base("rgb-per-key-zones", hidcodes)
		{
		}
	}
}
