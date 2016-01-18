using System;
using System.Diagnostics;
using System.Text;

namespace OTA.Logging
{
    /// <summary>
    /// The LogTraceListener is used to attatch into the .NET diagnostic classes, such as <see cref="System.Diagnostics.Trace"/>
    /// or <see cref="System.Diagnostics.Debug"/> in order to capture trace data and write it to ProgramLog.
    /// It is not often you would use this in a plugin as it's typically used by OTAPI itself.
    /// </summary>
	public class LogTraceListener : TraceListener
	{
		[ThreadStatic]
		static StringBuilder cache;
		
		public LogTraceListener () : base ("LogTraceListener") { }
		
		public override bool IsThreadSafe
		{
			get { return true; }
		}
		
		public override void Write (string text)
		{
			if (cache == null)
				cache = new StringBuilder ();
			cache.Append (text);
		}
		
		public override void WriteLine (string text)
		{
			if (cache != null && cache.Length > 0)
			{
				cache.Append (text);
				ProgramLog.Log (cache.ToString());
				cache.Clear ();
			}
			else
				ProgramLog.Log (text);
		}
	}
}

