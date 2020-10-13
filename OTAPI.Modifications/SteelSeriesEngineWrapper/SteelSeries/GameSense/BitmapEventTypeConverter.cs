using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x0200006C RID: 108
	public class BitmapEventTypeConverter : fsDirectConverter<BitmapEventHandlerType>
	{
		// Token: 0x060001EB RID: 491 RVA: 0x00008F6F File Offset: 0x0000716F
		protected override fsResult DoSerialize(BitmapEventHandlerType model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<string>(serialized, null, "device-type", "rgb-per-key-zones");
			base.SerializeMember<string>(serialized, null, "mode", "bitmap");
			base.SerializeMember<bool>(serialized, null, "value_optional", true);
			return fsResult.Success;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref BitmapEventHandlerType model)
		{
			return fsResult.Fail("Not implemented");
		}
	}
}
