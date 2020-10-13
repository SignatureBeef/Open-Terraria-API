using System;
using System.Globalization;

namespace FullSerializer.Internal
{
	// Token: 0x0200001D RID: 29
	public class fsDateConverter : fsConverter
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00005B3D File Offset: 0x00003D3D
		private string DateTimeFormatString
		{
			get
			{
				return this.Serializer.Config.CustomDateTimeFormatString ?? "o";
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00005B58 File Offset: 0x00003D58
		public override bool CanProcess(Type type)
		{
			return type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00005B90 File Offset: 0x00003D90
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			if (instance is DateTime)
			{
				serialized = new fsData(((DateTime)instance).ToString(this.DateTimeFormatString));
				return fsResult.Success;
			}
			if (instance is DateTimeOffset)
			{
				serialized = new fsData(((DateTimeOffset)instance).ToString("o"));
				return fsResult.Success;
			}
			if (instance is TimeSpan)
			{
				serialized = new fsData(((TimeSpan)instance).ToString());
				return fsResult.Success;
			}
			throw new InvalidOperationException("FullSerializer Internal Error -- Unexpected serialization type");
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00005C24 File Offset: 0x00003E24
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (!data.IsString)
			{
				return fsResult.Fail("Date deserialization requires a string, not " + data.Type);
			}
			if (storageType == typeof(DateTime))
			{
				DateTime dateTime;
				if (DateTime.TryParse(data.AsString, null, DateTimeStyles.RoundtripKind, out dateTime))
				{
					instance = dateTime;
					return fsResult.Success;
				}
				if (fsGlobalConfig.AllowInternalExceptions)
				{
					try
					{
						instance = Convert.ToDateTime(data.AsString);
						return fsResult.Success;
					}
					catch (Exception ex)
					{
						return fsResult.Fail(string.Concat(new object[]
						{
							"Unable to parse ",
							data.AsString,
							" into a DateTime; got exception ",
							ex
						}));
					}
				}
				return fsResult.Fail("Unable to parse " + data.AsString + " into a DateTime");
			}
			else if (storageType == typeof(DateTimeOffset))
			{
				DateTimeOffset dateTimeOffset;
				if (DateTimeOffset.TryParse(data.AsString, null, DateTimeStyles.RoundtripKind, out dateTimeOffset))
				{
					instance = dateTimeOffset;
					return fsResult.Success;
				}
				return fsResult.Fail("Unable to parse " + data.AsString + " into a DateTimeOffset");
			}
			else
			{
				if (!(storageType == typeof(TimeSpan)))
				{
					throw new InvalidOperationException("FullSerializer Internal Error -- Unexpected deserialization type");
				}
				TimeSpan timeSpan;
				if (TimeSpan.TryParse(data.AsString, out timeSpan))
				{
					instance = timeSpan;
					return fsResult.Success;
				}
				return fsResult.Fail("Unable to parse " + data.AsString + " into a TimeSpan");
			}
		}

		// Token: 0x04000046 RID: 70
		private const string DefaultDateTimeFormatString = "o";

		// Token: 0x04000047 RID: 71
		private const string DateTimeOffsetFormatString = "o";
	}
}
