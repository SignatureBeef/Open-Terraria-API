using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000074 RID: 116
	[fsObject(Converter = typeof(PartialBitmapEventTypeConverter))]
	public class PartialBitmapEventHandlerType : AbstractHandler
	{
		// Token: 0x0400021C RID: 540
		public string[] EventsToExclude;
	}
}
