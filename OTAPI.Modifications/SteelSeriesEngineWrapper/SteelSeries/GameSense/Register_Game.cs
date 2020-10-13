using System;
using FullSerializer;

namespace SteelSeries.GameSense
{
	// Token: 0x02000055 RID: 85
	[fsObject(Converter = typeof(RegisterGameConverter))]
	public class Register_Game
	{
		// Token: 0x04000168 RID: 360
		public string game;

		// Token: 0x04000169 RID: 361
		public string game_display_name;

		// Token: 0x0400016A RID: 362
		public IconColor icon_color_id = IconColor.Green;
	}
}
