using System;

namespace OTA.Misc
{
    /// <summary>
    /// The OTA specific exit exception to trigger the server/application to close
    /// </summary>
    /// <remarks>This may be used from command/web threads etc</remarks>
	public class ExitException : Exception
	{
		public ExitException() { }

		public ExitException(string Info) : base(Info) { }
	}
}