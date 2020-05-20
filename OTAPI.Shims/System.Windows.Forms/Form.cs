using System.Drawing;

namespace System.Windows.Forms
{
	public enum FormBorderStyle
	{
		None,
		FixedSingle,
		Fixed3D,
		FixedDialog,
		Sizable,
		FixedToolWindow,
		SizableToolWindow
	}

	public class Form : Control
	{
		public FormWindowState WindowState { get; set; }
		public FormBorderStyle FormBorderStyle { get; set; }

		public static Form ActiveForm => null;
		public new Size ClientSize { get; set; }
	}
}
