namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Lighting
	{
		//TODO: somehow make this not an object
		internal static void Swipe(object lightingSwipeData)
		{
			var result = Hooks.Lighting.Swipe?.Invoke(lightingSwipeData);
			if (result == HookResult.Cancel) return;
			
			((dynamic)lightingSwipeData).function.Invoke(lightingSwipeData);
		}
	}
}
