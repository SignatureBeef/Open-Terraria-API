using System;

namespace SteelSeries.GameSense.DeviceZone
{
	// Token: 0x0200007F RID: 127
	public class AbstarctGenericTactile_Zone : AbstractTactileDevice_Zone
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000253 RID: 595 RVA: 0x00009862 File Offset: 0x00007A62
		public string zone
		{
			get
			{
				return this._zone;
			}
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000986A File Offset: 0x00007A6A
		public AbstarctGenericTactile_Zone(string device, string zone) : base(device)
		{
			this._zone = zone;
		}

		// Token: 0x0400022A RID: 554
		protected string _zone;
	}
}
