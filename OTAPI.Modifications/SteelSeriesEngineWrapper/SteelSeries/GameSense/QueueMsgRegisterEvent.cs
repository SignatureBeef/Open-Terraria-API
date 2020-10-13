using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200004D RID: 77
	public class QueueMsgRegisterEvent : QueueMsg
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x00008AFF File Offset: 0x00006CFF
		public override Uri uri
		{
			get
			{
				return QueueMsgRegisterEvent._uri;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x00008B06 File Offset: 0x00006D06
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x00008AF3 File Offset: 0x00006CF3
		public override object data
		{
			get
			{
				return this._data as Register_Event;
			}
			set
			{
				this._data = value;
			}
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00008AFC File Offset: 0x00006CFC
		public override bool IsCritical()
		{
			return true;
		}

		// Token: 0x0400015E RID: 350
		public static Uri _uri;
	}
}
