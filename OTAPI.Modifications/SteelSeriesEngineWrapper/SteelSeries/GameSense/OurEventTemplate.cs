using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200004A RID: 74
	public class OurEventTemplate
	{
		// Token: 0x06000197 RID: 407 RVA: 0x00008AC8 File Offset: 0x00006CC8
		public OurEventTemplate(string upperCaseEventNameToUse)
		{
			this.upperCaseEventName = upperCaseEventNameToUse;
		}

		// Token: 0x04000158 RID: 344
		public string upperCaseEventName;

		// Token: 0x04000159 RID: 345
		public int minimumValue;

		// Token: 0x0400015A RID: 346
		public int maximumValue = 100;

		// Token: 0x0400015B RID: 347
		public EventIconId IconId;
	}
}
