using System;
using System.Collections.Generic;
using OTA.Command;

namespace OTA.Commands
{
    public class CommandInfo : CommandDefinition<CommandInfo>
    {
        public CommandInfo(string[] aliases) : base(aliases)
        {

        }
    }

    public class CommandDefinition<T> : CommandDefinition where T : CommandDefinition
    {
        public CommandDefinition(string[] aliases) : base(aliases)
        {

        }

        public T SubCommand(params string[] aliases)
        {
            var cmd = CommandManager.Parser.FindOrCreate<T>(aliases, this);
            cmd.Plugin = this.Plugin;

            return cmd;
        }

        /// <summary>
        /// Sets the description of the command.
        /// </summary>
        /// <returns>The description.</returns>
        /// <param name="desc">Desc.</param>
        public T WithDescription(string desc)
        {
            _description = desc;
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets the help text of the command.
        /// </summary>
        /// <returns>The help text.</returns>
        /// <param name="help">Help.</param>
        public T WithHelpText(string help)
        {
            _helpText.Add(help);
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets a flag for OTA to generate a default usage based on the prefix when a sender queries how to use the command.
        /// </summary>
        /// <returns>The default usage.</returns>
        public T SetDefaultUsage()
        {
            _defaultHelp = true;
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets the flag that the output help text must be in the pre TDSM Rebind format
        /// </summary>
        /// <returns>The old help style.</returns>
        public T SetOldHelpStyle()
        {
            _oldHelpStyle = true;
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets the permission ndoe for this command
        /// </summary>
        /// <returns>The permission node.</returns>
        /// <param name="node">Node.</param>
        public T WithPermissionNode(string node)
        {
            this._node = node;
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets the callback for the command
        /// </summary>
        /// <param name="callback">Callback.</param>
        public T Calls(Action<ISender, ArgumentList> callback)
        {
            _tokenCallback = callback;
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets the callback for the command
        /// </summary>
        /// <param name="callback">Callback.</param>
        public T Calls(Action<ISender, string> callback)
        {
            _stringCallback = callback;
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets a LUA callback
        /// </summary>
        /// <returns>The call.</returns>
        /// <param name="callback">Callback.</param>
        public T LuaCall(NLua.LuaFunction callback)
        {
            LuaCallback = callback;
            return (T)(CommandDefinition)this;
        }
    }

    /// <summary>
    /// An OTA command definition
    /// </summary>
    public abstract class CommandDefinition
    {
        internal string _description;
        internal List<string> _helpText = new List<string>();
        internal string _node;
        internal Action<ISender, ArgumentList> _tokenCallback;
        internal Action<ISender, string> _stringCallback;

        internal event Action<CommandDefinition> BeforeEvent;
        internal event Action<CommandDefinition> AfterEvent;

        internal string[] _aliases;
        internal bool _defaultHelp;
        internal bool _oldHelpStyle;

        internal NLua.LuaFunction LuaCallback;

        internal OTA.Plugin.BasePlugin Plugin { get; set; }

        internal bool _running, _paused;

        internal CommandDefinition _parent;
        internal Dictionary<string, CommandDefinition> _children;

        /// <summary>
        /// The parent command if this instance is a child.
        /// </summary>
        public CommandDefinition Parent
        {
            get
            { return _parent; }
            protected set
            { _parent = value; }
        }

        /// <summary>
        /// Command aliases.
        /// </summary>
        /// <value>The prefix.</value>
        public string[] Aliases
        {
            get
            { return _aliases; }
        }

        /// <summary>
        /// Gets the default alias.
        /// </summary>
        public string DefaultAlias
        {
            get
            { return _aliases[0]; }
        }

        /// <summary>
        /// Gets the permission node.
        /// </summary>
        /// <value>The node.</value>
        public string Node
        {
            get
            { return _node; }
        }

        internal CommandDefinition(string[] aliases)
        {
            this._aliases = aliases;
        }

        internal void InitFrom(CommandDefinition other)
        {
            _description = other._description;
            _helpText = other._helpText;
            _tokenCallback = other._tokenCallback;
            _stringCallback = other._stringCallback;
            LuaCallback = other.LuaCallback;
            ClearEvents();
        }

        internal void ClearCallbacks()
        {
            _tokenCallback = null;
            _stringCallback = null;
            LuaCallback = null;
        }

        /// <summary>
        /// Shows help message for a sender
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="noHelp">If set to <c>true</c> no help.</param>
        public void ShowHelp(ISender sender, bool noHelp = false)
        {
            if (_helpText.Count == 0 && noHelp)
            {
                // Disabled this since it's not needed. There will usually be a description. But there should be some checks on if these are actually set, especially for plugins.
                //sender.SendMessage("No help text specified.");
                return;
            }

            if (!_oldHelpStyle)
            {
                const String Push = "       ";
                string command = (sender is Terraria.Player ? "/" : String.Empty) + this.DefaultAlias;
                if (_defaultHelp)
                    sender.SendMessage("Usage: " + command);

                bool first = !_defaultHelp;
                foreach (var line in _helpText)
                {
                    if (first)
                    {
                        first = false;
                        sender.SendMessage("Usage: " + command + " " + line);
                    }
                    else
                    {
                        sender.SendMessage(Push + command + " " + line);
                    }
                }
            }
            else
            {
                foreach (var line in _helpText)
                    sender.SendMessage(line);
            }
        }

        /// <summary>
        /// Shows the description to a sender
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="padd">Padd.</param>
        public void ShowDescription(ISender sender, int padd)
        {
            var space = String.Empty;
            for (var x = 0; x < padd - this.DefaultAlias.Length; x++)
                space += ' ';

            sender.SendMessage((sender is Terraria.Player ? "/" : String.Empty) + this.DefaultAlias +
                space + " - " + (this._description ?? "No description specified")
            );
        }

        internal void Run(ISender sender, string args)
        {
            if (BeforeEvent != null)
                BeforeEvent(this);

            try
            {
                if (_stringCallback != null)
                    _stringCallback(sender, args);
                else if (LuaCallback != null)
                    LuaCallback.Call(this, sender, args);
                else
                    sender.SendMessage("This command is no longer available", 255, 238, 130, 238);
            }
            finally
            {
                if (AfterEvent != null)
                    AfterEvent(this);
            }
        }

        internal void Run(ISender sender, ArgumentList args)
        {
            if (BeforeEvent != null)
                BeforeEvent(this);

            try
            {
                if (_tokenCallback != null)
                    _tokenCallback(sender, args);
                else if (LuaCallback != null)
                    LuaCallback.Call(this, sender, args);
                else
                    sender.SendMessage("This command is no longer available", 255, 238, 130, 238);
            }
            finally
            {
                if (AfterEvent != null)
                    AfterEvent(this);
            }
        }

        internal void ClearEvents()
        {
            AfterEvent = null;
            BeforeEvent = null;
        }
    }
}

