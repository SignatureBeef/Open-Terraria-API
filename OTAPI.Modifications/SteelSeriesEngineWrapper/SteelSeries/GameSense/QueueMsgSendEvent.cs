using System;

namespace SteelSeries.GameSense
{
	// Token: 0x02000050 RID: 80
	public class QueueMsgSendEvent : QueueMsg
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x00008981 File Offset: 0x00006B81
		public override Uri uri
		{
			get
			{
				return QueueMsgSendEvent._uri;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x00008B3B File Offset: 0x00006D3B
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x00008AF3 File Offset: 0x00006CF3
		public override object data
		{
			get
			{
				return this._data as Send_Event;
			}
			set
			{
				this._data = value;
			}
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool IsCritical()
		{
			return false;
		}

		// Token: 0x04000161 RID: 353
		public static Uri _uri;
	}
}
