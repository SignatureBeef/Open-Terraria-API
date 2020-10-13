using System;
using System.Collections.Generic;
using System.Text;

namespace FullSerializer.Internal
{
	// Token: 0x0200001F RID: 31
	public class fsEnumConverter : fsConverter
	{
		// Token: 0x060000D3 RID: 211 RVA: 0x00006261 File Offset: 0x00004461
		public override bool CanProcess(Type type)
		{
			return type.Resolve().IsEnum;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000626E File Offset: 0x0000446E
		public override object CreateInstance(fsData data, Type storageType)
		{
			return Enum.ToObject(storageType, 0);
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000627C File Offset: 0x0000447C
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			if (this.Serializer.Config.SerializeEnumsAsInteger)
			{
				serialized = new fsData(Convert.ToInt64(instance));
			}
			else if (fsPortableReflection.GetAttribute<FlagsAttribute>(storageType) != null)
			{
				long num = Convert.ToInt64(instance);
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				foreach (object obj in Enum.GetValues(storageType))
				{
					long num2 = Convert.ToInt64(obj);
					if ((num & num2) != 0L)
					{
						if (!flag)
						{
							stringBuilder.Append(",");
						}
						flag = false;
						stringBuilder.Append(obj.ToString());
					}
				}
				serialized = new fsData(stringBuilder.ToString());
			}
			else
			{
				serialized = new fsData(Enum.GetName(storageType, instance));
			}
			return fsResult.Success;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00006360 File Offset: 0x00004560
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			if (data.IsString)
			{
				string[] array = data.AsString.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				long num = 0L;
				foreach (string text in array)
				{
					if (!fsEnumConverter.ArrayContains<string>(Enum.GetNames(storageType), text))
					{
						return fsResult.Fail(string.Concat(new object[]
						{
							"Cannot find enum name ",
							text,
							" on type ",
							storageType
						}));
					}
					long num2 = (long)Convert.ChangeType(Enum.Parse(storageType, text), typeof(long));
					num |= num2;
				}
				instance = Enum.ToObject(storageType, num);
				return fsResult.Success;
			}
			if (data.IsInt64)
			{
				int num3 = (int)data.AsInt64;
				instance = Enum.ToObject(storageType, num3);
				return fsResult.Success;
			}
			return fsResult.Fail("EnumConverter encountered an unknown JSON data type");
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00006444 File Offset: 0x00004644
		private static bool ArrayContains<T>(T[] values, T value)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (EqualityComparer<T>.Default.Equals(values[i], value))
				{
					return true;
				}
			}
			return false;
		}
	}
}
