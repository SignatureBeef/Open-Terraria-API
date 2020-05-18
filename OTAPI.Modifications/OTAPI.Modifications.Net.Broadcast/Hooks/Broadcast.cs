namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Net
		{
			/// <summary>
			/// Called upon Terraria.Netplay.StartBroadCasting
			/// </summary>
			public static HookResultHandler StartBroadCasting;

			/// <summary>
			/// Called upon Terraria.Netplay.StopBroadCasting
			/// </summary>
			public static HookResultHandler StopBroadCasting;
		}
	}
}
