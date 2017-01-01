using OTAPI.Tests.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OTAPI.Client.Xna.Tests
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			var runner = new GameRunner();
			runner.PreStart += AttachHooks;
			runner.Main(args);
		}

		static void AttachHooks(object sender, EventArgs args)
		{

		}
	}
}
