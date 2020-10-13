using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000056 RID: 86
	public class SendEventConverter : fsDirectConverter<Send_Event>
	{
		// Token: 0x060001C9 RID: 457 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Send_Event model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00008C68 File Offset: 0x00006E68
		protected override fsResult DoSerialize(Send_Event model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<string>(serialized, null, "game", model.game);
			base.SerializeMember<string>(serialized, null, "event", model.event_name);
			base.SerializeMember<EventData>(serialized, null, "data", model.data);
			return fsResult.Success;
		}
	}
}
