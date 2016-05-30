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
        /// Assemblies loaded from disk that are to be passed to the InjectionRunner.
        /// </summary>
        /// <remarks>
        ///     Key     = Key to be used in the InjectionContext.Assembilies
        /// </remarks>
        private static Dictionary<String, String> _patchAssemblies = new Dictionary<String, String>();

        /// <summary>
        /// Assemblies on disk that are to be merged into one assembly.
        /// </summary>
        private static List<String> _mergeAsseblies = new List<String>();

        private static OptionSet _startupOptions;

        private static String OutputFileName;
        private static String OutputAssemblyName;
        private static String MergeOutputFileName;

        public static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Clear();

            Console.WriteLine("Open Terraria API patcher, v2\thttps://openterraria.com/");

            // Parse command line arguments.
            ParseArguments(args);

            //Merge the binaries together
            if (!String.IsNullOrWhiteSpace(MergeOutputFileName))
            {
                //Ensure that the binaries specified exist before anything occurs to them.
                VerifyAssemblies(_mergeAsseblies);

                var repacker = new ILRepacking.ILRepack(new ILRepacking.RepackOptions()
                {
                    InputAssemblies = _mergeAsseblies.ToArray(),
                    OutputFile = MergeOutputFileName,
                    TargetKind = ILRepacking.ILRepack.Kind.Dll,
                    SearchDirectories = new[] { Environment.CurrentDirectory },
                    Parallel = true,
                    Version = new Version(1, 0, 0, 0),
                    CopyAttributes = true,
#if DEBUG
                    DebugInfo = true
#endif
                });
                repacker.Repack();
            }

            //Ensure that the binaries specified exist before anything occurs to them.
            VerifyAssemblies(_patchAssemblies.Values);

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
            var context = InjectionContext.LoadFromAssemblies<Modifications.Helpers.OTAPIContext/*TODO: load the dynamic asm and load types at inject time into a map*/>(_patchAssemblies);
            var runner = InjectionRunner.LoadFromAssembly<Program>(context);

            //Apply the injections to the loaded binary
            runner.Run(_startupOptions);

            runner.SaveAs(OutputFileName, OutputAssemblyName);
        }

        /// <summary>
        /// Verifies that all assembilies exist and are usable by the InjectionContexts
        /// </summary>
        static void VerifyAssemblies(IEnumerable<String> assemblies)
        {
            foreach (var asm in assemblies)
            {
                if (!File.Exists(asm))
                {
                    throw new FileNotFoundException("Cannot find input assembly", asm);
                }
            }
        }

        /// <summary>
        /// Displays the applications help text in the console window
        /// </summary>
        static void DisplayHelp()
        {
            _startupOptions.WriteOptionDescriptions(Console.Out);
        }

        /// <summary>
        /// Parses string arguments, typically launch arguments.
        /// </summary>
        /// <param name="args"></param>
        static void ParseArguments(string[] args)
        {
            _startupOptions = new OptionSet()
                .Add("patch={:}", (k, v) => _patchAssemblies.Add(k, v))
                .Add("merge=|m=", (asm) => _mergeAsseblies.Add(asm))
                .Add("merge-output=|mo=", merge => MergeOutputFileName = merge)
                .Add("out=|o=|output-file=", output => OutputFileName = output)
                .Add("output-name=", name => OutputAssemblyName = name)
                .Add("?|h|help", h => DisplayHelp());
            _startupOptions.Parse(args);
        }
    }
}
