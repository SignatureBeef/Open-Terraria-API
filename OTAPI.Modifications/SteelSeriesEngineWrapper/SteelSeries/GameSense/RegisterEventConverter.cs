using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000052 RID: 82
	public class RegisterEventConverter : fsDirectConverter<Register_Event>
	{
		// Token: 0x060001C1 RID: 449 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Register_Event model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00008B50 File Offset: 0x00006D50
		protected override fsResult DoSerialize(Register_Event model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<string>(serialized, null, "game", model.game.ToUpper());
			base.SerializeMember<string>(serialized, null, "event", model.eventName);
			base.SerializeMember<int>(serialized, null, "min_value", model.minValue);
			base.SerializeMember<int>(serialized, null, "max_value", model.maxValue);
			base.SerializeMember<uint>(serialized, null, "icon_id", (uint)model.iconId);
			return fsResult.Success;
		}
	}
}
