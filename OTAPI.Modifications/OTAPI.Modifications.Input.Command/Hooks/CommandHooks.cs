namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Command
		{
			#region Handlers
			public delegate HookResult ProcessCommandHandler(string lowered, string raw);
			#endregion

			/// <summary>
			/// Occurs when the server is to start listening for commands.
			/// </summary>
			public static HookResultHandler StartCommandThread;

			/// <summary>
			/// Occurs when a console command has been sent and is to be processed
			/// </summary>
			public static ProcessCommandHandler Process;
		}
	}
}
