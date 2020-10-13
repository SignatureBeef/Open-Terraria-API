using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200004E RID: 78
	public class QueueMsgRegisterGame : QueueMsg
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060001A9 RID: 425 RVA: 0x00008B13 File Offset: 0x00006D13
		public override Uri uri
		{
			get
			{
				return QueueMsgRegisterGame._uri;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060001AA RID: 426 RVA: 0x00008B1A File Offset: 0x00006D1A
		// (set) Token: 0x060001AB RID: 427 RVA: 0x00008AF3 File Offset: 0x00006CF3
		public override object data
		{
			get
			{
				return this._data as Register_Game;
			}
			set
			{
				this._data = value;
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00008AFC File Offset: 0x00006CFC
		public override bool IsCritical()
		{
			return true;
		}

		// Token: 0x0400015F RID: 351
		public static Uri _uri;
	}
}
