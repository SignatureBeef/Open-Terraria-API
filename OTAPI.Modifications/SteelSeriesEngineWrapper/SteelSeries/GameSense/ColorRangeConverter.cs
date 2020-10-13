using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x0200003A RID: 58
	internal class ColorRangeConverter : fsDirectConverter<ColorRange>
	{
		// Token: 0x0600017B RID: 379 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref ColorRange model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00008290 File Offset: 0x00006490
		protected override fsResult DoSerialize(ColorRange model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<uint>(serialized, null, "low", model.low);
			base.SerializeMember<uint>(serialized, null, "high", model.high);
			base.SerializeMember<AbstractColor_Nonrecursive>(serialized, null, "color", model.color);
			return fsResult.Success;
		}
	}
}
