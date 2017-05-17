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
			return Hooks.Command.StartCommandThread?.Invoke() != HookResult.Cancel;
		}

		/// <summary>
		/// Injected into startDedInput to capture all input from the ReadLine method.
		/// </summary>
		/// <param name="lowered">Lowered line read from the console</param>
		/// <param name="raw">Raw text read from the console</param>
		/// <returns>True to continue on to vanilla code, otherwise false</returns>
		internal static bool ProcessCommand(string lowered, string raw)
		{
			return Hooks.Command.Process?.Invoke(lowered, raw) != HookResult.Cancel;
		}
	}
}
