using System;

namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Console
		{
			#region Handlers
			public delegate HookResult WriteLineHandler<ConsoleHookArgs>(ConsoleHookArgs value);
			public delegate HookResult WriteLineArgsHandler(object format, object arg0, object arg1);
			#endregion

			/// <summary>
			/// Occurs each time vanilla calls Console.WriteLine
			/// </summary>
			public static WriteLineHandler<ConsoleHookArgs> WriteLine;
		}
	}

	public struct ConsoleHookArgs
	{
		public string Format { get; set; }

		public object Arg1 { get; set; }

		public object Arg2 { get; set; }
	}
}
