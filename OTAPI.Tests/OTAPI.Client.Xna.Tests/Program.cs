using OTAPI.Tests.Common;
using System;

namespace OTAPI.Client.Xna.Tests
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				var runner = new GameRunner();
				runner.PreStart += AttachHooks;
				runner.Main(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.ReadKey();
			}
		}

		static void AttachHooks(object sender, EventArgs args)
		{
		}
	}
}
