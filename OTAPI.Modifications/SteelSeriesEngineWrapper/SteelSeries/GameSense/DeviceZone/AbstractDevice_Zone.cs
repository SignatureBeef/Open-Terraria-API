using System;

namespace SteelSeries.GameSense.DeviceZone
{
	// Token: 0x02000080 RID: 128
	public abstract class AbstractDevice_Zone
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000255 RID: 597 RVA: 0x0000987A File Offset: 0x00007A7A
		public string device
		{
			get
			{
				return this._device;
			}
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00009882 File Offset: 0x00007A82
		public AbstractDevice_Zone(string device)
		{
			this._device = device;
		}

		// Token: 0x0400022B RID: 555
		protected string _device;
	}
}
