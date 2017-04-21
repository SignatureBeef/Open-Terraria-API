namespace OTAPI.Callbacks.Terraria
{
	internal static partial class IO
	{
		private static bool IsMono()
		{
			return System.Type.GetType("Mono.Runtime") != null;
		}

		internal static bool DeleteFile(string path)
		{
			bool deleted = false;

			if (IsMono())
			{
				System.IO.File.Delete(path);
				deleted = true;
			}
			else
			{
				deleted = global::Terraria.Utilities.FileOperationAPIWrapper.MoveToRecycleBin(path);
			}

			return deleted;
		}
	}
}
