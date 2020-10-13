using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x0200003F RID: 63
	[fsObject(Converter = typeof(BindEventConverter))]
	public class Bind_Event
	{
		// Token: 0x06000186 RID: 390 RVA: 0x00008454 File Offset: 0x00006654
		public Bind_Event(string gameName, string eventName, int minValue, int maxValue, EventIconId iconId, AbstractHandler[] handlers)
		{
			this.game = gameName;
			this.eventName = eventName;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.iconId = iconId;
			this.handlers = handlers;
			this.defaultDisplayName = eventName;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00008490 File Offset: 0x00006690
		public Bind_Event(string gameName, string eventName, string defaultDisplayName, Dictionary<string, string> localizedDisplayNames, int minValue, int maxValue, EventIconId iconId, AbstractHandler[] handlers)
		{
			this.game = gameName;
			this.eventName = eventName;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.iconId = iconId;
			this.handlers = handlers;
			this.defaultDisplayName = defaultDisplayName;
			this.localizedDisplayNames = localizedDisplayNames;
		}

		// Token: 0x0400006F RID: 111
		[NonSerialized]
		public string game;

		// Token: 0x04000070 RID: 112
		public string eventName;

		// Token: 0x04000071 RID: 113
		public int minValue;

		// Token: 0x04000072 RID: 114
		public int maxValue;

		// Token: 0x04000073 RID: 115
		public EventIconId iconId;

		// Token: 0x04000074 RID: 116
		public AbstractHandler[] handlers;

		// Token: 0x04000075 RID: 117
		public string defaultDisplayName;

		// Token: 0x04000076 RID: 118
		public Dictionary<string, string> localizedDisplayNames;
	}
}
