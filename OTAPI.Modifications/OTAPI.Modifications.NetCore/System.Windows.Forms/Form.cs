namespace System.Windows.Forms
{
	public class Form : Control
	{
		public FormWindowState WindowState { get; set; }

		public static Form ActiveForm => null;
	}
}
