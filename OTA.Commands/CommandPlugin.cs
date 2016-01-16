using OTA.Plugin;
using OTA.Logging;
using System;
using OTA.Commands.Events;
using OTA.Misc;

#if SERVER
using Terraria;
#endif

namespace OTA.Commands
{
    [OTAVersion(1, 0)]
    public class CommandPlugin : BasePlugin
    {
        public CommandPlugin()
        {
            this.Author = "DeathCradle";
            this.Description = "OTA Command system";
            this.Name = "OTA Commands";
            this.Version = "1b";
        }

        protected override void Enabled()
        {
            base.Enabled();

            OTACommand.Initialise(this);

            ProgramLog.Plugin.Log(this.Name + " initialised");
        }

        [Hook]
        void OnStartCommandProcessing(ref HookContext ctx, ref HookArgs.StartCommandProcessing args)
        {
            ctx.SetResult(HookResult.IGNORE);

            if (Console.IsInputRedirected)
            {
                ProgramLog.Admin.Log("Console input redirection has been detected.");
                return;
            }

            (new OTA.Misc.ProgramThread("Command", ListenForCommands)).Start();
        }

#if CLIENT
        [Hook]
        void OnGUIChatBoxSend(ref HookContext ctx, ref HookArgs.GUIChatBoxSend args)
        {
            if (CommandManager.Parser.ParsePlayerCommand(Terraria.Main.player[Terraria.Main.myPlayer], args.Message))
            {
                ctx.SetResult(HookResult.RECTIFY, false);
            }
        }
#elif SERVER
        [Hook(HookOrder.LATE)]
        void OnPlayerCommand(ref HookContext ctx, ref HookArgs.ReceiveNetMessage args)
        {
            if (args.PacketId == (int)Packet.PLAYER_CHAT)
            {
                ctx.SetResult(HookResult.IGNORE);
                try
                {
                    var buffer = NetMessage.buffer[args.BufferId];
                    var player = Terraria.Main.player[args.BufferId];

                    //Discard
                    buffer.reader.ReadByte();
                    var color = buffer.reader.ReadRGB();

                    var message = buffer.reader.ReadString();

                    if (!CommandManager.Parser.ParsePlayerCommand(player, message))
                    {
                        if (player.difficulty == 2)
                        {
                            color = Main.hcColor;
                        }
                        else if (player.difficulty == 1)
                        {
                            color = Main.mcColor;
                        }
                        NetMessage.SendData(25, -1, -1, message, args.BufferId, (float)color.R, (float)color.G, (float)color.B, 0, 0, 0);
                        if (Main.dedServ)
                        {
                            Logger.Vanilla("<" + player.name + "> " + message);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    ProgramLog.Log(e, "Failed to parse player chat");
                }
            }
        }
#endif

        private void ListenForCommands()
        {
            var ctx = new HookContext();
            var args = new CommandArgs.Listening();
            CommandEvents.Listening.Invoke(ref ctx, ref args);

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ProgramLog.Log("Ready for commands.");
            while (!Terraria.Netplay.disconnect /*|| Server.RestartInProgress*/)
            {
                try
                {
                    var ln = Console.ReadLine();
                    if (!String.IsNullOrEmpty(ln))
                    {
                        CommandManager.Parser.ParseConsoleCommand(ln);
                    }
                    else if (null == ln)
                    {
                        ProgramLog.Log("No console input available");
                        break;
                    }
                }
                catch (ExitException)
                {
                }
                catch (Exception e)
                {
                    ProgramLog.Log("Exception from command");
                    ProgramLog.Log(e);
                }
            }
        }

        [Hook]
        public void OnPluginReplacing(ref HookContext ctx, ref HookArgs.PluginReplacing args)
        {
            //Unregister the old commands for said plugin
            //The new one will register them again
            CommandManager.Parser.RemovePluginCommands(args.OldPlugin);
        }

        [Hook]
        public void OnPluginDisposed(ref HookContext ctx, ref HookArgs.PluginDisposed args)
        {
            CommandManager.Parser.RemovePluginCommands(args.Plugin);
        }

        [Hook]
        public void OnPluginPauseComplete(ref HookContext ctx, ref HookArgs.PluginPauseComplete args)
        {
            var def = CommandManager.Parser.GetPluginCommands(args.Plugin);
            if (null != def)
            {
                foreach (var cmd in def)
                    cmd.paused = false;
            }
        }

        [Hook]
        public void OnPluginPausing(ref HookContext ctx, ref HookArgs.PluginPausing args)
        {
            var def = CommandManager.Parser.GetPluginCommands(args.Plugin);
            if (null != def)
            {
                foreach (var cmd in def)
                {
                    cmd.paused = true; 
        
                    // wait for commands that may have already been running to finish
                    while (args.Plugin.HasRunningCommands())
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }
            }
        }
    }
}