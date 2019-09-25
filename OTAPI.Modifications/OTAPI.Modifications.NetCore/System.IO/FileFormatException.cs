namespace System.IO
{
	[Serializable]
	public class FileFormatException : FormatException
	{
		public FileFormatException() : base() { }

		public FileFormatException(string message) : base(message) { }
	}
}
