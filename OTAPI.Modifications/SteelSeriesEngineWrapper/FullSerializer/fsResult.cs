using System;
using System.Collections.Generic;
using System.Linq;

namespace FullSerializer
{
	// Token: 0x02000018 RID: 24
	public struct fsResult
	{
		// Token: 0x0600007D RID: 125 RVA: 0x000040E6 File Offset: 0x000022E6
		public void AddMessage(string message)
		{
			if (this._messages == null)
			{
				this._messages = new List<string>();
			}
			this._messages.Add(message);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004107 File Offset: 0x00002307
		public void AddMessages(fsResult result)
		{
			if (result._messages == null)
			{
				return;
			}
			if (this._messages == null)
			{
				this._messages = new List<string>();
			}
			this._messages.AddRange(result._messages);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004138 File Offset: 0x00002338
		public fsResult Merge(fsResult other)
		{
			this._success = (this._success && other._success);
			if (other._messages != null)
			{
				if (this._messages == null)
				{
					this._messages = new List<string>(other._messages);
				}
				else
				{
					this._messages.AddRange(other._messages);
				}
			}
			return this;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004198 File Offset: 0x00002398
		public static fsResult Warn(string warning)
		{
			return new fsResult
			{
				_success = true,
				_messages = new List<string>
				{
					warning
				}
			};
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000041CC File Offset: 0x000023CC
		public static fsResult Fail(string warning)
		{
			return new fsResult
			{
				_success = false,
				_messages = new List<string>
				{
					warning
				}
			};
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000041FD File Offset: 0x000023FD
		public static fsResult operator +(fsResult a, fsResult b)
		{
			return a.Merge(b);
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00004207 File Offset: 0x00002407
		public bool Failed
		{
			get
			{
				return !this._success;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00004212 File Offset: 0x00002412
		public bool Succeeded
		{
			get
			{
				return this._success;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000085 RID: 133 RVA: 0x0000421A File Offset: 0x0000241A
		public bool HasWarnings
		{
			get
			{
				return this._messages != null && this._messages.Any<string>();
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00004231 File Offset: 0x00002431
		public fsResult AssertSuccess()
		{
			if (this.Failed)
			{
				throw this.AsException;
			}
			return this;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004248 File Offset: 0x00002448
		public fsResult AssertSuccessWithoutWarnings()
		{
			if (this.Failed || this.RawMessages.Any<string>())
			{
				throw this.AsException;
			}
			return this;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000088 RID: 136 RVA: 0x0000426C File Offset: 0x0000246C
		public Exception AsException
		{
			get
			{
				if (!this.Failed && !this.RawMessages.Any<string>())
				{
					throw new Exception("Only a failed result can be converted to an exception");
				}
				return new Exception(this.FormattedMessages);
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00004299 File Offset: 0x00002499
		public IEnumerable<string> RawMessages
		{
			get
			{
				if (this._messages != null)
				{
					return this._messages;
				}
				return fsResult.EmptyStringArray;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600008A RID: 138 RVA: 0x000042AF File Offset: 0x000024AF
		public string FormattedMessages
		{
			get
			{
				return string.Join(",\n", this.RawMessages.ToArray<string>());
			}
		}

		// Token: 0x0400002C RID: 44
		private static readonly string[] EmptyStringArray = new string[0];

		// Token: 0x0400002D RID: 45
		private bool _success;

		// Token: 0x0400002E RID: 46
		private List<string> _messages;

		// Token: 0x0400002F RID: 47
		public static fsResult Success = new fsResult
		{
			_success = true
		};
	}
}
