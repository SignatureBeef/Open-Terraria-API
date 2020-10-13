using System;
using System.Collections.Generic;
using FullSerializer;
using SteelSeries.GameSense.DeviceZone;

namespace SteelSeries.GameSense
{
	// Token: 0x0200006F RID: 111
	internal class ColorHandlerConverter : fsDirectConverter<ColorHandler>
	{
		// Token: 0x0600020B RID: 523 RVA: 0x00008283 File Offset: 0x00006483
		protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref ColorHandler model)
		{
			return fsResult.Fail("Not implemented");
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00009164 File Offset: 0x00007364
		protected override fsResult DoSerialize(ColorHandler model, Dictionary<string, fsData> serialized)
		{
			base.SerializeMember<string>(serialized, null, "device-type", model.deviceZone.device);
			if (model.deviceZone.HasCustomZone())
			{
				base.SerializeMember<byte[]>(serialized, null, "custom-zone-keys", ((AbstractIlluminationDevice_CustomZone)model.deviceZone).zone);
			}
			else
			{
				base.SerializeMember<string>(serialized, null, "zone", ((AbstractIlluminationDevice_StandardZone)model.deviceZone).zone);
			}
			switch (model.mode)
			{
			case IlluminationMode.Color:
				base.SerializeMember<string>(serialized, null, "mode", "color");
				break;
			case IlluminationMode.Percent:
				base.SerializeMember<string>(serialized, null, "mode", "percent");
				break;
			case IlluminationMode.Count:
				base.SerializeMember<string>(serialized, null, "mode", "count");
				break;
			}
			switch (model.color.ColorEffectType())
			{
			case ColorEffect.Static:
				base.SerializeMember<ColorStatic>(serialized, null, "color", model.color_static);
				break;
			case ColorEffect.Gradient:
				base.SerializeMember<ColorGradient>(serialized, null, "color", model.color_gradient);
				break;
			case ColorEffect.Range:
				base.SerializeMember<ColorRange[]>(serialized, null, "color", model.color_range.ranges);
				break;
			}
			base.SerializeMember<AbstractRate>(serialized, null, "rate", model.rate);
			return fsResult.Success;
		}
	}
}
