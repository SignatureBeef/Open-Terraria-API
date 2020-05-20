using System.Drawing;

namespace System.Windows.Forms
{
	public class Screen
	{
		public static Screen[] AllScreens { get; }
		public int BitsPerPixel => 0;

		public Rectangle Bounds => default(Rectangle);

		public string DeviceName => "";

		public bool Primary => false;

		public static Screen PrimaryScreen => null;
		public Rectangle WorkingArea { get; }
		public override bool Equals(object obj) => false;
		public static Screen FromPoint(Point point) => null;
		public static Screen FromRectangle(Rectangle rect) => null;
		public static Screen FromControl(Control control) => null;
		public static Screen FromHandle(IntPtr hwnd) => null;
		public static Rectangle GetWorkingArea(Point pt) => default(Rectangle);
		public static Rectangle GetWorkingArea(Rectangle rect) => default(Rectangle);
		public static Rectangle GetWorkingArea(Control ctl) => default(Rectangle);
		public static Rectangle GetBounds(Point pt) => default(Rectangle);
		public static Rectangle GetBounds(Rectangle rect) => default(Rectangle);
		public static Rectangle GetBounds(Control ctl) => default(Rectangle);
		public override int GetHashCode() => 0;

		public override string ToString() => "";
	}
}
