using System;
using System.Collections.Generic;

namespace FullSerializer
{
	// Token: 0x0200000C RID: 12
	public abstract class fsDirectConverter<TModel> : fsDirectConverter
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00002D8B File Offset: 0x00000F8B
		public override Type ModelType
		{
			get
			{
				return typeof(TModel);
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002D98 File Offset: 0x00000F98
		public sealed override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			Dictionary<string, fsData> dictionary = new Dictionary<string, fsData>();
			fsResult result = this.DoSerialize((TModel)((object)instance), dictionary);
			serialized = new fsData(dictionary);
			return result;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002DC0 File Offset: 0x00000FC0
		public sealed override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			fsResult fsResult = fsResult.Success;
			fsResult fsResult2;
			fsResult = (fsResult2 = fsResult + base.CheckType(data, fsDataType.Object));
			if (fsResult2.Failed)
			{
				return fsResult;
			}
			TModel tmodel = (TModel)((object)instance);
			fsResult += this.DoDeserialize(data.AsDictionary, ref tmodel);
			instance = tmodel;
			return fsResult;
		}

		// Token: 0x06000046 RID: 70
		protected abstract fsResult DoSerialize(TModel model, Dictionary<string, fsData> serialized);

		// Token: 0x06000047 RID: 71
		protected abstract fsResult DoDeserialize(Dictionary<string, fsData> data, ref TModel model);
	}
}
