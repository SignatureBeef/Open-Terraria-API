using System;

namespace OTAPI.Callbacks.Terraria
{
	internal partial class Console
	{
		internal static void WriteLine()
		{
			if (Hooks.Console.WriteLine?.Invoke(new ConsoleHookArgs()
			{
				Format = null,
				Arg1 = null,
				Arg2 = null
			}) == HookResult.Cancel)
				return;

			System.Console.WriteLine();
		}
		internal static void WriteLine(string format, object arg0, object arg1)
		{
			if (Hooks.Console.WriteLine?.Invoke(new ConsoleHookArgs()
			{
				Format = format,
				Arg1 = arg0,
				Arg2 = arg1
			}) == HookResult.Cancel)
				return;

			System.Console.WriteLine(format, arg0, arg1);
		}

		internal static void WriteLine(object value)
		{
			if (Hooks.Console.WriteLine?.Invoke(new ConsoleHookArgs()
			{
				Format = null,
				Arg1 = value,
				Arg2 = null
			}) == HookResult.Cancel)
				return;

			System.Console.WriteLine(value);
		}
	}
}
