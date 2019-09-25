namespace System.Windows.Forms
{
	public class Control
	{
		public Drawing.Point Location { get; set; }

		public static Control FromHandle(IntPtr handle) => null;
	}
}
