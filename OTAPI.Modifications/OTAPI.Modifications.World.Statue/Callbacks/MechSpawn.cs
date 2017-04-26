namespace OTAPI.Callbacks.Terraria
{
	internal static partial class World
	{
		/// <summary>
		/// This method is injected both the NPC and Item MechSpawn functions
		/// in order to control the return value.
		/// </summary>
		/// <returns>True to continue on to vanilla code, otherwise false</returns>
		internal static bool MechSpawn(float x, float y, int type, ref int num, ref int num2, ref int num3, StatueType caller)
		{
			var res = Hooks.World.Statue?.Invoke(caller, x, y, type, ref num, ref num2, ref num3);
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}
	}
}
