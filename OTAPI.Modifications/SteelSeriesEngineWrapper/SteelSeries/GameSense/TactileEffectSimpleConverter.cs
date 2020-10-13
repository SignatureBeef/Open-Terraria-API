using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000066 RID: 102
	internal class TactileEffectSimpleConverter : fsDirectConverter<TactileEffectSimple>
	{
		// Token: 0x060001E6 RID: 486 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref TactileEffectSimple model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00008F28 File Offset: 0x00007128
		protected override fsResult DoSerialize(TactileEffectSimple model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<TactileEffectType>(serialized, null, "type", model.type);
			base.SerializeMember<uint>(serialized, null, "delay-ms", model.delay_ms);
			return fsResult.Success;
		}
	}
}
