using System;
using System.Collections.Generic;
using FullSerializer;
using SteelSeries.GameSense.DeviceZone;

namespace SteelSeries.GameSense
{
	// Token: 0x02000073 RID: 115
	public class ContextColorEventTypeConverter : fsDirectConverter<ContextColorEventHandlerType>
	{
		// Token: 0x06000217 RID: 535 RVA: 0x000092F0 File Offset: 0x000074F0
		protected override fsResult DoSerialize(ContextColorEventHandlerType model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<string>(serialized, null, "mode", "context-color");
			base.SerializeMember<string>(serialized, null, "device-type", model.DeviceZone.device);
			if (model.DeviceZone.HasCustomZone())
			{
				base.SerializeMember<byte[]>(serialized, null, "custom-zone-keys", ((AbstractIlluminationDevice_CustomZone)model.DeviceZone).zone);
			}
			else
			{
				base.SerializeMember<string>(serialized, null, "zone", ((AbstractIlluminationDevice_StandardZone)model.DeviceZone).zone);
			}
			base.SerializeMember<string>(serialized, null, "context-frame-key", model.ContextFrameKey);
			base.SerializeMember<bool>(serialized, null, "value_optional", true);
			return fsResult.Success;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref ContextColorEventHandlerType model)
		{
			return fsResult.Fail("Not implemented");
		}
	}
}
