using System;

namespace SteelSeries.GameSense.DeviceZone
{
	// Token: 0x02000083 RID: 131
	public abstract class AbstractIlluminationDevice_Zone : AbstractDevice_Zone
	{
		// Token: 0x0600025C RID: 604 RVA: 0x000098B9 File Offset: 0x00007AB9
		public AbstractIlluminationDevice_Zone(string device) : base(device)
		{
		}

		// Token: 0x0600025D RID: 605
		public abstract bool HasCustomZone();
	}
}
