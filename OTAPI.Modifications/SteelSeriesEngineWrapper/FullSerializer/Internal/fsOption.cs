using System;

namespace FullSerializer.Internal
{
	// Token: 0x0200002B RID: 43
	public struct fsOption<T>
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000129 RID: 297 RVA: 0x000075C1 File Offset: 0x000057C1
		public bool HasValue
		{
			get
			{
				return this._hasValue;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600012A RID: 298 RVA: 0x000075C9 File Offset: 0x000057C9
		public bool IsEmpty
		{
			get
			{
				return !this._hasValue;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600012B RID: 299 RVA: 0x000075D4 File Offset: 0x000057D4
		public T Value
		{
			get
			{
				if (this.IsEmpty)
				{
					throw new InvalidOperationException("fsOption is empty");
				}
				return this._value;
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000075EF File Offset: 0x000057EF
		public fsOption(T value)
		{
			this._hasValue = true;
			this._value = value;
		}

		// Token: 0x0400004D RID: 77
		private bool _hasValue;

		// Token: 0x0400004E RID: 78
		private T _value;

		// Token: 0x0400004F RID: 79
		public static fsOption<T> Empty;
	}
}
