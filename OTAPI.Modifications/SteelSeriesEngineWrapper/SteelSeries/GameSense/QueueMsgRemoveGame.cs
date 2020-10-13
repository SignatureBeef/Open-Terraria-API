using System;

namespace SteelSeries.GameSense
{
	// Token: 0x0200004F RID: 79
	public class QueueMsgRemoveGame : QueueMsg
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060001AF RID: 431 RVA: 0x00008B27 File Offset: 0x00006D27
		public override Uri uri
		{
			get
			{
				return QueueMsgRemoveGame._uri;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x00008B2E File Offset: 0x00006D2E
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x00008AF3 File Offset: 0x00006CF3
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

		// Token: 0x060001B2 RID: 434 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool IsCritical()
		{
			return false;
		}

		// Token: 0x04000160 RID: 352
		public static Uri _uri;
	}
}
