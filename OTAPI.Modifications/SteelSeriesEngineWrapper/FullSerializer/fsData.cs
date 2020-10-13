using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FullSerializer
{
	// Token: 0x02000009 RID: 9
	public sealed class fsData
	{
		// Token: 0x0600001F RID: 31 RVA: 0x0000290F File Offset: 0x00000B0F
		public fsData()
		{
			this._value = null;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x0000291E File Offset: 0x00000B1E
		public fsData(bool boolean)
		{
			this._value = boolean;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002932 File Offset: 0x00000B32
		public fsData(double f)
		{
			this._value = f;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002946 File Offset: 0x00000B46
		public fsData(long i)
		{
			this._value = i;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000295A File Offset: 0x00000B5A
		public fsData(string str)
		{
			this._value = str;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000295A File Offset: 0x00000B5A
		public fsData(Dictionary<string, fsData> dict)
		{
			this._value = dict;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000295A File Offset: 0x00000B5A
		public fsData(List<fsData> list)
		{
			this._value = list;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002969 File Offset: 0x00000B69
		public static fsData CreateDictionary()
		{
			return new fsData(new Dictionary<string, fsData>(fsGlobalConfig.IsCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase));
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002988 File Offset: 0x00000B88
		public static fsData CreateList()
		{
			return new fsData(new List<fsData>());
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002994 File Offset: 0x00000B94
		public static fsData CreateList(int capacity)
		{
			return new fsData(new List<fsData>(capacity));
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000029A1 File Offset: 0x00000BA1
		internal void BecomeDictionary()
		{
			this._value = new Dictionary<string, fsData>();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000029AE File Offset: 0x00000BAE
		internal fsData Clone()
		{
			return new fsData
			{
				_value = this._value
			};
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000029C4 File Offset: 0x00000BC4
		public fsDataType Type
		{
			get
			{
				if (this._value == null)
				{
					return fsDataType.Null;
				}
				if (this._value is double)
				{
					return fsDataType.Double;
				}
				if (this._value is long)
				{
					return fsDataType.Int64;
				}
				if (this._value is bool)
				{
					return fsDataType.Boolean;
				}
				if (this._value is string)
				{
					return fsDataType.String;
				}
				if (this._value is Dictionary<string, fsData>)
				{
					return fsDataType.Object;
				}
				if (this._value is List<fsData>)
				{
					return fsDataType.Array;
				}
				throw new InvalidOperationException("unknown JSON data type");
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002A3F File Offset: 0x00000C3F
		public bool IsNull
		{
			get
			{
				return this._value == null;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002A4A File Offset: 0x00000C4A
		public bool IsDouble
		{
			get
			{
				return this._value is double;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002A5A File Offset: 0x00000C5A
		public bool IsInt64
		{
			get
			{
				return this._value is long;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002A6A File Offset: 0x00000C6A
		public bool IsBool
		{
			get
			{
				return this._value is bool;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002A7A File Offset: 0x00000C7A
		public bool IsString
		{
			get
			{
				return this._value is string;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002A8A File Offset: 0x00000C8A
		public bool IsDictionary
		{
			get
			{
				return this._value is Dictionary<string, fsData>;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002A9A File Offset: 0x00000C9A
		public bool IsList
		{
			get
			{
				return this._value is List<fsData>;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002AAA File Offset: 0x00000CAA
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public double AsDouble
		{
			get
			{
				return this.Cast<double>();
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002AB2 File Offset: 0x00000CB2
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public long AsInt64
		{
			get
			{
				return this.Cast<long>();
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002ABA File Offset: 0x00000CBA
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool AsBool
		{
			get
			{
				return this.Cast<bool>();
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002AC2 File Offset: 0x00000CC2
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public string AsString
		{
			get
			{
				return this.Cast<string>();
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002ACA File Offset: 0x00000CCA
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Dictionary<string, fsData> AsDictionary
		{
			get
			{
				return this.Cast<Dictionary<string, fsData>>();
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002AD2 File Offset: 0x00000CD2
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public List<fsData> AsList
		{
			get
			{
				return this.Cast<List<fsData>>();
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002ADC File Offset: 0x00000CDC
		private T Cast<T>()
		{
			if (this._value is T)
			{
				return (T)((object)this._value);
			}
			throw new InvalidCastException(string.Concat(new object[]
			{
				"Unable to cast <",
				this,
				"> (with type = ",
				this._value.GetType(),
				") to type ",
				typeof(T)
			}));
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002B49 File Offset: 0x00000D49
		public override string ToString()
		{
			return fsJsonPrinter.CompressedJson(this);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002B51 File Offset: 0x00000D51
		public override bool Equals(object obj)
		{
			return this.Equals(obj as fsData);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002B60 File Offset: 0x00000D60
		public bool Equals(fsData other)
		{
			if (other == null || this.Type != other.Type)
			{
				return false;
			}
			switch (this.Type)
			{
			case fsDataType.Array:
			{
				List<fsData> asList = this.AsList;
				List<fsData> asList2 = other.AsList;
				if (asList.Count != asList2.Count)
				{
					return false;
				}
				for (int i = 0; i < asList.Count; i++)
				{
					if (!asList[i].Equals(asList2[i]))
					{
						return false;
					}
				}
				return true;
			}
			case fsDataType.Object:
			{
				Dictionary<string, fsData> asDictionary = this.AsDictionary;
				Dictionary<string, fsData> asDictionary2 = other.AsDictionary;
				if (asDictionary.Count != asDictionary2.Count)
				{
					return false;
				}
				foreach (string key in asDictionary.Keys)
				{
					if (!asDictionary2.ContainsKey(key))
					{
						return false;
					}
					if (!asDictionary[key].Equals(asDictionary2[key]))
					{
						return false;
					}
				}
				return true;
			}
			case fsDataType.Double:
				return this.AsDouble == other.AsDouble || Math.Abs(this.AsDouble - other.AsDouble) < double.Epsilon;
			case fsDataType.Int64:
				return this.AsInt64 == other.AsInt64;
			case fsDataType.Boolean:
				return this.AsBool == other.AsBool;
			case fsDataType.String:
				return this.AsString == other.AsString;
			case fsDataType.Null:
				return true;
			default:
				throw new Exception("Unknown data type");
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002D00 File Offset: 0x00000F00
		public static bool operator ==(fsData a, fsData b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null || b == null)
			{
				return false;
			}
			if (a.IsDouble && b.IsDouble)
			{
				return Math.Abs(a.AsDouble - b.AsDouble) < double.Epsilon;
			}
			return a.Equals(b);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002D50 File Offset: 0x00000F50
		public static bool operator !=(fsData a, fsData b)
		{
			return !(a == b);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002D5C File Offset: 0x00000F5C
		public override int GetHashCode()
		{
			return this._value.GetHashCode();
		}

		// Token: 0x04000010 RID: 16
		private object _value;

		// Token: 0x04000011 RID: 17
		public static readonly fsData True = new fsData(true);

		// Token: 0x04000012 RID: 18
		public static readonly fsData False = new fsData(false);

		// Token: 0x04000013 RID: 19
		public static readonly fsData Null = new fsData();
	}
}
