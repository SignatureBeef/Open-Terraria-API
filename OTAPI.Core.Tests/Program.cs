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

            Listen();

            Terraria.WindowsLaunch.Main(args);
        }

        static void Listen()
        {
            Hooks.Command.StartCommandThread = () =>
            {
                Console.WriteLine("[Hook] Command thread.");
                return HookResult.Continue;
            };
            //Hooks.Net.SendData =
            //(
            //    ref int bufferIndex,
            //    ref int msgType,
            //    ref int remoteClient,
            //    ref int ignoreClient,
            //    ref string text,
            //    ref int number,
            //    ref float number2,
            //    ref float number3,
            //    ref float number4,
            //    ref int number5,
            //    ref int number6,
            //    ref int number7
            //) =>
            //{
            //    //Console.WriteLine($"Sending {msgType} to {bufferIndex}");
            //    return HookResult.Continue;
            //};
            //Hooks.Net.ReceiveData =
            //(
            //    global::Terraria.MessageBuffer buffer,
            //    ref byte packetId,
            //    ref int readOffset,
            //    ref int start,
            //    ref int length,
            //    ref int messageType
            //) =>
            //{
            //    //Console.WriteLine($"Receiving {packetId} from {buffer.whoAmI}");
            //    return HookResult.Continue;
            //};
            Hooks.Net.PreGreetPlayer = (ref int playerId) =>
            {
                Console.WriteLine(nameof(Hooks.Net.PreGreetPlayer) + " " + playerId);
                return HookResult.Continue;
            };
            Hooks.Net.PostGreetPlayer = (int playerId) =>
            {
                Console.WriteLine(nameof(Hooks.Net.PostGreetPlayer) + " " + playerId);
                return null;
            };
            //Hooks.Net.SendBytes = (
            //    ref int remoteClient,
            //    ref byte[] data,
            //    ref int offset,
            //    ref int size,
            //    ref global::Terraria.Net.Sockets.SocketSendCallback callback,
            //    ref object state) =>
            //{
            //    Console.WriteLine($"Sending {size} bytes of data to {remoteClient}");
            //    return HookResult.Continue;
            //};
            Hooks.Net.NameCollision = (player) =>
            {
                Console.WriteLine($"Booting {player.name} as their name conflicts with an existing player.");
                return HookResult.Continue;
            };
            Hooks.Npc.Strike = (
                global::Terraria.NPC npc,
                ref int cancelResult,
                ref int Damage,
                ref float knockBack,
                ref int hitDirection,
                ref bool crit,
                ref bool noEffect,
                ref bool fromNet) =>
            {
                cancelResult = 0;
                Console.WriteLine($"Hitting npc at {npc.whoAmI} with: {Damage},{knockBack},{hitDirection},{crit},{noEffect},{fromNet}");
                return HookResult.Continue;
            };
        }
    }
}
