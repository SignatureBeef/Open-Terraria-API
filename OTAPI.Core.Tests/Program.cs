using System;

namespace OTAPI.Core.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Clear();

            Console.WriteLine("OTAPI Test Launcher.");
            Console.WriteLine("Menu.");

            var offset = 0;

            var wait = true;

            var menus = new[]
            {
                new
                {
                    Text = "Start server",
                    Execute = new Action(() => { wait=false; })
                },
                new
                {
                    Text = "Exit",
                    Execute = new Action(() => { Environment.Exit(0); })
                }
            };

            var startX = Console.CursorLeft;
            var startY = Console.CursorTop;

            for (var x = 0; x < menus.Length; x++)
            {
                Console.Write(x == offset ? "\t> " : "\t  ");
                Console.WriteLine(menus[x].Text);
            }

            while (wait)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.DownArrow)
                {
                    offset++;

                    if (offset >= menus.Length) offset = 0;
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    offset--;

                    if (offset < 0) offset = menus.Length - 1;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    menus[offset].Execute();
                }

                Console.CursorLeft = startX;
                Console.CursorTop = startY;
                for (var x = 0; x < menus.Length; x++)
                {
                    Console.Write(x == offset ? "\t> " : "\t  ");
                    Console.WriteLine(menus[x].Text);
                }
            }

            Terraria.WindowsLaunch.Main(args);
        }
    }
}
