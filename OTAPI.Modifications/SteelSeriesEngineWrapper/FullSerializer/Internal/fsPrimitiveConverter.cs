using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000025 RID: 37
	public class fsPrimitiveConverter : fsConverter
	{
		// Token: 0x060000FE RID: 254 RVA: 0x00006BC1 File Offset: 0x00004DC1
		public override bool CanProcess(Type type)
		{
			return type.Resolve().IsPrimitive || type == typeof(string) || type == typeof(decimal);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00006BF4 File Offset: 0x00004DF4
		private static bool UseBool(Type type)
		{
			return type == typeof(bool);
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00006C08 File Offset: 0x00004E08
		private static bool UseInt64(Type type)
		{
			return type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00006CA5 File Offset: 0x00004EA5
		private static bool UseDouble(Type type)
		{
			return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00006CDD File Offset: 0x00004EDD
		private static bool UseString(Type type)
		{
			return type == typeof(string) || type == typeof(char);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00006D04 File Offset: 0x00004F04
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			Type type = instance.GetType();
			if (this.Serializer.Config.Serialize64BitIntegerAsString && (type == typeof(long) || type == typeof(ulong)))
			{
				serialized = new fsData((string)Convert.ChangeType(instance, typeof(string)));
				return fsResult.Success;
			}
			if (fsPrimitiveConverter.UseBool(type))
			{
				serialized = new fsData((bool)instance);
				return fsResult.Success;
			}
			if (fsPrimitiveConverter.UseInt64(type))
			{
				serialized = new fsData((long)Convert.ChangeType(instance, typeof(long)));
				return fsResult.Success;
			}
			if (fsPrimitiveConverter.UseDouble(type))
			{
				if (instance.GetType() == typeof(float) && (float)instance != -3.4028235E+38f && (float)instance != 3.4028235E+38f && !float.IsInfinity((float)instance) && !float.IsNaN((float)instance))
				{
					serialized = new fsData((double)((decimal)((float)instance)));
					return fsResult.Success;
				}
				serialized = new fsData((double)Convert.ChangeType(instance, typeof(double)));
				return fsResult.Success;
			}
			else
			{
				if (fsPrimitiveConverter.UseString(type))
				{
					serialized = new fsData((string)Convert.ChangeType(instance, typeof(string)));
					return fsResult.Success;
				}
				serialized = null;
				return fsResult.Fail("Unhandled primitive type " + instance.GetType());
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00006E90 File Offset: 0x00005090
		public override fsResult TryDeserialize(fsData storage, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			if (fsPrimitiveConverter.UseBool(storageType))
			{
				fsResult fsResult2;
				fsResult = (fsResult2 = fsResult + base.CheckType(storage, fsDataType.Boolean));
				if (fsResult2.Succeeded)
				{
					instance = storage.AsBool;
				}
				return fsResult;
			}
			if (fsPrimitiveConverter.UseDouble(storageType) || fsPrimitiveConverter.UseInt64(storageType))
			{
				if (storage.IsDouble)
				{
					instance = Convert.ChangeType(storage.AsDouble, storageType);
				}
				else if (storage.IsInt64)
				{
					instance = Convert.ChangeType(storage.AsInt64, storageType);
				}
				else
				{
					if (!this.Serializer.Config.Serialize64BitIntegerAsString || !storage.IsString || (!(storageType == typeof(long)) && !(storageType == typeof(ulong))))
					{
						return fsResult.Fail(string.Concat(new object[]
						{
							base.GetType().Name,
							" expected number but got ",
							storage.Type,
							" in ",
							storage
						}));
					}
					instance = Convert.ChangeType(storage.AsString, storageType);
				}
				return fsResult.Success;
			}
			if (fsPrimitiveConverter.UseString(storageType))
			{
				fsResult fsResult2;
				fsResult = (fsResult2 = fsResult + base.CheckType(storage, fsDataType.String));
				if (fsResult2.Succeeded)
				{
					instance = storage.AsString;
				}
				return fsResult;
			}
			return fsResult.Fail(base.GetType().Name + ": Bad data; expected bool, number, string, but got " + storage);
		}
	}
}
