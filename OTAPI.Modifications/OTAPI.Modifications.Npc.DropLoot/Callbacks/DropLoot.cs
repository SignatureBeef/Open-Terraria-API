using Microsoft.Xna.Framework;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Npc
	{
		public static bool DropLootBegin
		(
			ref int itemId,
			ref Vector2 pos,
			ref Vector2 randomBox,
			ref int Type,
			ref int Stack,
			ref bool noBroadcast,
			ref int prefixGiven,
			ref bool noGrabDelay,
			ref bool reverseLookup,
			global::Terraria.NPC npc
		)
		{
			int x = (int)pos.X;
			int y = (int)pos.Y;
			int width = (int)randomBox.X;
			int height = (int)randomBox.Y;

			var result = DropLootBegin
			(
				ref itemId,
				ref x,
				ref y,
				ref width,
				ref height,
				ref Type,
				ref Stack,
				ref noBroadcast,
				ref prefixGiven,
				ref noGrabDelay,
				ref reverseLookup,
				npc
			);

			pos.X = x;
			pos.Y = y;
			randomBox.X = width;
			randomBox.Y = height;

			return result;
		}
		public static bool DropLootBegin
		(
			ref int itemId,
			ref Vector2 pos,
			ref int width,
			ref int height,
			ref int Type,
			ref int Stack,
			ref bool noBroadcast,
			ref int prefixGiven,
			ref bool noGrabDelay,
			ref bool reverseLookup,
			global::Terraria.NPC npc
		)
		{
			int x = (int)pos.X;
			int y = (int)pos.Y;

			var result = DropLootBegin
			(
				ref itemId,
				ref x,
				ref y,
				ref width,
				ref height,
				ref Type,
				ref Stack,
				ref noBroadcast,
				ref prefixGiven,
				ref noGrabDelay,
				ref reverseLookup,
				npc
			);

			pos.X = x;
			pos.Y = y;

			return result;
		}

		internal static bool DropLootBegin
		(
			ref int itemId,
			ref int x,
			ref int y,
			ref int width,
			ref int height,
			ref int type,
			ref int stack,
			ref bool noBroadcast,
			ref int pfix,
			ref bool noGrabDelay,
			ref bool reverseLookup,
			global::Terraria.NPC npc
		)
		{
			var res = Hooks.Npc.PreDropLoot?.Invoke
			(
				npc,
				ref itemId,
				ref x,
				ref y,
				ref width,
				ref height,
				ref type,
				ref stack,
				ref noBroadcast,
				ref pfix,
				ref noGrabDelay,
				ref reverseLookup
			);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		internal static void DropLootEnd
		(
			Vector2 pos,
			Vector2 randomBox,
			int type,
			int stack,
			bool noBroadcast,
			int prefixGiven,
			bool noGrabDelay,
			bool reverseLookup,
			global::Terraria.NPC npc
		) => Hooks.Npc.PostDropLoot?.Invoke
		(
			npc,
			(int)pos.X,
			(int)pos.Y,
			(int)randomBox.X,
			(int)randomBox.Y,
			type,
			stack,
			noBroadcast,
			prefixGiven,
			noGrabDelay,
			reverseLookup
		);

		internal static void DropLootEnd
		(
			Vector2 pos,
			int width,
			int height,
			int type,
			int stack,
			bool noBroadcast,
			int prefixGiven,
			bool noGrabDelay,
			bool reverseLookup,
			global::Terraria.NPC npc
		) => Hooks.Npc.PostDropLoot?.Invoke
		(
			npc,
			(int)pos.X,
			(int)pos.Y,
			width,
			height,
			type,
			stack,
			noBroadcast,
			prefixGiven,
			noGrabDelay,
			reverseLookup
		);

		internal static void DropLootEnd
		(
			int x,
			int y,
			int width,
			int height,
			int type,
			int stack,
			bool noBroadcast,
			int pfix,
			bool noGrabDelay,
			bool reverseLookup,
			global::Terraria.NPC npc
		) => Hooks.Npc.PostDropLoot?.Invoke
		(
			npc,
			x,
			y,
			width,
			height,
			type,
			stack,
			noBroadcast,
			pfix,
			noGrabDelay,
			reverseLookup
		);
	}
}
