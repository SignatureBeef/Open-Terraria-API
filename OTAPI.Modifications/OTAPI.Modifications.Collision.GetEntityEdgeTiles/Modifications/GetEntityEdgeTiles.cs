//using Microsoft.Xna.Framework;
//using OTAPI.Patcher.Engine.Extensions;
//using OTAPI.Patcher.Engine.Modification;
//using System.Collections.Generic;

//namespace OTAPI.Patcher.Engine.Modifications.Hooks.Collision
//{
//	public class GetEntityEdgeTiles : ModificationBase
//	{
//		public override System.Collections.Generic.IEnumerable<string> AssemblyTargets => new[]
//		{
//			"TerrariaServer, Version=1.4.3.5, Culture=neutral, PublicKeyToken=null"
//		};
//		public override string Description => "Hooking Collision.GetEntityEdgeTiles...";

//		public override void Run()
//		{
//			var vanilla = this.SourceDefinition.Type("Terraria.Collision").Method("GetEntityEdgeTiles");

//			List<Point> results = null;
//			var cbkBegin = this.Method(() => OTAPI.Callbacks.Terraria.Collision.PreGetEntityEdgeTiles(ref results, null, false, false, false, false));

//			vanilla.InjectNonVoidBeginCallback(cbkBegin);
//		}
//	}
//}
