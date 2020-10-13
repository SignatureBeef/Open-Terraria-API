using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000079 RID: 121
	internal class RateStaticConverter : fsDirectConverter<RateStatic>
	{
		// Token: 0x06000229 RID: 553 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref RateStatic model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00009573 File Offset: 0x00007773
		protected override fsResult DoSerialize(RateStatic model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<uint>(serialized, null, "frequency", model.frequency);
			base.SerializeMember<uint>(serialized, null, "repeat_limit", model.repeatLimit);
			return fsResult.Success;
		}
	}
}
