using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000061 RID: 97
	[fsObject(Converter = typeof(TactileEffectCustomConverter))]
	[Serializable]
	public class TactileEffectCustom
	{
		// Token: 0x060001D6 RID: 470 RVA: 0x00008D8D File Offset: 0x00006F8D
		public TactileEffectCustom(uint length, uint delay = 0U)
		{
			this.length_ms = length;
			this.delay_ms = delay;
		}

		// Token: 0x04000184 RID: 388
		public const string type = "custom";

		// Token: 0x04000185 RID: 389
		public uint length_ms;

		// Token: 0x04000186 RID: 390
		public uint delay_ms;
	}
}
