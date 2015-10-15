using System;
using OTA.Command;
using OTA.Plugin;
using OTA.Misc;
using OTA.Logging;

namespace OTA.Callbacks
{
    /// <summary>
    /// Callbacks from vanilla code for miscellaneous patches
    /// </summary>
    public static class Patches
    {
        /// <summary>
        /// Used in vanilla code where there was fixed windows paths
        /// </summary>
        /// <returns>The current directory.</returns>
        public static string GetCurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }
    }

    /// <summary>
    /// Input specific callbacks from Terraria.Main
    /// </summary>
    public static class UserInput
    {
        /// <summary>
        /// The request from vanilla code to start listening for commands
        /// </summary>
        public static void ListenForCommands()
        {
            var ctx = new HookContext()
            {
                Sender = HookContext.ConsoleSender
            };
            var args = new HookArgs.StartCommandProcessing();
            HookPoints.StartCommandProcessing.Invoke(ref ctx, ref args);

            if (ctx.Result == HookResult.DEFAULT)
            {
                //This might change to the vanilla code (allowing for pure vanilla functionalities). but we'll see.
                System.Threading.ThreadPool.QueueUserWorkItem(ListenForCommands); 
            }
        }

        static readonly CommandParser _cmdParser = new CommandParser();

        /// <summary>
        /// The active server instance of the command parser
        /// </summary>
        /// <value>The command parser.</value>
        public static CommandParser CommandParser
        {
            get
            { return _cmdParser; }
        }

        /// <summary>
        /// The call to start OTA command
        /// </summary>
        /// <param name="threadContext">Thread context.</param>
        private static void ListenForCommands(object threadContext)
        {
            System.Threading.Thread.CurrentThread.Name = "APC";

            ProgramLog.Console.Print("Ready for commands.");
#if Full_API
            while (!Terraria.Netplay.disconnect)
            {
                try
                {
                    Console.Write(": ");
                    var input = Console.ReadLine();
                    _cmdParser.ParseConsoleCommand(input);
                }
                catch (ExitException e)
                {
                    ProgramLog.Log(e.Message);
                    break;
                }
                catch (Exception e)
                {
                    ProgramLog.Log(e, "Exception from command");
                }
            }
#endif
        }
    }
}
