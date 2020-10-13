using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000028 RID: 40
	public class fsWeakReferenceConverter : fsConverter
	{
		// Token: 0x06000114 RID: 276 RVA: 0x00007234 File Offset: 0x00005434
		public override bool CanProcess(Type type)
		{
			return type == typeof(WeakReference);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x000059C8 File Offset: 0x00003BC8
		public override bool RequestInheritanceSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00007248 File Offset: 0x00005448
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			WeakReference weakReference = (WeakReference)instance;
			fsResult fsResult = fsResult.Success;
			serialized = fsData.CreateDictionary();
			if (weakReference.IsAlive)
			{
				fsData value;
				fsResult fsResult2;
				fsResult = (fsResult2 = fsResult + this.Serializer.TrySerialize<object>(weakReference.Target, out value));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				serialized.AsDictionary["Target"] = value;
				serialized.AsDictionary["TrackResurrection"] = new fsData(weakReference.TrackResurrection);
			}
			return fsResult;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000072C8 File Offset: 0x000054C8
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + base.CheckType(data, fsDataType.Object));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			if (data.AsDictionary.ContainsKey("Target"))
			{
				fsData data2 = data.AsDictionary["Target"];
				object target = null;
				fsResult = (fsResult2 = fsResult + this.Serializer.TryDeserialize(data2, typeof(object), ref target));
				if (fsResult2.Failed)
				{
					return fsResult;
				}
				bool trackResurrection = false;
				if (data.AsDictionary.ContainsKey("TrackResurrection") && data.AsDictionary["TrackResurrection"].IsBool)
				{
					trackResurrection = data.AsDictionary["TrackResurrection"].AsBool;
				}
				instance = new WeakReference(target, trackResurrection);
			}
			return fsResult;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00007399 File Offset: 0x00005599
		public override object CreateInstance(fsData data, Type storageType)
		{
			return new WeakReference(null);
		}
	}
}
