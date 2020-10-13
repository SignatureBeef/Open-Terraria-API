using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x0200003E RID: 62
	public class BindEventLocalizationsConverter : fsDirectConverter<BindEventLocalizations>
	{
		// Token: 0x06000183 RID: 387 RVA: 0x000083E4 File Offset: 0x000065E4
		protected override fsResult DoSerialize(BindEventLocalizations model, Dictionary<string, fsData> serialized)
		{
			foreach (KeyValuePair<string, string> keyValuePair in model.AvailableLocalizations)
			{
				base.SerializeMember<string>(serialized, null, keyValuePair.Key, keyValuePair.Value);
			}
			return fsResult.Success;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref BindEventLocalizations model)
		{
			return fsResult.Fail("Not implemented");
		}
	}
}
