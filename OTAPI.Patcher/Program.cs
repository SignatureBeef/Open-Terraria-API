using Mono.Cecil;
using NDesk.Options;
using OTAPI.Patcher.Modification;
using System;
using System.Collections.Generic;
using System.IO;

namespace OTAPI.Patcher
{
    public class Program
    {
        /// <summary>
        /// Assemblies loaded from disk that are to be passed to the ModificationRunner.
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

            //Run modification files through processor
            Run();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Runs all modifications that have been loaded.
        /// </summary>
        static void Run()
        {
            var context = ModificationContext.LoadFromAssemblies<Modifications.Helpers.OTAPIContext/*We could do this dynamically, but this class is for OTAPI*/>(_patchAssemblies);
            var runner = ModificationRunner.LoadFromAssembly<Program>(context);

            //Apply the modifications to the loaded binary
            runner.Run(_startupOptions);

            runner.SaveAs(OutputFileName, OutputAssemblyName);
        }

        /// <summary>
        /// Verifies that all assemblies exist and are usable by the ModificationContext's
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
