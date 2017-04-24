using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTAPI.Modifications.NetworkText
{
	public delegate HookResult BeforeChatMessageHandler(global::Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer);
	public delegate void AfterChatMessageHandler(global::Terraria.Localization.NetworkText text, ref Color color, ref int ignorePlayer);
}
