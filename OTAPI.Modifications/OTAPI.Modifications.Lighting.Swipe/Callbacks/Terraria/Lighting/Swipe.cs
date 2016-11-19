using System;
using System.Reflection.Emit;

namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Lighting
	{
		static Action<object> LightingSwipe = GetLightingSwipeMethod();

		/// <summary>
		/// Generates a dynamic action that replicates vanilla functionality
		/// as the types we need are private at compile time.
		/// </summary>
		/// <remarks>
		/// An alternate would be to inject classes that replicate the nested classes in Terraria.Lighting,
		/// and make the nested classes inherit ours. However, that would make me have to maintain extra code
		/// </remarks>
		/// <returns>Action that will process lighting swipe data</returns>
		private static Action<object> GetLightingSwipeMethod()
		{
			var dm = new DynamicMethod("InvokeSwipe", typeof(void), new[] { typeof(object) });
			var processor = dm.GetILGenerator();

			//IL_0000: nop
			//IL_0001: ldarg.0
			//IL_0002: isinst [OTAPI]Terraria.Lighting/LightingSwipeData
			//IL_0007: stloc.0
			//IL_0008: ldloc.0
			//IL_0009: ldfld class [mscorlib]System.Action`1<class [OTAPI]Terraria.Lighting/LightingSwipeData> [OTAPI]Terraria.Lighting/LightingSwipeData::function
			//IL_000e: ldloc.0
			//IL_000f: callvirt instance void class [mscorlib]System.Action`1<class [OTAPI]Terraria.Lighting/LightingSwipeData>::Invoke(!0)
			//IL_0014: nop
			//IL_0015: ret

			var tSwipeData = typeof(global::Terraria.Lighting).GetNestedType("LightingSwipeData");
			var fSwipeData = tSwipeData.GetField("function");

			var swipeData = processor.DeclareLocal(tSwipeData, false);

			processor.Emit(OpCodes.Ldarg_0);
			processor.Emit(OpCodes.Isinst, tSwipeData);
			processor.Emit(OpCodes.Stloc_0);
			processor.Emit(OpCodes.Ldloc_0);
			processor.Emit(OpCodes.Ldfld, fSwipeData);
			processor.Emit(OpCodes.Ldloc_0);
			processor.Emit(OpCodes.Callvirt, fSwipeData.FieldType.GetMethod("Invoke"));
			processor.Emit(OpCodes.Ret);

			return (Action<object>)dm.CreateDelegate(typeof(Action<object>));
		}
		
		internal static void Swipe(object lightingSwipeData)
		{
			var result = Hooks.Lighting.Swipe?.Invoke(lightingSwipeData);
			if (result == HookResult.Cancel) return;

			LightingSwipe(lightingSwipeData);
		}
	}
}
