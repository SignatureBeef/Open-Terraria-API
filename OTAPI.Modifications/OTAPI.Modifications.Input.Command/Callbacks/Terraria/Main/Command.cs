namespace OTAPI.Callbacks.Terraria
{
	internal static partial class Main
	{
		/// <summary>
		/// This method is injected into the start of the startDedInput method.
		/// The return value will dictate if normal vanilla code should continue to run.
		/// </summary>
		/// <returns>True to continue on to vanilla code, otherwise false</returns>
		internal static bool startDedInput()
		{
			var res = Hooks.Command.StartCommandThread?.Invoke();
			if (res.HasValue) return res.Value == HookResult.Continue;
			return true;
		}

		/// <summary>
		/// Injected into startDedInput to capture all input from the ReadLine method.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="raw"></param>
		/// <returns></returns>
		internal static bool ProcessCommand(string command, string raw)
		{
			var result = Hooks.Command.Process?.Invoke(command, raw);
			if (result.Value == HookResult.Cancel) return false;
			return true;
		}
	}
}
