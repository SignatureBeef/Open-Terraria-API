using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200004C RID: 76
	public class QueueMsgBindEvent : QueueMsg
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600019D RID: 413 RVA: 0x00008ADF File Offset: 0x00006CDF
		public override Uri uri
		{
			get
			{
				return QueueMsgBindEvent._uri;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600019E RID: 414 RVA: 0x00008AE6 File Offset: 0x00006CE6
		// (set) Token: 0x0600019F RID: 415 RVA: 0x00008AF3 File Offset: 0x00006CF3
		public override object data
		{
			get
			{
				return this._data as Bind_Event;
			}
			set
			{
				this._data = value;
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00008AFC File Offset: 0x00006CFC
		public override bool IsCritical()
		{
			return true;
		}

		// Token: 0x0400015D RID: 349
		public static Uri _uri;
	}
}
