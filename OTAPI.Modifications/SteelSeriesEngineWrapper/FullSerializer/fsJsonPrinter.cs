using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FullSerializer
{
	// Token: 0x02000013 RID: 19
	public static class fsJsonPrinter
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00003A28 File Offset: 0x00001C28
		private static void InsertSpacing(TextWriter stream, int count)
		{
			for (int i = 0; i < count; i++)
			{
				stream.Write("    ");
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003A4C File Offset: 0x00001C4C
		private static string EscapeString(string str)
		{
			bool flag = false;
			int i = 0;
			while (i < str.Length)
			{
				char c = str[i];
				int num = Convert.ToInt32(c);
				if (num < 0 || num > 127)
				{
					flag = true;
					break;
				}
				if (c <= '\r')
				{
					if (c == '\0')
					{
						goto IL_5D;
					}
					switch (c)
					{
					case '\a':
					case '\b':
					case '\t':
					case '\n':
					case '\f':
					case '\r':
						goto IL_5D;
					}
				}
				else if (c == '"' || c == '\\')
				{
					goto IL_5D;
				}
				IL_5F:
				if (!flag)
				{
					i++;
					continue;
				}
				break;
				IL_5D:
				flag = true;
				goto IL_5F;
			}
			if (!flag)
			{
				return str;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c2 in str)
			{
				int num2 = Convert.ToInt32(c2);
				if (num2 < 0 || num2 > 127)
				{
					stringBuilder.Append(string.Format("\\u{0:x4} ", num2).Trim());
				}
				else
				{
					if (c2 <= '\r')
					{
						if (c2 == '\0')
						{
							stringBuilder.Append("\\0");
							goto IL_18E;
						}
						switch (c2)
						{
						case '\a':
							stringBuilder.Append("\\a");
							goto IL_18E;
						case '\b':
							stringBuilder.Append("\\b");
							goto IL_18E;
						case '\t':
							stringBuilder.Append("\\t");
							goto IL_18E;
						case '\n':
							stringBuilder.Append("\\n");
							goto IL_18E;
						case '\f':
							stringBuilder.Append("\\f");
							goto IL_18E;
						case '\r':
							stringBuilder.Append("\\r");
							goto IL_18E;
						}
					}
					else
					{
						if (c2 == '"')
						{
							stringBuilder.Append("\\\"");
							goto IL_18E;
						}
						if (c2 == '\\')
						{
							stringBuilder.Append("\\\\");
							goto IL_18E;
						}
					}
					stringBuilder.Append(c2);
				}
				IL_18E:;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003C00 File Offset: 0x00001E00
		private static void BuildCompressedString(fsData data, TextWriter stream)
		{
			switch (data.Type)
			{
			case fsDataType.Array:
			{
				stream.Write('[');
				bool flag = false;
				foreach (fsData data2 in data.AsList)
				{
					if (flag)
					{
						stream.Write(',');
					}
					flag = true;
					fsJsonPrinter.BuildCompressedString(data2, stream);
				}
				stream.Write(']');
				return;
			}
			case fsDataType.Object:
			{
				stream.Write('{');
				bool flag2 = false;
				foreach (KeyValuePair<string, fsData> keyValuePair in data.AsDictionary)
				{
					if (flag2)
					{
						stream.Write(',');
					}
					flag2 = true;
					stream.Write('"');
					stream.Write(keyValuePair.Key);
					stream.Write('"');
					stream.Write(":");
					fsJsonPrinter.BuildCompressedString(keyValuePair.Value, stream);
				}
				stream.Write('}');
				return;
			}
			case fsDataType.Double:
				stream.Write(fsJsonPrinter.ConvertDoubleToString(data.AsDouble));
				return;
			case fsDataType.Int64:
				stream.Write(data.AsInt64);
				return;
			case fsDataType.Boolean:
				if (data.AsBool)
				{
					stream.Write("true");
					return;
				}
				stream.Write("false");
				return;
			case fsDataType.String:
				stream.Write('"');
				stream.Write(fsJsonPrinter.EscapeString(data.AsString));
				stream.Write('"');
				return;
			case fsDataType.Null:
				stream.Write("null");
				return;
			default:
				return;
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003D9C File Offset: 0x00001F9C
		private static void BuildPrettyString(fsData data, TextWriter stream, int depth)
		{
			switch (data.Type)
			{
			case fsDataType.Array:
			{
				if (data.AsList.Count == 0)
				{
					stream.Write("[]");
					return;
				}
				bool flag = false;
				stream.Write('[');
				stream.WriteLine();
				foreach (fsData data2 in data.AsList)
				{
					if (flag)
					{
						stream.Write(',');
						stream.WriteLine();
					}
					flag = true;
					fsJsonPrinter.InsertSpacing(stream, depth + 1);
					fsJsonPrinter.BuildPrettyString(data2, stream, depth + 1);
				}
				stream.WriteLine();
				fsJsonPrinter.InsertSpacing(stream, depth);
				stream.Write(']');
				return;
			}
			case fsDataType.Object:
			{
				stream.Write('{');
				stream.WriteLine();
				bool flag2 = false;
				foreach (KeyValuePair<string, fsData> keyValuePair in data.AsDictionary)
				{
					if (flag2)
					{
						stream.Write(',');
						stream.WriteLine();
					}
					flag2 = true;
					fsJsonPrinter.InsertSpacing(stream, depth + 1);
					stream.Write('"');
					stream.Write(keyValuePair.Key);
					stream.Write('"');
					stream.Write(": ");
					fsJsonPrinter.BuildPrettyString(keyValuePair.Value, stream, depth + 1);
				}
				stream.WriteLine();
				fsJsonPrinter.InsertSpacing(stream, depth);
				stream.Write('}');
				return;
			}
			case fsDataType.Double:
				stream.Write(fsJsonPrinter.ConvertDoubleToString(data.AsDouble));
				return;
			case fsDataType.Int64:
				stream.Write(data.AsInt64);
				return;
			case fsDataType.Boolean:
				if (data.AsBool)
				{
					stream.Write("true");
					return;
				}
				stream.Write("false");
				return;
			case fsDataType.String:
				stream.Write('"');
				stream.Write(fsJsonPrinter.EscapeString(data.AsString));
				stream.Write('"');
				return;
			case fsDataType.Null:
				stream.Write("null");
				return;
			default:
				return;
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003F9C File Offset: 0x0000219C
		public static void PrettyJson(fsData data, TextWriter outputStream)
		{
			fsJsonPrinter.BuildPrettyString(data, outputStream, 0);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003FA8 File Offset: 0x000021A8
		public static string PrettyJson(fsData data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string result;
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				fsJsonPrinter.BuildPrettyString(data, stringWriter, 0);
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003FF0 File Offset: 0x000021F0
		public static void CompressedJson(fsData data, StreamWriter outputStream)
		{
			fsJsonPrinter.BuildCompressedString(data, outputStream);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003FFC File Offset: 0x000021FC
		public static string CompressedJson(fsData data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string result;
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				fsJsonPrinter.BuildCompressedString(data, stringWriter);
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004044 File Offset: 0x00002244
		private static string ConvertDoubleToString(double d)
		{
			if (double.IsInfinity(d) || double.IsNaN(d))
			{
				return d.ToString(CultureInfo.InvariantCulture);
			}
			string text = d.ToString(CultureInfo.InvariantCulture);
			if (!text.Contains("."))
			{
				text += ".0";
			}
			return text;
		}
	}
}
