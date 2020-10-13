using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x0200005F RID: 95
	public class RepeatLimitConverter : fsDirectConverter<RepeatLimit>
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref RepeatLimit model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00008D20 File Offset: 0x00006F20
		protected override fsResult DoSerialize(RepeatLimit model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<uint>(serialized, null, "low", model.low);
			base.SerializeMember<uint>(serialized, null, "high", model.high);
			base.SerializeMember<uint>(serialized, null, "repeat_limit", model.repeatLimit);
			return fsResult.Success;
		}
	}
}
