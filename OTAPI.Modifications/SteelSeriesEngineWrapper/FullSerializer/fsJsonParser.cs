using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FullSerializer
{
	// Token: 0x02000012 RID: 18
	public class fsJsonParser
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00002E80 File Offset: 0x00001080
		private fsResult MakeFailure(string message)
		{
			int num = Math.Max(0, this._start - 20);
			int length = Math.Min(50, this._input.Length - num);
			return fsResult.Fail(string.Concat(new string[]
			{
				"Error while parsing: ",
				message,
				"; context = <",
				this._input.Substring(num, length),
				">"
			}));
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002EEE File Offset: 0x000010EE
		private bool TryMoveNext()
		{
			if (this._start < this._input.Length)
			{
				this._start++;
				return true;
			}
			return false;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002F14 File Offset: 0x00001114
		private bool HasValue()
		{
			return this.HasValue(0);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002F1D File Offset: 0x0000111D
		private bool HasValue(int offset)
		{
			return this._start + offset >= 0 && this._start + offset < this._input.Length;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002F41 File Offset: 0x00001141
		private char Character()
		{
			return this.Character(0);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002F4A File Offset: 0x0000114A
		private char Character(int offset)
		{
			return this._input[this._start + offset];
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002F60 File Offset: 0x00001160
		private void SkipSpace()
		{
			while (this.HasValue())
			{
				if (char.IsWhiteSpace(this.Character()))
				{
					this.TryMoveNext();
				}
				else
				{
					if (!this.HasValue(1) || this.Character(0) != '/')
					{
						break;
					}
					if (this.Character(1) == '/')
					{
						while (this.HasValue())
						{
							if (Environment.NewLine.Contains(this.Character().ToString() ?? ""))
							{
								break;
							}
							this.TryMoveNext();
						}
					}
					else if (this.Character(1) == '*')
					{
						this.TryMoveNext();
						this.TryMoveNext();
						while (this.HasValue(1))
						{
							if (this.Character(0) == '*' && this.Character(1) == '/')
							{
								this.TryMoveNext();
								this.TryMoveNext();
								this.TryMoveNext();
								break;
							}
							this.TryMoveNext();
						}
					}
				}
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003049 File Offset: 0x00001249
		private bool IsHex(char c)
		{
			return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003070 File Offset: 0x00001270
		private uint ParseSingleChar(char c1, uint multipliyer)
		{
			uint result = 0U;
			if (c1 >= '0' && c1 <= '9')
			{
				result = (uint)(c1 - '0') * multipliyer;
			}
			else if (c1 >= 'A' && c1 <= 'F')
			{
				result = (uint)(c1 - 'A' + '\n') * multipliyer;
			}
			else if (c1 >= 'a' && c1 <= 'f')
			{
				result = (uint)(c1 - 'a' + '\n') * multipliyer;
			}
			return result;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000030C0 File Offset: 0x000012C0
		private uint ParseUnicode(char c1, char c2, char c3, char c4)
		{
			uint num = this.ParseSingleChar(c1, 4096U);
			uint num2 = this.ParseSingleChar(c2, 256U);
			uint num3 = this.ParseSingleChar(c3, 16U);
			uint num4 = this.ParseSingleChar(c4, 1U);
			return num + num2 + num3 + num4;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003100 File Offset: 0x00001300
		private fsResult TryUnescapeChar(out char escaped)
		{
			this.TryMoveNext();
			if (!this.HasValue())
			{
				escaped = ' ';
				return this.MakeFailure("Unexpected end of input after \\");
			}
			char c = this.Character();
			if (c <= '\\')
			{
				if (c <= '/')
				{
					if (c == '"')
					{
						this.TryMoveNext();
						escaped = '"';
						return fsResult.Success;
					}
					if (c == '/')
					{
						this.TryMoveNext();
						escaped = '/';
						return fsResult.Success;
					}
				}
				else
				{
					if (c == '0')
					{
						this.TryMoveNext();
						escaped = '\0';
						return fsResult.Success;
					}
					if (c == '\\')
					{
						this.TryMoveNext();
						escaped = '\\';
						return fsResult.Success;
					}
				}
			}
			else if (c <= 'b')
			{
				if (c == 'a')
				{
					this.TryMoveNext();
					escaped = '\a';
					return fsResult.Success;
				}
				if (c == 'b')
				{
					this.TryMoveNext();
					escaped = '\b';
					return fsResult.Success;
				}
			}
			else
			{
				if (c == 'f')
				{
					this.TryMoveNext();
					escaped = '\f';
					return fsResult.Success;
				}
				if (c == 'n')
				{
					this.TryMoveNext();
					escaped = '\n';
					return fsResult.Success;
				}
				switch (c)
				{
				case 'r':
					this.TryMoveNext();
					escaped = '\r';
					return fsResult.Success;
				case 't':
					this.TryMoveNext();
					escaped = '\t';
					return fsResult.Success;
				case 'u':
					this.TryMoveNext();
					if (this.IsHex(this.Character(0)) && this.IsHex(this.Character(1)) && this.IsHex(this.Character(2)) && this.IsHex(this.Character(3)))
					{
						uint num = this.ParseUnicode(this.Character(0), this.Character(1), this.Character(2), this.Character(3));
						this.TryMoveNext();
						this.TryMoveNext();
						this.TryMoveNext();
						this.TryMoveNext();
						escaped = (char)num;
						return fsResult.Success;
					}
					escaped = '\0';
					return this.MakeFailure(string.Format("invalid escape sequence '\\u{0}{1}{2}{3}'\n", new object[]
					{
						this.Character(0),
						this.Character(1),
						this.Character(2),
						this.Character(3)
					}));
				}
			}
			escaped = '\0';
			return this.MakeFailure(string.Format("Invalid escape sequence \\{0}", this.Character()));
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003344 File Offset: 0x00001544
		private fsResult TryParseExact(string content)
		{
			for (int i = 0; i < content.Length; i++)
			{
				if (this.Character() != content[i])
				{
					return this.MakeFailure("Expected " + content[i].ToString());
				}
				if (!this.TryMoveNext())
				{
					return this.MakeFailure("Unexpected end of content when parsing " + content);
				}
			}
			return fsResult.Success;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000033B0 File Offset: 0x000015B0
		private fsResult TryParseTrue(out fsData data)
		{
			fsResult result = this.TryParseExact("true");
			if (result.Succeeded)
			{
				data = new fsData(true);
				return fsResult.Success;
			}
			data = null;
			return result;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000033E4 File Offset: 0x000015E4
		private fsResult TryParseFalse(out fsData data)
		{
			fsResult result = this.TryParseExact("false");
			if (result.Succeeded)
			{
				data = new fsData(false);
				return fsResult.Success;
			}
			data = null;
			return result;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003418 File Offset: 0x00001618
		private fsResult TryParseNull(out fsData data)
		{
			fsResult result = this.TryParseExact("null");
			if (result.Succeeded)
			{
				data = new fsData();
				return fsResult.Success;
			}
			data = null;
			return result;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x0000344B File Offset: 0x0000164B
		private bool IsSeparator(char c)
		{
			return char.IsWhiteSpace(c) || c == ',' || c == '}' || c == ']';
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003468 File Offset: 0x00001668
		private fsResult TryParseNumber(out fsData data)
		{
			int start = this._start;
			while (this.TryMoveNext() && this.HasValue() && !this.IsSeparator(this.Character()))
			{
			}
			string text = this._input.Substring(start, this._start - start);
			if (text.Contains(".") || text == "Infinity" || text == "-Infinity" || text == "NaN")
			{
				double f;
				if (!double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out f))
				{
					data = null;
					return this.MakeFailure("Bad double format with " + text);
				}
				data = new fsData(f);
				return fsResult.Success;
			}
			else
			{
				long i;
				if (!long.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out i))
				{
					data = null;
					return this.MakeFailure("Bad Int64 format with " + text);
				}
				data = new fsData(i);
				return fsResult.Success;
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003550 File Offset: 0x00001750
		private fsResult TryParseString(out string str)
		{
			this._cachedStringBuilder.Length = 0;
			if (this.Character() != '"' || !this.TryMoveNext())
			{
				str = string.Empty;
				return this.MakeFailure("Expected initial \" when parsing a string");
			}
			while (this.HasValue() && this.Character() != '"')
			{
				char c = this.Character();
				if (c == '\\')
				{
					char value;
					fsResult result = this.TryUnescapeChar(out value);
					if (result.Failed)
					{
						str = string.Empty;
						return result;
					}
					this._cachedStringBuilder.Append(value);
				}
				else
				{
					this._cachedStringBuilder.Append(c);
					if (!this.TryMoveNext())
					{
						str = string.Empty;
						return this.MakeFailure("Unexpected end of input when reading a string");
					}
				}
			}
			if (!this.HasValue() || this.Character() != '"' || !this.TryMoveNext())
			{
				str = string.Empty;
				return this.MakeFailure("No closing \" when parsing a string");
			}
			str = this._cachedStringBuilder.ToString();
			return fsResult.Success;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003640 File Offset: 0x00001840
		private fsResult TryParseArray(out fsData arr)
		{
			if (this.Character() != '[')
			{
				arr = null;
				return this.MakeFailure("Expected initial [ when parsing an array");
			}
			if (!this.TryMoveNext())
			{
				arr = null;
				return this.MakeFailure("Unexpected end of input when parsing an array");
			}
			this.SkipSpace();
			List<fsData> list = new List<fsData>();
			while (this.HasValue() && this.Character() != ']')
			{
				fsData item;
				fsResult result = this.RunParse(out item);
				if (result.Failed)
				{
					arr = null;
					return result;
				}
				list.Add(item);
				this.SkipSpace();
				if (this.HasValue() && this.Character() == ',')
				{
					if (!this.TryMoveNext())
					{
						break;
					}
					this.SkipSpace();
				}
			}
			if (!this.HasValue() || this.Character() != ']' || !this.TryMoveNext())
			{
				arr = null;
				return this.MakeFailure("No closing ] for array");
			}
			arr = new fsData(list);
			return fsResult.Success;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003718 File Offset: 0x00001918
		private fsResult TryParseObject(out fsData obj)
		{
			if (this.Character() != '{')
			{
				obj = null;
				return this.MakeFailure("Expected initial { when parsing an object");
			}
			if (!this.TryMoveNext())
			{
				obj = null;
				return this.MakeFailure("Unexpected end of input when parsing an object");
			}
			this.SkipSpace();
			Dictionary<string, fsData> dictionary = new Dictionary<string, fsData>(fsGlobalConfig.IsCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
			while (this.HasValue() && this.Character() != '}')
			{
				this.SkipSpace();
				string text;
				fsResult result = this.TryParseString(out text);
				if (result.Failed)
				{
					obj = null;
					return result;
				}
				this.SkipSpace();
				if (!this.HasValue() || this.Character() != ':' || !this.TryMoveNext())
				{
					obj = null;
					return this.MakeFailure("Expected : after key \"" + text + "\"");
				}
				this.SkipSpace();
				fsData value;
				result = this.RunParse(out value);
				if (result.Failed)
				{
					obj = null;
					return result;
				}
				dictionary.Add(text, value);
				this.SkipSpace();
				if (this.HasValue() && this.Character() == ',')
				{
					if (!this.TryMoveNext())
					{
						break;
					}
					this.SkipSpace();
				}
			}
			if (!this.HasValue() || this.Character() != '}' || !this.TryMoveNext())
			{
				obj = null;
				return this.MakeFailure("No closing } for object");
			}
			obj = new fsData(dictionary);
			return fsResult.Success;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003868 File Offset: 0x00001A68
		private fsResult RunParse(out fsData data)
		{
			this.SkipSpace();
			if (!this.HasValue())
			{
				data = null;
				return this.MakeFailure("Unexpected end of input");
			}
			char c = this.Character();
			if (c <= '[')
			{
				if (c <= 'I')
				{
					switch (c)
					{
					case '"':
					{
						string str;
						fsResult result = this.TryParseString(out str);
						if (result.Failed)
						{
							data = null;
							return result;
						}
						data = new fsData(str);
						return fsResult.Success;
					}
					case '#':
					case '$':
					case '%':
					case '&':
					case '\'':
					case '(':
					case ')':
					case '*':
					case ',':
					case '/':
						goto IL_11F;
					case '+':
					case '-':
					case '.':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						break;
					default:
						if (c != 'I')
						{
							goto IL_11F;
						}
						break;
					}
				}
				else if (c != 'N')
				{
					if (c != '[')
					{
						goto IL_11F;
					}
					return this.TryParseArray(out data);
				}
				return this.TryParseNumber(out data);
			}
			if (c <= 'n')
			{
				if (c == 'f')
				{
					return this.TryParseFalse(out data);
				}
				if (c == 'n')
				{
					return this.TryParseNull(out data);
				}
			}
			else
			{
				if (c == 't')
				{
					return this.TryParseTrue(out data);
				}
				if (c == '{')
				{
					return this.TryParseObject(out data);
				}
			}
			IL_11F:
			data = null;
			return this.MakeFailure("unable to parse; invalid token \"" + this.Character().ToString() + "\"");
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000039BA File Offset: 0x00001BBA
		public static fsResult Parse(string input, out fsData data)
		{
			if (string.IsNullOrEmpty(input))
			{
				data = null;
				return fsResult.Fail("No input");
			}
			return new fsJsonParser(input).RunParse(out data);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000039E0 File Offset: 0x00001BE0
		public static fsData Parse(string input)
		{
			fsData result;
			fsJsonParser.Parse(input, out result).AssertSuccess();
			return result;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000039FF File Offset: 0x00001BFF
		private fsJsonParser(string input)
		{
			this._input = input;
			this._start = 0;
		}

		// Token: 0x0400001E RID: 30
		private int _start;

		// Token: 0x0400001F RID: 31
		private string _input;

		// Token: 0x04000020 RID: 32
		private readonly StringBuilder _cachedStringBuilder = new StringBuilder(256);
	}
}
