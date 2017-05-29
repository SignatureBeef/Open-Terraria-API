//using Microsoft.Xna.Framework;
//using System.Collections.Generic;
//using Terraria;

//namespace OTAPI.Callbacks.Terraria
//{
//	internal static partial class Collision
//	{
//		internal static bool PreGetEntityEdgeTiles(ref List<Point> results, Entity entity, bool left = true, bool right = true, bool up = true, bool down = true)
//		{
//			var result = Hooks.Collision.PreGetEntityEdgeTiles?.Invoke(ref results, entity, left, right, up, down);
//			if (result.HasValue) return result.Value == HookResult.Continue;
//			return true;
//		}

//		internal static void PostGetEntityEdgeTiles(Entity entity, bool left = true, bool right = true, bool up = true, bool down = true)
//		{
//			Hooks.Collision.PostGetEntityEdgeTiles?.Invoke(entity, left, right, up, down);
//		}
//	}
//}
