using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000054 RID: 84
	[fsObject(Converter = typeof(RegisterEventConverter))]
	public class Register_Event
	{
		// Token: 0x060001C7 RID: 455 RVA: 0x00008C2A File Offset: 0x00006E2A
		public Register_Event(string gameName, string eventName, int minValue, int maxValue, EventIconId iconId)
		{
			this.game = gameName;
			this.eventName = eventName;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.iconId = iconId;
		}

		// Token: 0x04000163 RID: 355
		[NonSerialized]
		public string game;

		// Token: 0x04000164 RID: 356
		public string eventName;

		// Token: 0x04000165 RID: 357
		public int minValue;

		// Token: 0x04000166 RID: 358
		public int maxValue;

		// Token: 0x04000167 RID: 359
		public EventIconId iconId;
	}
}
