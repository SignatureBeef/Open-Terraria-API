using System;
using System.Collections.Generic;
using OTA.Plugin;
using OTA.Commands.Events;
using OTA.Misc;
using System.Text;
using OTA.Command;
using System.Collections;
using System.Linq;

namespace OTA.Commands
{
    /// <summary>
    /// Functionality to process sender commands, and the default vanilla commands as an OTA version.
    /// </summary>
    public class CommandParser : CommandManager<CommandInfo>
    {
        public static ConsoleSender ConsoleSender { get; } = new ConsoleSender();

        public char PlayerCommandPrefix = '/';

        /// <summary>
        /// Parses new console command
        /// </summary>
        /// <param name="line">Command to parse</param>
        /// <param name="sender">Sending entity</param>
        public bool ParseConsoleCommand(string line, ConsoleSender sender = null)
        {
            line = line.Trim();

            if (sender == null)
                sender = ConsoleSender;

            return ParseAndProcess(sender, line);
        }

        /// <summary>
        /// Parses player commands
        /// </summary>
        /// <param name="player">Sending player</param>
        /// <param name="line">Command to parse</param>
        public bool ParsePlayerCommand(ISender player, string line, bool log = true)
        {
            if (!String.IsNullOrEmpty(line) && line[0] == PlayerCommandPrefix)
            {
                line = line.Remove(0, 1);

                if(log) 
                    OTA.Logging.ProgramLog.Log(player.SenderName + " sent command: " + line);

                return ParseAndProcess(player, line);
            }

            return false;
        }

        /// <summary>
        /// Parses and process a command from a sender
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="line">Line.</param>
        public bool ParseAndProcess(ISender sender, string line)
        {
            var ctx = new HookContext
            {
                Sender = sender,
//                Player = sender as Player
            };

            var hargs = new CommandArgs.CommandIssued();

            try
            {
                CommandInfo info;

                var firstSpace = line.IndexOf(' ');

                if (firstSpace < 0)
                    firstSpace = line.Length;

                var prefix = line.Substring(0, firstSpace).ToLower();

                hargs.Prefix = prefix;

                if (FindStringCommand(prefix, out info))
                {
                    hargs.ArgumentString = (firstSpace < line.Length - 1 ? line.Substring(firstSpace + 1, line.Length - firstSpace - 1) : "").Trim();

                    CommandEvents.CommandIssued.Invoke(ref ctx, ref hargs);

                    if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                        return true;

                    try
                    {
                        info.Run(sender, hargs.ArgumentString);
                    }
                    catch (NLua.Exceptions.LuaScriptException e)
                    {
                        if (e.IsNetException)
                        {
                            var ex = e.GetBaseException();
                            if (ex != null)
                            {
                                if (ex is CommandError)
                                {
                                    sender.SendMessage(prefix + ": " + ex.Message);
                                    info.ShowHelp(sender);
                                }
                            }
                        }
                    }
                    catch (ExitException e)
                    {
                        throw e;
                    }
                    catch (CommandError e)
                    {
                        sender.SendMessage(prefix + ": " + e.Message);
                        info.ShowHelp(sender);
                    }
                    return true;
                }

                var args = new ArgumentList();
                var command = Tokenize(line, args);

                if (command != null)
                {
                    if (FindTokenCommand(command, out info))
                    {
                        hargs.Arguments = args;
                        hargs.ArgumentString = args.ToString();

                        args.Plugin = info.Plugin;

                        CommandEvents.CommandIssued.Invoke(ref ctx, ref hargs);

                        if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                            return true;

                        try
                        {
                            info.Run(sender, hargs.Arguments);
                        }
                        catch (NLua.Exceptions.LuaScriptException e)
                        {
                            if (e.IsNetException)
                            {
                                var ex = e.GetBaseException();
                                if (ex != null)
                                {
                                    if (ex is CommandError)
                                    {
                                        sender.SendMessage(command + ": " + ex.Message);
                                        info.ShowHelp(sender);
                                    }
                                }
                            }
                        }
                        catch (ExitException e)
                        {
                            throw e;
                        }
                        catch (CommandError e)
                        {
                            sender.SendMessage(command + ": " + e.Message);
                            info.ShowHelp(sender);
                        }
                        return true;
                    }
                    else
                    {
                        sender.SendMessage(String.Format("No such command '{0}'.", command));
                    }
                }
            }
            catch (ExitException e)
            {
                throw e;
            }
            catch (TokenizerException e)
            {
                sender.SendMessage(e.Message);
            }
            return false;
        }

