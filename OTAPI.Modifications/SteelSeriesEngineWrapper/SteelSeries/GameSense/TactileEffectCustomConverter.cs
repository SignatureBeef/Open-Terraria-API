using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000062 RID: 98
	internal class TactileEffectCustomConverter : fsDirectConverter<TactileEffectCustom>
	{
		// Token: 0x060001D7 RID: 471 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref TactileEffectCustom model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00008DA4 File Offset: 0x00006FA4
		protected override fsResult DoSerialize(TactileEffectCustom model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<string>(serialized, null, "type", "custom");
			base.SerializeMember<uint>(serialized, null, "length-ms", model.length_ms);
			base.SerializeMember<uint>(serialized, null, "delay-ms", model.delay_ms);
			return fsResult.Success;
		}
	}
}
