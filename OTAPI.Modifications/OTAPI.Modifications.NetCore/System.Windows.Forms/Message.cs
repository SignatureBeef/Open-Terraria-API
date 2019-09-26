namespace System.Windows.Forms
{
	public struct Message
	{
		public IntPtr HWnd { get; set; }
		public int Msg { get; set; }
		public IntPtr WParam { get; set; }
		public IntPtr LParam { get; set; }

		public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) => new Message();
	}
}
