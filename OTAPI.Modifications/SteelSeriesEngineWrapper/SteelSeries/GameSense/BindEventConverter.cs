using System;
using System.Collections.Generic;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x0200003C RID: 60
	public class BindEventConverter : fsDirectConverter<Bind_Event>
	{
		// Token: 0x0600017F RID: 383 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref Bind_Event model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00008308 File Offset: 0x00006508
		protected override fsResult DoSerialize(Bind_Event model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<string>(serialized, null, "game", model.game.ToUpper());
			base.SerializeMember<string>(serialized, null, "event", model.eventName);
			base.SerializeMember<int>(serialized, null, "min_value", model.minValue);
			base.SerializeMember<int>(serialized, null, "max_value", model.maxValue);
			base.SerializeMember<uint>(serialized, null, "icon_id", (uint)model.iconId);
			base.SerializeMember<AbstractHandler[]>(serialized, null, "handlers", model.handlers);
			if (model.defaultDisplayName != null)
			{
				base.SerializeMember<string>(serialized, null, "default_display_name", model.defaultDisplayName);
			}
			if (model.localizedDisplayNames != null)
			{
				BindEventLocalizations value = new BindEventLocalizations
				{
					AvailableLocalizations = model.localizedDisplayNames
				};
				base.SerializeMember<BindEventLocalizations>(serialized, null, "localized_display_names", value);
			}
			return fsResult.Success;
		}
	}
}
