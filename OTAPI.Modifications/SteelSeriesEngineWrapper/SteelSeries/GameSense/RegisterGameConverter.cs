using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000053 RID: 83
	public class RegisterGameConverter : fsDirectConverter<Register_Game>
	{
		// Token: 0x060001C4 RID: 452 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Register_Game model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00008BD4 File Offset: 0x00006DD4
		protected override fsResult DoSerialize(Register_Game model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<string>(serialized, null, "game", model.game);
			base.SerializeMember<string>(serialized, null, "game_display_name", model.game_display_name);
			base.SerializeMember<uint>(serialized, null, "icon_color_id", (uint)model.icon_color_id);
			return fsResult.Success;
		}
	}
}
