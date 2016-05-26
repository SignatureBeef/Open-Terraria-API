using NDesk.Options;
using OTAPI.Patcher.Inject;
using System;
using System.Collections.Generic;
using System.IO;

namespace OTAPI.Patcher
{
    public class Program
    {
        private static Dictionary<String, String> assemblies = new Dictionary<string, string>();

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

            //Discover .inj files
            //Initialise processor
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
            var context = InjectionContext.LoadFromAssemblies<OTAPIContext>(assemblies);
            var runner = InjectionRunner.LoadFromAssembly<Program>(context);

        }

        /// <summary>
        /// Verifies that all assembilies exist and are usable by the InjectionContexts
        /// </summary>
        static void VerifyAssemblies()
        {
            foreach(var asm in assemblies.Keys )
            {
                if(!File.Exists(assemblies[asm]))
                {
                    throw new FileNotFoundException("Cannot find input assembly", assemblies[asm]);
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
            new OptionSet()
                .Add("asm={:}", (k, v) => assemblies.Add(k, v))
                .Add("?|h|help", h => DisplayHelp())
                .Parse(args);
        }
    }
}
