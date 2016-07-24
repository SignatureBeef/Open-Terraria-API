using System.Windows.Forms;

namespace OTAPI.Core
{
	public static partial class Hooks
	{
		public static partial class Input
		{
			#region Handlers
			public delegate IMessageFilter MessageFilterHandler();
			#endregion

			public static MessageFilterHandler CreateMessageFilter;
		}
	}
}
