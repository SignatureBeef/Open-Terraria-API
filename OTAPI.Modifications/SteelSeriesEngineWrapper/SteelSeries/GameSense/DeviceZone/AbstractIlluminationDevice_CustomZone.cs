using System;

namespace SteelSeries.GameSense.DeviceZone
{
	// Token: 0x02000081 RID: 129
	public abstract class AbstractIlluminationDevice_CustomZone : AbstractIlluminationDevice_Zone
	{
		// Token: 0x06000257 RID: 599 RVA: 0x00009891 File Offset: 0x00007A91
		public AbstractIlluminationDevice_CustomZone(string device, byte[] zone) : base(device)
		{
			this.zone = zone;
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00008AFC File Offset: 0x00006CFC
		public override bool HasCustomZone()
		{
			return true;
		}

		// Token: 0x0400022C RID: 556
		public byte[] zone;
	}
}
