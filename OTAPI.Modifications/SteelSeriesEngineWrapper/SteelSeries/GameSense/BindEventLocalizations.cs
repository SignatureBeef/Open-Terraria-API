using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x0200003D RID: 61
	[fsObject(Converter = typeof(BindEventLocalizationsConverter))]
	public class BindEventLocalizations
	{
		// Token: 0x0400006E RID: 110
		public Dictionary<string, string> AvailableLocalizations;
	}
}
