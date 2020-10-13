using System;

namespace SteelSeries.GameSense.DeviceZone
{
	// Token: 0x02000082 RID: 130
	public abstract class AbstractIlluminationDevice_StandardZone : AbstractIlluminationDevice_Zone
	{
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000259 RID: 601 RVA: 0x000098A1 File Offset: 0x00007AA1
		public string zone
		{
			get
			{
				return this._zone;
			}
		}

		// Token: 0x0600025A RID: 602 RVA: 0x000098A9 File Offset: 0x00007AA9
		public AbstractIlluminationDevice_StandardZone(string device, string zone) : base(device)
		{
			this._zone = zone;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool HasCustomZone()
		{
			return false;
		}

		// Token: 0x0400022D RID: 557
		protected string _zone;
	}
}
