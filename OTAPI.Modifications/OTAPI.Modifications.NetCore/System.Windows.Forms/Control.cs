using System.Drawing;

namespace System.Windows.Forms
{
	public class Control
	{
		public Drawing.Point Location { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }

		public static Control FromHandle(IntPtr handle) => null;

		public Size MinimumSize { get; set; }

		public void SendToBack() { }
		public void BringToFront() { }
	}
}
