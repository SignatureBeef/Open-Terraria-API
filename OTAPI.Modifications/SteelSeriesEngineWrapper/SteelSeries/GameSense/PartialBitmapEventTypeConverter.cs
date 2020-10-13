using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000075 RID: 117
	public class PartialBitmapEventTypeConverter : fsDirectConverter<PartialBitmapEventHandlerType>
	{
		// Token: 0x0600021B RID: 539 RVA: 0x000093A4 File Offset: 0x000075A4
		protected override fsResult DoSerialize(PartialBitmapEventHandlerType model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<string>(serialized, null, "device-type", "rgb-per-key-zones");
			base.SerializeMember<string>(serialized, null, "mode", "partial-bitmap");
			base.SerializeMember<bool>(serialized, null, "value_optional", true);
			base.SerializeMember<string[]>(serialized, null, "excluded-events", model.EventsToExclude);
			return fsResult.Success;
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref PartialBitmapEventHandlerType model)
		{
			return fsResult.Fail("Not implemented");
		}
	}
}
