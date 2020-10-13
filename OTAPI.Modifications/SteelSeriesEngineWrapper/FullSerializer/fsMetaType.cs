using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using FullSerializer.Internal;

namespace FullSerializer
{
	// Token: 0x0200001B RID: 27
	public class fsMetaType
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x0000539C File Offset: 0x0000359C
		public static fsMetaType Get(fsConfig config, Type type)
		{
			return null;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000053E8 File Offset: 0x000035E8
		public static void ClearCache()
		{
			fsMetaType._configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000053F4 File Offset: 0x000035F4
		private fsMetaType(fsConfig config, Type reflectedType)
		{
			this.ReflectedType = reflectedType;
			List<fsMetaProperty> list = new List<fsMetaProperty>();
			fsMetaType.CollectProperties(config, list, reflectedType);
			this.Properties = list.ToArray();
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00005428 File Offset: 0x00003628
		private static void CollectProperties(fsConfig config, List<fsMetaProperty> properties, Type reflectedType)
		{
			bool flag = config.DefaultMemberSerialization == fsMemberSerialization.OptIn;
			bool flag2 = config.DefaultMemberSerialization == fsMemberSerialization.OptOut;
			fsObjectAttribute attribute = fsPortableReflection.GetAttribute<fsObjectAttribute>(reflectedType);
			if (attribute != null)
			{
				flag = (attribute.MemberSerialization == fsMemberSerialization.OptIn);
				flag2 = (attribute.MemberSerialization == fsMemberSerialization.OptOut);
			}
			MemberInfo[] declaredMembers = reflectedType.GetDeclaredMembers();
			MemberInfo[] array = declaredMembers;
			for (int i = 0; i < array.Length; i++)
			{
				MemberInfo member = array[i];
				if (!config.IgnoreSerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(member, t)))
				{
					PropertyInfo propertyInfo = member as PropertyInfo;
					FieldInfo fieldInfo = member as FieldInfo;
					if ((!(propertyInfo == null) || !(fieldInfo == null)) && (!flag || config.SerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(member, t))) && (!flag2 || !config.IgnoreSerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(member, t))))
					{
						if (propertyInfo != null)
						{
							if (fsMetaType.CanSerializeProperty(config, propertyInfo, declaredMembers, flag2))
							{
								properties.Add(new fsMetaProperty(config, propertyInfo));
							}
						}
						else if (fieldInfo != null && fsMetaType.CanSerializeField(config, fieldInfo, flag2))
						{
							properties.Add(new fsMetaProperty(config, fieldInfo));
						}
					}
				}
			}
			if (reflectedType.Resolve().BaseType != null)
			{
				fsMetaType.CollectProperties(config, properties, reflectedType.Resolve().BaseType);
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00005598 File Offset: 0x00003798
		private static bool IsAutoProperty(PropertyInfo property, MemberInfo[] members)
		{
			if (!property.CanWrite || !property.CanRead)
			{
				return false;
			}
			string b = "<" + property.Name + ">k__BackingField";
			for (int i = 0; i < members.Length; i++)
			{
				if (members[i].Name == b)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000055F0 File Offset: 0x000037F0
		private static bool CanSerializeProperty(fsConfig config, PropertyInfo property, MemberInfo[] members, bool annotationFreeValue)
		{
			if (typeof(Delegate).IsAssignableFrom(property.PropertyType))
			{
				return false;
			}
			MethodInfo getMethod = property.GetGetMethod(false);
			MethodInfo setMethod = property.GetSetMethod(false);
			return (!(getMethod != null) || !getMethod.IsStatic) && (!(setMethod != null) || !setMethod.IsStatic) && property.GetIndexParameters().Length == 0 && (config.SerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(property, t)) || (property.CanRead && property.CanWrite && (((config.SerializeNonAutoProperties || fsMetaType.IsAutoProperty(property, members)) && getMethod != null && (config.SerializeNonPublicSetProperties || setMethod != null)) || annotationFreeValue)));
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000056E0 File Offset: 0x000038E0
		private static bool CanSerializeField(fsConfig config, FieldInfo field, bool annotationFreeValue)
		{
			return !typeof(Delegate).IsAssignableFrom(field.FieldType) && !field.IsDefined(typeof(CompilerGeneratedAttribute), false) && !field.IsStatic && (config.SerializeAttributes.Any((Type t) => fsPortableReflection.HasAttribute(field, t)) || annotationFreeValue || field.IsPublic);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00005770 File Offset: 0x00003970
		public bool EmitAotData()
		{
			if (this._hasEmittedAotData)
			{
				return false;
			}
			this._hasEmittedAotData = true;
			for (int i = 0; i < this.Properties.Length; i++)
			{
				if (!this.Properties[i].IsPublic)
				{
					return false;
				}
			}
			if (!this.HasDefaultConstructor)
			{
				return false;
			}
			fsAotCompilationManager.AddAotCompilation(this.ReflectedType, this.Properties, this._isDefaultConstructorPublic);
			return true;
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000BB RID: 187 RVA: 0x000057D4 File Offset: 0x000039D4
		// (set) Token: 0x060000BC RID: 188 RVA: 0x000057DC File Offset: 0x000039DC
		public fsMetaProperty[] Properties { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000BD RID: 189 RVA: 0x000057E8 File Offset: 0x000039E8
		public bool HasDefaultConstructor
		{
			get
			{
				if (this._hasDefaultConstructorCache == null)
				{
					if (this.ReflectedType.Resolve().IsArray)
					{
						this._hasDefaultConstructorCache = new bool?(true);
						this._isDefaultConstructorPublic = true;
					}
					else if (this.ReflectedType.Resolve().IsValueType)
					{
						this._hasDefaultConstructorCache = new bool?(true);
						this._isDefaultConstructorPublic = true;
					}
					else
					{
						ConstructorInfo declaredConstructor = this.ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes);
						this._hasDefaultConstructorCache = new bool?(declaredConstructor != null);
						if (declaredConstructor != null)
						{
							this._isDefaultConstructorPublic = declaredConstructor.IsPublic;
						}
					}
				}
				return this._hasDefaultConstructorCache.Value;
			}
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00005898 File Offset: 0x00003A98
		public object CreateInstance()
		{
			if (this.ReflectedType.Resolve().IsInterface || this.ReflectedType.Resolve().IsAbstract)
			{
				throw new Exception("Cannot create an instance of an interface or abstract type for " + this.ReflectedType);
			}
			if (typeof(string) == this.ReflectedType)
			{
				return string.Empty;
			}
			if (!this.HasDefaultConstructor)
			{
				return FormatterServices.GetSafeUninitializedObject(this.ReflectedType);
			}
			if (this.ReflectedType.Resolve().IsArray)
			{
				return Array.CreateInstance(this.ReflectedType.GetElementType(), 0);
			}
			object result;
			try
			{
				result = Activator.CreateInstance(this.ReflectedType, true);
			}
			catch (MissingMethodException innerException)
			{
				throw new InvalidOperationException("Unable to create instance of " + this.ReflectedType + "; there is no default constructor", innerException);
			}
			catch (TargetInvocationException innerException2)
			{
				throw new InvalidOperationException("Constructor of " + this.ReflectedType + " threw an exception when creating an instance", innerException2);
			}
			catch (MemberAccessException innerException3)
			{
				throw new InvalidOperationException("Unable to access constructor of " + this.ReflectedType, innerException3);
			}
			return result;
		}

		// Token: 0x04000040 RID: 64
		private static Dictionary<fsConfig, Dictionary<Type, fsMetaType>> _configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();

		// Token: 0x04000041 RID: 65
		public Type ReflectedType;

		// Token: 0x04000042 RID: 66
		private bool _hasEmittedAotData;

		// Token: 0x04000044 RID: 68
		private bool? _hasDefaultConstructorCache;

		// Token: 0x04000045 RID: 69
		private bool _isDefaultConstructorPublic;
	}
}
