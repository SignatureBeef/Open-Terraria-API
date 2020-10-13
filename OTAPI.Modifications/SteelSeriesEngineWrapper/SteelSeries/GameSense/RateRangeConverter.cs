using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000077 RID: 119
	internal class RateRangeConverter : fsDirectConverter<RateRange>
	{
		// Token: 0x06000223 RID: 547 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref RateRange model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00009518 File Offset: 0x00007718
		protected override fsResult DoSerialize(RateRange model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<Frequency[]>(serialized, null, "frequency", model.frequency);
			if (model.repeatLimit != null)
			{
				base.SerializeMember<RepeatLimit[]>(serialized, null, "repeat_limit", model.repeatLimit);
			}
			return fsResult.Success;
		}
	}
}
