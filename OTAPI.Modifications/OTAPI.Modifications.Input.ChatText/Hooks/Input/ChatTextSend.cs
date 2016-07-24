namespace OTAPI.Core
{
	public static partial class Hooks
	{
		public static partial class Input
		{
            /// <summary>
            /// Occurs right before a chat message is sent to the server
            /// </summary>
			public static HookResultHandler ChatSend;
		}
	}
}
