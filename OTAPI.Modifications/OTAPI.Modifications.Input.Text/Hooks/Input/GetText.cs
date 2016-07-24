namespace OTAPI.Core
{
	public static partial class Hooks
	{
		public static partial class Input
		{
			#region Handlers
			public delegate HookResult GetTextHandler(ref string chatText);
			#endregion

			public static GetTextHandler GetText;
		}
	}
}
