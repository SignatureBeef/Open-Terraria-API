using System;
using System.Threading;

namespace SteelSeries.GameSense
{
	// Token: 0x02000049 RID: 73
	public class LocklessQueue<T>
	{
		// Token: 0x06000193 RID: 403 RVA: 0x000089B2 File Offset: 0x00006BB2
		public LocklessQueue(uint size)
		{
			this._bfr = new T[size];
			this._length = (int)size;
			this._readIdx = 0;
			this._maxReadIdx = 0;
			this._writeIdx = 0;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x000089E2 File Offset: 0x00006BE2
		private int index(int i)
		{
			return i % this._length;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x000089EC File Offset: 0x00006BEC
		public bool PEnqueue(T obj)
		{
			int writeIdx;
			for (;;)
			{
				writeIdx = this._writeIdx;
				int readIdx = this._readIdx;
				if (this.index(writeIdx + 1) == this.index(readIdx))
				{
					break;
				}
				if (Interlocked.CompareExchange(ref this._writeIdx, this.index(writeIdx + 1), writeIdx) == writeIdx)
				{
					goto Block_1;
				}
			}
			return false;
			Block_1:
			this._bfr[this.index(writeIdx)] = obj;
			while (Interlocked.CompareExchange(ref this._maxReadIdx, this.index(writeIdx + 1), writeIdx) != writeIdx)
			{
			}
			return true;
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00008A60 File Offset: 0x00006C60
		public T CDequeue()
		{
			T result = default(T);
			for (;;)
			{
				int readIdx = this._readIdx;
				int maxReadIdx = this._maxReadIdx;
				if (this.index(readIdx) == this.index(maxReadIdx))
				{
					break;
				}
				result = this._bfr[this.index(readIdx)];
				if (Interlocked.CompareExchange(ref this._readIdx, readIdx + 1, readIdx) != this._readIdx)
				{
					return result;
				}
			}
			return default(T);
		}

		// Token: 0x04000153 RID: 339
		private T[] _bfr;

		// Token: 0x04000154 RID: 340
		private int _length;

		// Token: 0x04000155 RID: 341
		private int _readIdx;

		// Token: 0x04000156 RID: 342
		private int _maxReadIdx;

		// Token: 0x04000157 RID: 343
		private int _writeIdx;
	}
}
