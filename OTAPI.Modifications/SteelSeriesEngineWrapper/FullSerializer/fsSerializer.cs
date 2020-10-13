using System;
using System.Collections.Generic;
using FullSerializer.Internal;

namespace FullSerializer
{
	// Token: 0x02000019 RID: 25
	public class fsSerializer
	{
		// Token: 0x0600008D RID: 141 RVA: 0x0000434B File Offset: 0x0000254B
		public static bool IsReservedKeyword(string key)
		{
			return fsSerializer._reservedKeywords.Contains(key);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00004358 File Offset: 0x00002558
		private static bool IsObjectReference(fsData data)
		{
			return data.IsDictionary && data.AsDictionary.ContainsKey("$ref");
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004374 File Offset: 0x00002574
		private static bool IsObjectDefinition(fsData data)
		{
			return data.IsDictionary && data.AsDictionary.ContainsKey("$id");
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004390 File Offset: 0x00002590
		private static bool IsVersioned(fsData data)
		{
			return data.IsDictionary && data.AsDictionary.ContainsKey("$version");
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000043AC File Offset: 0x000025AC
		private static bool IsTypeSpecified(fsData data)
		{
			return data.IsDictionary && data.AsDictionary.ContainsKey("$type");
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000043C8 File Offset: 0x000025C8
		private static bool IsWrappedData(fsData data)
		{
			return data.IsDictionary && data.AsDictionary.ContainsKey("$content");
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000043E4 File Offset: 0x000025E4
		public static void StripDeserializationMetadata(ref fsData data)
		{
			if (data.IsDictionary && data.AsDictionary.ContainsKey("$content"))
			{
				data = data.AsDictionary["$content"];
			}
			if (data.IsDictionary)
			{
				Dictionary<string, fsData> asDictionary = data.AsDictionary;
				asDictionary.Remove("$ref");
				asDictionary.Remove("$id");
				asDictionary.Remove("$type");
				asDictionary.Remove("$version");
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004460 File Offset: 0x00002660
		private static void ConvertLegacyData(ref fsData data)
		{
			if (!data.IsDictionary)
			{
				return;
			}
			Dictionary<string, fsData> asDictionary = data.AsDictionary;
			if (asDictionary.Count > 2)
			{
				return;
			}
			string key = "ReferenceId";
			string key2 = "SourceId";
			string key3 = "Data";
			string key4 = "Type";
			string key5 = "Data";
			if (asDictionary.Count == 2 && asDictionary.ContainsKey(key4) && asDictionary.ContainsKey(key5))
			{
				data = asDictionary[key5];
				fsSerializer.EnsureDictionary(data);
				fsSerializer.ConvertLegacyData(ref data);
				data.AsDictionary["$type"] = asDictionary[key4];
				return;
			}
			if (asDictionary.Count == 2 && asDictionary.ContainsKey(key2) && asDictionary.ContainsKey(key3))
			{
				data = asDictionary[key3];
				fsSerializer.EnsureDictionary(data);
				fsSerializer.ConvertLegacyData(ref data);
				data.AsDictionary["$id"] = asDictionary[key2];
				return;
			}
			if (asDictionary.Count == 1 && asDictionary.ContainsKey(key))
			{
				data = fsData.CreateDictionary();
				data.AsDictionary["$ref"] = asDictionary[key];
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004574 File Offset: 0x00002774
		private static void Invoke_OnBeforeSerialize(List<fsObjectProcessor> processors, Type storageType, object instance)
		{
			for (int i = 0; i < processors.Count; i++)
			{
				processors[i].OnBeforeSerialize(storageType, instance);
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000045A0 File Offset: 0x000027A0
		private static void Invoke_OnAfterSerialize(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data)
		{
			for (int i = processors.Count - 1; i >= 0; i--)
			{
				processors[i].OnAfterSerialize(storageType, instance, ref data);
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000045D0 File Offset: 0x000027D0
		private static void Invoke_OnBeforeDeserialize(List<fsObjectProcessor> processors, Type storageType, ref fsData data)
		{
			for (int i = 0; i < processors.Count; i++)
			{
				processors[i].OnBeforeDeserialize(storageType, ref data);
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000045FC File Offset: 0x000027FC
		private static void Invoke_OnBeforeDeserializeAfterInstanceCreation(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data)
		{
			for (int i = 0; i < processors.Count; i++)
			{
				processors[i].OnBeforeDeserializeAfterInstanceCreation(storageType, instance, ref data);
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000462C File Offset: 0x0000282C
		private static void Invoke_OnAfterDeserialize(List<fsObjectProcessor> processors, Type storageType, object instance)
		{
			for (int i = processors.Count - 1; i >= 0; i--)
			{
				processors[i].OnAfterDeserialize(storageType, instance);
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000465C File Offset: 0x0000285C
		private static void EnsureDictionary(fsData data)
		{
			if (!data.IsDictionary)
			{
				fsData value = data.Clone();
				data.BecomeDictionary();
				data.AsDictionary["$content"] = value;
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00004690 File Offset: 0x00002890
		public fsSerializer()
		{
			this._cachedConverterTypeInstances = new Dictionary<Type, fsBaseConverter>();
			this._cachedConverters = new Dictionary<Type, fsBaseConverter>();
			this._cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
			this._references = new fsCyclicReferenceManager();
			this._lazyReferenceWriter = new fsSerializer.fsLazyCycleDefinitionWriter();
			this._availableConverters = new List<fsConverter>
			{
				new fsNullableConverter
				{
					Serializer = this
				},
				new fsGuidConverter
				{
					Serializer = this
				},
				new fsTypeConverter
				{
					Serializer = this
				},
				new fsDateConverter
				{
					Serializer = this
				},
				new fsEnumConverter
				{
					Serializer = this
				},
				new fsPrimitiveConverter
				{
					Serializer = this
				},
				new fsArrayConverter
				{
					Serializer = this
				},
				new fsDictionaryConverter
				{
					Serializer = this
				},
				new fsIEnumerableConverter
				{
					Serializer = this
				},
				new fsKeyValuePairConverter
				{
					Serializer = this
				},
				new fsWeakReferenceConverter
				{
					Serializer = this
				},
				new fsReflectedConverter
				{
					Serializer = this
				}
			};
			this._availableDirectConverters = new Dictionary<Type, fsDirectConverter>();
			this._processors = new List<fsObjectProcessor>
			{
				new fsSerializationCallbackProcessor()
			};
			this.Context = new fsContext();
			this.Config = new fsConfig();
			foreach (Type type in fsConverterRegistrar.Converters)
			{
				this.AddConverter((fsBaseConverter)Activator.CreateInstance(type));
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004844 File Offset: 0x00002A44
		public void AddProcessor(fsObjectProcessor processor)
		{
			this._processors.Add(processor);
			this._cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004860 File Offset: 0x00002A60
		public void RemoveProcessor<TProcessor>()
		{
			int i = 0;
			while (i < this._processors.Count)
			{
				if (this._processors[i] is TProcessor)
				{
					this._processors.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			this._cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000048B0 File Offset: 0x00002AB0
		private List<fsObjectProcessor> GetProcessors(Type type)
		{
			fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
			List<fsObjectProcessor> list;
			if (attribute != null && attribute.Processor != null)
			{
				fsObjectProcessor item = (fsObjectProcessor)Activator.CreateInstance(attribute.Processor);
				list = new List<fsObjectProcessor>();
				list.Add(item);
				this._cachedProcessors[type] = list;
			}
			else if (!this._cachedProcessors.TryGetValue(type, out list))
			{
				list = new List<fsObjectProcessor>();
				for (int i = 0; i < this._processors.Count; i++)
				{
					fsObjectProcessor fsObjectProcessor = this._processors[i];
					if (fsObjectProcessor.CanProcess(type))
					{
						list.Add(fsObjectProcessor);
					}
				}
				this._cachedProcessors[type] = list;
			}
			return list;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x0000495C File Offset: 0x00002B5C
		public void AddConverter(fsBaseConverter converter)
		{
			if (converter.Serializer != null)
			{
				throw new InvalidOperationException("Cannot add a single converter instance to multiple fsConverters -- please construct a new instance for " + converter);
			}
			if (converter is fsDirectConverter)
			{
				fsDirectConverter fsDirectConverter = (fsDirectConverter)converter;
				this._availableDirectConverters[fsDirectConverter.ModelType] = fsDirectConverter;
			}
			else
			{
				if (!(converter is fsConverter))
				{
					throw new InvalidOperationException("Unable to add converter " + converter + "; the type association strategy is unknown. Please use either fsDirectConverter or fsConverter as your base type.");
				}
				this._availableConverters.Insert(0, (fsConverter)converter);
			}
			converter.Serializer = this;
			this._cachedConverters = new Dictionary<Type, fsBaseConverter>();
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000049EC File Offset: 0x00002BEC
		private fsBaseConverter GetConverter(Type type, Type overrideConverterType)
		{
			if (overrideConverterType != null)
			{
				fsBaseConverter fsBaseConverter;
				if (!this._cachedConverterTypeInstances.TryGetValue(overrideConverterType, out fsBaseConverter))
				{
					fsBaseConverter = (fsBaseConverter)Activator.CreateInstance(overrideConverterType);
					fsBaseConverter.Serializer = this;
					this._cachedConverterTypeInstances[overrideConverterType] = fsBaseConverter;
				}
				return fsBaseConverter;
			}
			fsBaseConverter fsBaseConverter2;
			if (this._cachedConverters.TryGetValue(type, out fsBaseConverter2))
			{
				return fsBaseConverter2;
			}
			fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(type);
			if (attribute != null && attribute.Converter != null)
			{
				fsBaseConverter2 = (fsBaseConverter)Activator.CreateInstance(attribute.Converter);
				fsBaseConverter2.Serializer = this;
				return this._cachedConverters[type] = fsBaseConverter2;
			}
			fsForwardAttribute attribute2 = fsPortableReflection.GetAttribute<fsForwardAttribute>(type);
			if (attribute2 != null)
			{
				fsBaseConverter2 = new fsForwardConverter(attribute2);
				fsBaseConverter2.Serializer = this;
				return this._cachedConverters[type] = fsBaseConverter2;
			}
			if (!this._cachedConverters.TryGetValue(type, out fsBaseConverter2))
			{
				if (this._availableDirectConverters.ContainsKey(type))
				{
					fsBaseConverter2 = this._availableDirectConverters[type];
					return this._cachedConverters[type] = fsBaseConverter2;
				}
				for (int i = 0; i < this._availableConverters.Count; i++)
				{
					if (this._availableConverters[i].CanProcess(type))
					{
						fsBaseConverter2 = this._availableConverters[i];
						return this._cachedConverters[type] = fsBaseConverter2;
					}
				}
			}
			throw new InvalidOperationException("Internal error -- could not find a converter for " + type);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004B4D File Offset: 0x00002D4D
		public fsResult TrySerialize<T>(T instance, out fsData data)
		{
			return this.TrySerialize(typeof(T), instance, out data);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004B68 File Offset: 0x00002D68
		public fsResult TryDeserialize<T>(fsData data, ref T instance)
		{
			object obj = instance;
			fsResult result = this.TryDeserialize(data, typeof(T), ref obj);
			if (result.Succeeded)
			{
				instance = (T)((object)obj);
			}
			return result;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00004BAB File Offset: 0x00002DAB
		public fsResult TrySerialize(Type storageType, object instance, out fsData data)
		{
			return this.TrySerialize(storageType, null, instance, out data);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00004BB8 File Offset: 0x00002DB8
		public fsResult TrySerialize(Type storageType, Type overrideConverterType, object instance, out fsData data)
		{
			List<fsObjectProcessor> processors = this.GetProcessors((instance == null) ? storageType : instance.GetType());
			fsSerializer.Invoke_OnBeforeSerialize(processors, storageType, instance);
			if (instance == null)
			{
				data = new fsData();
				fsSerializer.Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
				return fsResult.Success;
			}
			fsResult result = this.InternalSerialize_1_ProcessCycles(storageType, overrideConverterType, instance, out data);
			fsSerializer.Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
			return result;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00004C10 File Offset: 0x00002E10
		private fsResult InternalSerialize_1_ProcessCycles(Type storageType, Type overrideConverterType, object instance, out fsData data)
		{
			fsResult result;
			try
			{
				this._references.Enter();
				if (!this.GetConverter(instance.GetType(), overrideConverterType).RequestCycleSupport(instance.GetType()))
				{
					result = this.InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
				}
				else if (this._references.IsReference(instance))
				{
					data = fsData.CreateDictionary();
					this._lazyReferenceWriter.WriteReference(this._references.GetReferenceId(instance), data.AsDictionary);
					result = fsResult.Success;
				}
				else
				{
					this._references.MarkSerialized(instance);
					fsResult fsResult = this.InternalSerialize_2_Inheritance(storageType, overrideConverterType, instance, out data);
					if (fsResult.Failed)
					{
						result = fsResult;
					}
					else
					{
						this._lazyReferenceWriter.WriteDefinition(this._references.GetReferenceId(instance), data);
						result = fsResult;
					}
				}
			}
			finally
			{
				if (this._references.Exit())
				{
					this._lazyReferenceWriter.Clear();
				}
			}
			return result;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00004D00 File Offset: 0x00002F00
		private fsResult InternalSerialize_2_Inheritance(Type storageType, Type overrideConverterType, object instance, out fsData data)
		{
			fsResult result = this.InternalSerialize_3_ProcessVersioning(overrideConverterType, instance, out data);
			if (result.Failed)
			{
				return result;
			}
			if (storageType != instance.GetType() && this.GetConverter(storageType, overrideConverterType).RequestInheritanceSupport(storageType))
			{
				fsSerializer.EnsureDictionary(data);
				data.AsDictionary["$type"] = new fsData(instance.GetType().FullName);
			}
			return result;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00004D6C File Offset: 0x00002F6C
		private fsResult InternalSerialize_3_ProcessVersioning(Type overrideConverterType, object instance, out fsData data)
		{
			fsOption<fsVersionedType> versionedType = fsVersionManager.GetVersionedType(instance.GetType());
			if (!versionedType.HasValue)
			{
				return this.InternalSerialize_4_Converter(overrideConverterType, instance, out data);
			}
			fsVersionedType value = versionedType.Value;
			fsResult result = this.InternalSerialize_4_Converter(overrideConverterType, instance, out data);
			if (result.Failed)
			{
				return result;
			}
			fsSerializer.EnsureDictionary(data);
			data.AsDictionary["$version"] = new fsData(value.VersionString);
			return result;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00004DDC File Offset: 0x00002FDC
		private fsResult InternalSerialize_4_Converter(Type overrideConverterType, object instance, out fsData data)
		{
			Type type = instance.GetType();
			return this.GetConverter(type, overrideConverterType).TrySerialize(instance, out data, type);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00004E00 File Offset: 0x00003000
		public fsResult TryDeserialize(fsData data, Type storageType, ref object result)
		{
			return this.TryDeserialize(data, storageType, null, ref result);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00004E0C File Offset: 0x0000300C
		public fsResult TryDeserialize(fsData data, Type storageType, Type overrideConverterType, ref object result)
		{
			if (data.IsNull)
			{
				result = null;
				List<fsObjectProcessor> processors = this.GetProcessors(storageType);
				fsSerializer.Invoke_OnBeforeDeserialize(processors, storageType, ref data);
				fsSerializer.Invoke_OnAfterDeserialize(processors, storageType, null);
				return fsResult.Success;
			}
			fsSerializer.ConvertLegacyData(ref data);
			fsResult result2;
			try
			{
				this._references.Enter();
				List<fsObjectProcessor> processors2;
				fsResult fsResult = this.InternalDeserialize_1_CycleReference(overrideConverterType, data, storageType, ref result, out processors2);
				if (fsResult.Succeeded)
				{
					fsSerializer.Invoke_OnAfterDeserialize(processors2, storageType, result);
				}
				result2 = fsResult;
			}
			finally
			{
				this._references.Exit();
			}
			return result2;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00004E98 File Offset: 0x00003098
		private fsResult InternalDeserialize_1_CycleReference(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
		{
			if (fsSerializer.IsObjectReference(data))
			{
				int id = int.Parse(data.AsDictionary["$ref"].AsString);
				result = this._references.GetReferenceObject(id);
				processors = this.GetProcessors(result.GetType());
				return fsResult.Success;
			}
			return this.InternalDeserialize_2_Version(overrideConverterType, data, storageType, ref result, out processors);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004EFC File Offset: 0x000030FC
		private fsResult InternalDeserialize_2_Version(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
		{
			if (fsSerializer.IsVersioned(data))
			{
				string asString = data.AsDictionary["$version"].AsString;
				fsOption<fsVersionedType> versionedType = fsVersionManager.GetVersionedType(storageType);
				if (versionedType.HasValue && versionedType.Value.VersionString != asString)
				{
					fsResult fsResult = fsResult.Success;
					List<fsVersionedType> list;
					fsResult += fsVersionManager.GetVersionImportPath(asString, versionedType.Value, out list);
					if (fsResult.Failed)
					{
						processors = this.GetProcessors(storageType);
						return fsResult;
					}
					fsResult += this.InternalDeserialize_3_Inheritance(overrideConverterType, data, list[0].ModelType, ref result, out processors);
					if (fsResult.Failed)
					{
						return fsResult;
					}
					for (int i = 1; i < list.Count; i++)
					{
						result = list[i].Migrate(result);
					}
					if (fsSerializer.IsObjectDefinition(data))
					{
						int id = int.Parse(data.AsDictionary["$id"].AsString);
						this._references.AddReferenceWithId(id, result);
					}
					processors = this.GetProcessors(fsResult.GetType());
					return fsResult;
				}
			}
			return this.InternalDeserialize_3_Inheritance(overrideConverterType, data, storageType, ref result, out processors);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00005034 File Offset: 0x00003234
		private fsResult InternalDeserialize_3_Inheritance(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors)
		{
			fsResult success = fsResult.Success;
			Type type = storageType;
			if (fsSerializer.IsTypeSpecified(data))
			{
				fsData fsData = data.AsDictionary["$type"];
				if (!fsData.IsString)
				{
					success.AddMessage("$type value must be a string (in " + data + ")");
				}
				else
				{
					string asString = fsData.AsString;
					Type type2 = fsTypeCache.GetType(asString);
					if (type2 == null)
					{
						success.AddMessage("Unable to locate specified type \"" + asString + "\"");
					}
					else if (!storageType.IsAssignableFrom(type2))
					{
						success.AddMessage(string.Concat(new object[]
						{
							"Ignoring type specifier; a field/property of type ",
							storageType,
							" cannot hold an instance of ",
							type2
						}));
					}
					else
					{
						type = type2;
					}
				}
			}
			processors = this.GetProcessors(type);
			fsSerializer.Invoke_OnBeforeDeserialize(processors, storageType, ref data);
			if (result == null || result.GetType() != type)
			{
				result = this.GetConverter(type, overrideConverterType).CreateInstance(data, type);
			}
			fsSerializer.Invoke_OnBeforeDeserializeAfterInstanceCreation(processors, storageType, result, ref data);
			return success + this.InternalDeserialize_4_Cycles(overrideConverterType, data, type, ref result);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00005150 File Offset: 0x00003350
		private fsResult InternalDeserialize_4_Cycles(Type overrideConverterType, fsData data, Type resultType, ref object result)
		{
			if (fsSerializer.IsObjectDefinition(data))
			{
				int id = int.Parse(data.AsDictionary["$id"].AsString);
				this._references.AddReferenceWithId(id, result);
			}
			return this.InternalDeserialize_5_Converter(overrideConverterType, data, resultType, ref result);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000519A File Offset: 0x0000339A
		private fsResult InternalDeserialize_5_Converter(Type overrideConverterType, fsData data, Type resultType, ref object result)
		{
			if (fsSerializer.IsWrappedData(data))
			{
				data = data.AsDictionary["$content"];
			}
			return this.GetConverter(resultType, overrideConverterType).TryDeserialize(data, ref result, resultType);
		}

		// Token: 0x04000030 RID: 48
		private static HashSet<string> _reservedKeywords = new HashSet<string>
		{
			"$ref",
			"$id",
			"$type",
			"$version",
			"$content"
		};

		// Token: 0x04000031 RID: 49
		private const string Key_ObjectReference = "$ref";

		// Token: 0x04000032 RID: 50
		private const string Key_ObjectDefinition = "$id";

		// Token: 0x04000033 RID: 51
		private const string Key_InstanceType = "$type";

		// Token: 0x04000034 RID: 52
		private const string Key_Version = "$version";

		// Token: 0x04000035 RID: 53
		private const string Key_Content = "$content";

		// Token: 0x04000036 RID: 54
		private Dictionary<Type, fsBaseConverter> _cachedConverterTypeInstances;

		// Token: 0x04000037 RID: 55
		private Dictionary<Type, fsBaseConverter> _cachedConverters;

		// Token: 0x04000038 RID: 56
		private Dictionary<Type, List<fsObjectProcessor>> _cachedProcessors;

		// Token: 0x04000039 RID: 57
		private readonly List<fsConverter> _availableConverters;

		// Token: 0x0400003A RID: 58
		private readonly Dictionary<Type, fsDirectConverter> _availableDirectConverters;

		// Token: 0x0400003B RID: 59
		private readonly List<fsObjectProcessor> _processors;

		// Token: 0x0400003C RID: 60
		private readonly fsCyclicReferenceManager _references;

		// Token: 0x0400003D RID: 61
		private readonly fsSerializer.fsLazyCycleDefinitionWriter _lazyReferenceWriter;

		// Token: 0x0400003E RID: 62
		public fsContext Context;

		// Token: 0x0400003F RID: 63
		public fsConfig Config;

		// Token: 0x020000B4 RID: 180
		internal class fsLazyCycleDefinitionWriter
		{
			// Token: 0x06000292 RID: 658 RVA: 0x00009B5F File Offset: 0x00007D5F
			public void WriteDefinition(int id, fsData data)
			{
				if (this._references.Contains(id))
				{
					fsSerializer.EnsureDictionary(data);
					data.AsDictionary["$id"] = new fsData(id.ToString());
					return;
				}
				this._pendingDefinitions[id] = data;
			}

			// Token: 0x06000293 RID: 659 RVA: 0x00009BA0 File Offset: 0x00007DA0
			public void WriteReference(int id, Dictionary<string, fsData> dict)
			{
				if (this._pendingDefinitions.ContainsKey(id))
				{
					fsData fsData = this._pendingDefinitions[id];
					fsSerializer.EnsureDictionary(fsData);
					fsData.AsDictionary["$id"] = new fsData(id.ToString());
					this._pendingDefinitions.Remove(id);
				}
				else
				{
					this._references.Add(id);
				}
				dict["$ref"] = new fsData(id.ToString());
			}

			// Token: 0x06000294 RID: 660 RVA: 0x00009C1B File Offset: 0x00007E1B
			public void Clear()
			{
				this._pendingDefinitions.Clear();
				this._references.Clear();
			}

			// Token: 0x0400024C RID: 588
			private Dictionary<int, fsData> _pendingDefinitions = new Dictionary<int, fsData>();

			// Token: 0x0400024D RID: 589
			private HashSet<int> _references = new HashSet<int>();
		}
	}
}
