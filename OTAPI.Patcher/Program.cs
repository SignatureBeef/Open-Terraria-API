using NDesk.Options;
using OTAPI.Patcher.Inject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OTAPI.Patcher
{
    public class Program
    {
        /// <summary>
        /// Assembilies loaded from disk that are to be passed to the InjectionRunner.
        /// </summary>
        /// <remarks>
        ///     Key     = Key to be used in the InjectionContext.Assembilies
        /// </remarks>
        private static Dictionary<String, String> _assembilies = new Dictionary<String, String>();

        private static OptionSet _startupOptions;

        private const String OutputFileName = "OTAPI.dll";

        public static void Main(string[] args)
        {
            args = "-asm=Terraria:TerrariaServer.exe -asm=OTAPI:OTAPI.Core.dll".Split(' ');
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Clear();

            Console.WriteLine("Open Terraria API patcher, v2\thttps://openterraria.com/");

            // Parse command line arguments.
            ParseArguments(args);

            //Ensure that the binaries specified exist before anything occurs to them.
            VerifyAssemblies();

            //Merge the binaries together
            //if(options.Merge)
            var repacker = new ILRepacking.ILRepack(new ILRepacking.RepackOptions()
            {
                InputAssemblies = _assembilies.Values.ToArray(),
                OutputFile = OutputFileName,
                TargetKind = ILRepacking.ILRepack.Kind.Dll,
                SearchDirectories = new[] { Environment.CurrentDirectory }
            });
            repacker.Repack();

            //Run injection files through processor
            Run();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Runs all injections that have been loaded.
        /// </summary>
        static void Run()
        {
            var context = InjectionContext.LoadFromAssemblies<OTAPIContext>(OutputFileName);
            var runner = InjectionRunner.LoadFromAssembly<Program>(context);

            //Apply the injections to the loaded binary
            runner.Run(_startupOptions);

            runner.SaveAllTo(OutputFileName);
        }

        /// <summary>
        /// Verifies that all assembilies exist and are usable by the InjectionContexts
        /// </summary>
        static void VerifyAssemblies()
        {
            foreach (var asm in _assembilies.Keys)
            {
                if (!File.Exists(_assembilies[asm]))
                {
                    throw new FileNotFoundException("Cannot find input assembly", _assembilies[asm]);
                }
            }
        }

        /// <summary>
        /// Displays the applications help text in the console window
        /// </summary>
        static void DisplayHelp()
        {
            Console.WriteLine("TODO");
        }

        /// <summary>
        /// Parses string arguments, typically launch arguments.
        /// </summary>
        /// <param name="args"></param>
        static void ParseArguments(string[] args)
        {
            _startupOptions = new OptionSet()
                .Add("asm={:}", (k, v) => _assembilies.Add(k, v))
                .Add("?|h|help", h => DisplayHelp());
            _startupOptions.Parse(args);
        }
    }
}
