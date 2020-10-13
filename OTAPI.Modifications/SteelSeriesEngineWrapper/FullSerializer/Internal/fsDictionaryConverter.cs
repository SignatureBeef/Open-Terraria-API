using System;
using System.Collections;
using System.Collections.Generic;

namespace FullSerializer.Internal
{
	// Token: 0x0200001E RID: 30
	public class fsDictionaryConverter : fsConverter
	{
		// Token: 0x060000CC RID: 204 RVA: 0x00005DB8 File Offset: 0x00003FB8
		public override bool CanProcess(Type type)
		{
			return typeof(IDictionary).IsAssignableFrom(type);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00005B1D File Offset: 0x00003D1D
		public override object CreateInstance(fsData data, Type storageType)
		{
			return fsMetaType.Get(this.Serializer.Config, storageType).CreateInstance();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00005DCC File Offset: 0x00003FCC
		public override fsResult TryDeserialize(fsData data, ref object instance_, Type storageType)
		{
			IDictionary dictionary = (IDictionary)instance_;
			fsResult fsResult = fsResult.Success;
			Type storageType2;
			Type storageType3;
			fsDictionaryConverter.GetKeyValueTypes(dictionary.GetType(), out storageType2, out storageType3);
			if (!data.IsList)
			{
				if (data.IsDictionary)
				{
					using (Dictionary<string, fsData>.Enumerator enumerator = data.AsDictionary.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, fsData> keyValuePair = enumerator.Current;
							if (!fsSerializer.IsReservedKeyword(keyValuePair.Key))
							{
								fsData data2 = new fsData(keyValuePair.Key);
								fsData value = keyValuePair.Value;
								object key = null;
								object value2 = null;
								fsResult fsResult2;
								fsResult = (fsResult2 = fsResult + this.Serializer.TryDeserialize(data2, storageType2, ref key));
								if (fsResult2.Failed)
								{
									return fsResult;
								}
								fsResult fsResult3;
								fsResult = (fsResult3 = fsResult + this.Serializer.TryDeserialize(value, storageType3, ref value2));
								if (fsResult3.Failed)
								{
									return fsResult;
								}
								this.AddItemToDictionary(dictionary, key, value2);
							}
						}
						return fsResult;
					}
				}
				return base.FailExpectedType(data, new fsDataType[]
				{
					fsDataType.Array,
					fsDataType.Object
				});
			}
			List<fsData> asList = data.AsList;
			for (int i = 0; i < asList.Count; i++)
			{
				fsData data3 = asList[i];
				fsResult fsResult2;
				fsResult = (fsResult2 = fsResult + base.CheckType(data3, fsDataType.Object));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				fsData data4;
				fsResult = (fsResult2 = fsResult + base.CheckKey(data3, "Key", out data4));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				fsData data5;
				fsResult = (fsResult2 = fsResult + base.CheckKey(data3, "Value", out data5));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				object key2 = null;
				object value3 = null;
				fsResult = (fsResult2 = fsResult + this.Serializer.TryDeserialize(data4, storageType2, ref key2));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				fsResult = (fsResult2 = fsResult + this.Serializer.TryDeserialize(data5, storageType3, ref value3));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				this.AddItemToDictionary(dictionary, key2, value3);
			}
			return fsResult;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00005FE8 File Offset: 0x000041E8
		public override fsResult TrySerialize(object instance_, out fsData serialized, Type storageType)
		{
			serialized = fsData.Null;
			fsResult fsResult = fsResult.Success;
			IDictionary dictionary = (IDictionary)instance_;
			Type storageType2;
			Type storageType3;
			fsDictionaryConverter.GetKeyValueTypes(dictionary.GetType(), out storageType2, out storageType3);
			IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
			bool flag = true;
			List<fsData> list = new List<fsData>(dictionary.Count);
			List<fsData> list2 = new List<fsData>(dictionary.Count);
			while (enumerator.MoveNext())
			{
				fsData fsData;
				fsResult fsResult2;
				fsResult = (fsResult2 = fsResult + this.Serializer.TrySerialize(storageType2, enumerator.Key, out fsData));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				fsData item;
				fsResult = (fsResult2 = fsResult + this.Serializer.TrySerialize(storageType3, enumerator.Value, out item));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				list.Add(fsData);
				list2.Add(item);
				flag &= fsData.IsString;
			}
			if (flag)
			{
				serialized = fsData.CreateDictionary();
				Dictionary<string, fsData> asDictionary = serialized.AsDictionary;
				for (int i = 0; i < list.Count; i++)
				{
					fsData fsData2 = list[i];
					fsData value = list2[i];
					asDictionary[fsData2.AsString] = value;
				}
			}
			else
			{
				serialized = fsData.CreateList(list.Count);
				List<fsData> asList = serialized.AsList;
				for (int j = 0; j < list.Count; j++)
				{
					fsData value2 = list[j];
					fsData value3 = list2[j];
					Dictionary<string, fsData> dictionary2 = new Dictionary<string, fsData>();
					dictionary2["Key"] = value2;
					dictionary2["Value"] = value3;
					asList.Add(new fsData(dictionary2));
				}
			}
			return fsResult;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000617C File Offset: 0x0000437C
		private fsResult AddItemToDictionary(IDictionary dictionary, object key, object value)
		{
			if (key != null && value != null)
			{
				dictionary[key] = value;
				return fsResult.Success;
			}
			Type @interface = fsReflectionUtility.GetInterface(dictionary.GetType(), typeof(ICollection<>));
			if (@interface == null)
			{
				return fsResult.Warn(dictionary.GetType() + " does not extend ICollection");
			}
			object obj = Activator.CreateInstance(@interface.GetGenericArguments()[0], new object[]
			{
				key,
				value
			});
			@interface.GetFlattenedMethod("Add").Invoke(dictionary, new object[]
			{
				obj
			});
			return fsResult.Success;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00006210 File Offset: 0x00004410
		private static void GetKeyValueTypes(Type dictionaryType, out Type keyStorageType, out Type valueStorageType)
		{
			Type @interface = fsReflectionUtility.GetInterface(dictionaryType, typeof(IDictionary<, >));
			if (@interface != null)
			{
				Type[] genericArguments = @interface.GetGenericArguments();
				keyStorageType = genericArguments[0];
				valueStorageType = genericArguments[1];
				return;
			}
			keyStorageType = typeof(object);
			valueStorageType = typeof(object);
		}
	}
}
