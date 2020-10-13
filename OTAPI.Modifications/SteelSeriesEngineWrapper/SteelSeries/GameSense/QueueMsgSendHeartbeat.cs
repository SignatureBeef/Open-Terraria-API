using System;

namespace SteelSeries.GameSense
{
	// Token: 0x02000051 RID: 81
	public class QueueMsgSendHeartbeat : QueueMsg
	{
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001BB RID: 443 RVA: 0x00008B48 File Offset: 0x00006D48
		public override Uri uri
		{
			get
			{
				return QueueMsgSendHeartbeat._uri;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00008B2E File Offset: 0x00006D2E
		// (set) Token: 0x060001BD RID: 445 RVA: 0x00008AF3 File Offset: 0x00006CF3
		public override object data
		{
			get
			{
				return this._data as Game;
			}
			set
			{
				this._data = value;
			}
		}

		// Token: 0x060001BE RID: 446 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool IsCritical()
		{
			return false;
		}

		// Token: 0x04000162 RID: 354
		public static Uri _uri;
	}
}
