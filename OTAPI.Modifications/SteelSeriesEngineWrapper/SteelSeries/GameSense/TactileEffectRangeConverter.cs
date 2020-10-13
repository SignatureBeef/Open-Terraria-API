using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000064 RID: 100
	internal class TactileEffectRangeConverter : fsDirectConverter<TactileEffectRange>
	{
		// Token: 0x060001E2 RID: 482 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref TactileEffectRange model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00008E8C File Offset: 0x0000708C
		protected override fsResult DoSerialize(TactileEffectRange model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<uint>(serialized, null, "low", model.low);
			base.SerializeMember<uint>(serialized, null, "high", model.high);
			TactilePatternType tactilePatternType = model.TactilePatternType;
			if (tactilePatternType != TactilePatternType.Simple)
			{
				if (tactilePatternType == TactilePatternType.Custom)
				{
					base.SerializeMember<TactileEffectCustom[]>(serialized, null, "pattern", model.pattern_custom.pattern);
				}
			}
			else
			{
				base.SerializeMember<TactileEffectSimple[]>(serialized, null, "pattern", model.pattern_simple.pattern);
			}
			return fsResult.Success;
		}
	}
}
