using System;
using System.Collections.Generic;
using OTA.Plugin;
using OTA.Commands.Events;
using OTA.Misc;
using System.Text;
using OTA.Command;
using System.Collections;
using System.Linq;
using OTA.DebugFramework;

namespace OTA.Commands
{
    /// <summary>
    /// Functionality to process sender commands, and the default vanilla commands as an OTA version.
    /// </summary>
    public class CommandParser : CommandManager<CommandDefinition>
    {
        public static ConsoleSender ConsoleSender { get; } = new ConsoleSender();

        public char PlayerCommandPrefix = '/';


        ///// <summary>
        ///// Parses new console command
        ///// </summary>
        ///// <param name="line">Command to parse</param>
        ///// <param name="sender">Sending entity</param>
        //public bool ParseConsoleCommand(string line, ConsoleSender sender = null)
        //{
        //    line = line.Trim();

        //    if (sender == null)
        //        sender = ConsoleSender;

        //    return ParseAndProcess(sender, line);
        //}

        ///// <summary>
        ///// Parses player commands
        ///// </summary>
        ///// <param name="player">Sending player</param>
        ///// <param name="line">Command to parse</param>
        //public bool ParsePlayerCommand(ISender player, string line, bool log = true)
        //{
        //    if (!String.IsNullOrEmpty(line) && line[0] == PlayerCommandPrefix)
        //    {
        //        line = line.Remove(0, 1);

        //        if (log)
        //            OTA.Logging.ProgramLog.Log(player.SenderName + " sent command: " + line);

        //        ParseAndProcess(player, line);
        //        return true;
        //    }

        //    return false;
        //}


        /// <summary>
        /// Parses and process a command from a sender
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="text">text.</param>
        public bool ParseAndProcess(ISender sender, string text)
        {
            if (sender == null)
                throw new System.ArgumentException("sender cannot me null", "original");
            if (text == null)
                throw new System.ArgumentException("Parameter cannot be null", "original");

            text = text.Trim();
            if (text.Length == 0)
                throw new System.ArgumentException("Parameter cannot be null", "original");

            if (text[0] == PlayerCommandPrefix)
                text = text.Remove(0, 1);
            else if (sender is BasePlayer) return false;

            var ctx = new HookContext
            {
                Sender = sender,
                //                Player = sender as Player
            };

            var hargs = new CommandArgs.CommandIssued()
            {
                Prefix = text
            };

            try
            {
                CommandDefinition info;

                var firstSpace = text.IndexOf(' ');
                if (firstSpace > -1)
                    hargs.Prefix = text.Substring(0, firstSpace);

                var args = TokenizeArguments(text);

                if (FindCommand(args, out info))
                {
                    hargs.Arguments = args;
                    hargs.ArgumentString = args.ToString();

                    args.Plugin = info.Plugin;

                    CommandEvents.CommandIssued.Invoke(ref ctx, ref hargs);

                    if (ctx.CheckForKick() || ctx.Result == HookResult.IGNORE)
                        return true;

                    if (ctx.Result != HookResult.CONTINUE && OTA.Permissions.Permissions.GetPermission(sender, info.Node) != Permissions.Permission.Permitted)
                    {
                        sender.SendMessage("Access denied.", G: 0, B: 0);
                        return true;
                    }

                    try
                    {
                        if (info._tokenCallback != null)
                            info.Run(sender, hargs.Arguments);
                        else if (info._stringCallback != null)
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
                                    sender.SendMessage(hargs.Prefix + ": " + ex.Message);
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
                        sender.SendMessage(hargs.Prefix + ": " + e.Message);
                        info.ShowHelp(sender);
                    }
                    return true;
                }
                else
                {
                    sender.SendMessage(String.Format("No such command '{0}'.", hargs.Prefix));
                    return true;
                }
            }
            catch (ExitException e)
            {
                throw e;
            }
            catch (TokenizerException e)
            {
                sender.SendMessage(e.Message);
                return true;
            }
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
        public static void Tokenize(string command, Action<string> segmentCallback)
        {
            char l = '\0';
            var b = new StringBuilder();
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
                                segmentCallback(b.ToString());
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
                segmentCallback(b.ToString());
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
        public static ArgumentList TokenizeArguments(string command)
        {
            var args = new ArgumentList();

            Tokenize(command, (segment) =>
            {
                args.Add(segment);
            });

            return args;
        }

        // for binary compatibility
        public static List<string> Tokenize(string command)
        {
            return (List<string>)TokenizeArguments(command);
        }

        public bool FindCommand(ArgumentList args, out CommandDefinition info)
        {
            info = null;

            string part = null;
            string node = null;
            CommandDefinition cur = null;

            while (args.Count > 0 && (part = args.GetString(0)) != null)
            {
                if (node == null) node = part;
                else if (part != null) node += '.' + part;

                if (FindCommand(node, out cur))
                {
                    info = cur;
                    args.RemoveRange(0, 1);
                }
                else break;
            }

            return info != null;
        }

        public bool FindCommand(string prefix, out CommandDefinition info)
        {
            info = null;

            prefix = prefix.ToLower();

            if (commands.TryGetValue(prefix, out info))
            {
                if (info.Plugin.IsEnabled)
                {
                    if (info._tokenCallback != null || info._stringCallback != null || info.LuaCallback != null)
                        return true;
                }
            }

            return false;
        }

        internal T FindOrCreate<T>(string[] aliases, CommandDefinition parent = null) where T : CommandDefinition
        {
            T cmd = null;
            foreach (var alias in aliases)
            {
                var key = alias;
                if (parent != null) key = $"{parent.DefaultAlias}.{alias}";
                if (commands.ContainsKey(key))
                {
                    if (!Remove(key))
                        throw new ApplicationException("AddCommand: failed to replace command: " + key);
                }

                if (cmd == null)
                {
                    cmd = (T)Activator.CreateInstance(typeof(T), new object[] { aliases });
                    cmd.BeforeEvent += NotifyBeforeCommand;
                    cmd.AfterEvent += NotifyAfterCommand;
                    cmd._parent = parent;
                }

                lock (commands)
                {
                    if (parent != null)
                    {
                        if (parent._children == null)
                            parent._children = new Dictionary<string, CommandDefinition>();

                        parent._children.Add(alias, cmd);
                    }
                    commands.Add(key, cmd);
                }
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
                    return commands.Remove(prefix);
                }
            }

            return false;
        }

        internal void RemovePluginCommands(BasePlugin plugin)
        {
            var commands = CommandManager.Parser.GetPluginCommands(plugin);
            foreach (var cmd in commands)
            {
                foreach (var alias in cmd.Aliases)
                {
                    var key = alias;
                    if (cmd.Parent != null) key = $"{cmd.Parent.DefaultAlias}.{alias}";
                    if (!CommandManager.Parser.Remove(key))
                    {
                        OTA.Logging.ProgramLog.Debug.Log($"Failed to clear command {key}.");
                    }
                }
            }
        }

        internal IEnumerable<CommandDefinition> GetPluginCommands(BasePlugin plugin = null)
        {
            return commands
                .Where(x => x.Value.Plugin != null && (plugin != null && x.Value.Plugin == plugin))
                .Select(x => x.Value);
        }

        public IEnumerable<CommandDefinition> GetCommandsForSender(ISender sender)
        {
            return commands.Where(x => sender.HasPermission(x.Value._node)).Select(x => x.Value);
        }
    }
}

