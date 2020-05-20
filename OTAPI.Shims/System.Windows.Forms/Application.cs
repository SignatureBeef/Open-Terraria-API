namespace System.Windows.Forms
{
	public sealed class Application
	{
		public static event EventHandler ApplicationExit;

		public static void AddMessageFilter(IMessageFilter value) => throw new NotImplementedException();
		public static void RemoveMessageFilter(IMessageFilter value) => throw new NotImplementedException();
	}
}
