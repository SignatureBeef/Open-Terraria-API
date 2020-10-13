using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000065 RID: 101
	[fsObject(Converter = typeof(TactileEffectSimpleConverter))]
	[Serializable]
	public class TactileEffectSimple
	{
		// Token: 0x060001E5 RID: 485 RVA: 0x00008F12 File Offset: 0x00007112
		public TactileEffectSimple(TactileEffectType effect, uint delay = 0U)
		{
			this.type = effect;
			this.delay_ms = delay;
		}

		// Token: 0x0400018B RID: 395
		public TactileEffectType type;

		// Token: 0x0400018C RID: 396
		public uint delay_ms;
	}
}
