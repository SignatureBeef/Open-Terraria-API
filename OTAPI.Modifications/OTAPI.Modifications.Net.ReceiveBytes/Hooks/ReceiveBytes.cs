namespace OTAPI
{
	public static partial class Hooks
	{
		public static partial class Net
		{
			#region Handlers
			public delegate HookResult ReceiveBytesHandler(ref byte[] bytes, ref int streamLength, ref int bufferIndex);
			#endregion

			public static ReceiveBytesHandler ReceiveBytes;
		}
	}
}