        class TokenizerException : Exception
        {
            public TokenizerException(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// Splits a command on spaces, with support for "parameters in quotes" and non-breaking\ spaces.
        /// Literal quotes need to be escaped like this: \"
        /// Literal backslashes need to escaped like this: \\
        /// Returns the first token
        /// </summary>
        /// <param name="command">Whole command line without trailing newline </param>
        /// <param name="args">An empty list to put the arguments in </param>
        public static string Tokenize(string command, List<string> args)
        {
            char l = '\0';
            var b = new StringBuilder();
            string result = null;
            int s = 0;

            foreach (char cc in command.Trim())
            {
                char c = cc;
                switch (s)
                {
                    case 0: // base state
                        {
                            if (c == '"' && l != '\\')
                                s = 1;
                            else if (c == ' ' && l != '\\' && b.Length > 0)
                            {
                                if (result == null)
                                    result = b.ToString();
                                else
                                    args.Add(b.ToString());
                                b.Length = 0;
                            }
                            else if ((c != '\\' && c != ' ') || l == '\\')
                            {
                                b.Append(c);
                                c = '\0';
                            }
                        }
                        break;

                    case 1: // inside quotes
                        {
                            if (c == '"' && l != '\\')
                                s = 0;
                            else if (c != '\\' || l == '\\')
                            {
                                b.Append(c);
                                c = '\0';
                            }
                        }
                        break;
                }
                l = c;
            }

            if (s == 1)
                throw new TokenizerException("Unmatched quote in command.");

            if (b.Length > 0)
            {
                if (result == null)
                    result = b.ToString();
                else
                    args.Add(b.ToString());
            }

            return result;
        }

        // for binary compatibility
        public static List<string> Tokenize(string command)
        {
            List<string> args = new List<string>();
            args.Insert(0, Tokenize(command, args));
            return args;
        }

        public bool FindStringCommand(string prefix, out CommandInfo info)
        {
            info = null;
        
            prefix = prefix.ToLower();

            if (commands.TryGetValue(prefix, out info))
            {
                if (info.Plugin.IsEnabled)
                    return info.stringCallback != null;
            }
        
            return false;
        }

        public bool FindTokenCommand(string prefix, out CommandInfo info)
        {
            info = null;
        
            prefix = prefix.ToLower();

            if (commands.TryGetValue(prefix, out info))
            {
                if (info.Plugin.IsEnabled)
                {
                    if ((info.tokenCallback != null || info.LuaCallback != null))
                        return true;
                }
            }
        
            return false;
        }

        internal CommandInfo FindOrCreate(string prefix)
        {
            if (commands.ContainsKey(prefix))
            {
                throw new ApplicationException("AddCommand: duplicate command: " + prefix);
            }

            var cmd = new CommandInfo(prefix);
            cmd.BeforeEvent += NotifyBeforeCommand;
            cmd.AfterEvent += NotifyAfterCommand;

            lock (commands)
            {
                commands.Add(prefix, cmd);
            }

            return cmd;
        }

        internal bool Remove(string prefix)
        {
            if (commands.ContainsKey(prefix))
            {
                var cmd = commands[prefix];

                cmd.BeforeEvent -= NotifyBeforeCommand;
                cmd.AfterEvent -= NotifyAfterCommand;

                cmd.ClearCallbacks();
                cmd.ClearEvents();

                lock (commands)
                {
                    commands.Remove(prefix);
                }
            }

            return false;
        }

        internal void RemovePluginCommands(BasePlugin plugin)
        {
            var commands = CommandManager.Parser.GetPluginCommands(plugin);
            foreach (var cmd in commands)
            {
                if (!CommandManager.Parser.Remove(cmd.Prefix))
                {
                    OTA.Logging.ProgramLog.Debug.Log($"Failed to clear command {cmd.Prefix}.");
                }
            }
        }

        internal IEnumerable<CommandInfo> GetPluginCommands(BasePlugin plugin = null)
        {
            return commands
                .Where(x => x.Value.Plugin != null && (plugin != null && x.Value.Plugin == plugin))
                .Select(x => x.Value);
        }

        public IEnumerable<CommandInfo> GetCommandsForSender(ISender sender)
        {
            return commands.Where(x => sender.HasPermission(x.Value.node)).Select(x => x.Value);
        }
    }
}

