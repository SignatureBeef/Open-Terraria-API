using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000057 RID: 87
	[fsObject(Converter = typeof(SendEventConverter))]
	public class Send_Event
	{
		// Token: 0x0400016B RID: 363
		public string game;

		// Token: 0x0400016C RID: 364
		public string event_name;

		// Token: 0x0400016D RID: 365
		public EventData data;
	}
}
