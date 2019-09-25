namespace OTAPI.Callbacks.Terraria
{
	internal static partial class IO
	{
		internal static bool DeleteFile(string path)
		{
			System.IO.File.Delete(path);
			return true;
		}
	}
}
