using System;
using System.Collections.Generic;
using OTA.Command;

namespace OTA.Commands
{
    public class CommandInfo : CommandDefinition<CommandInfo>
    {
        public CommandInfo(string prefix) : base(prefix)
        {

        }
    }

    public class CommandDefinition<T> : CommandDefinition where T : CommandDefinition
    {
        public CommandDefinition(string prefix) : base(prefix)
        {

        }

        /// <summary>
        /// Sets the description of the command.
        /// </summary>
        /// <returns>The description.</returns>
        /// <param name="desc">Desc.</param>
        public T WithDescription(string desc)
        {
            description = desc;
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets the help text of the command.
        /// </summary>
        /// <returns>The help text.</returns>
        /// <param name="help">Help.</param>
        public T WithHelpText(string help)
        {
            helpText.Add(help);
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
            this.node = node;
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets the permission node relative to the command name.
        /// </summary>
        /// <param name="code">Permission node key</param>
        public T ByPermissionNode(string code)
        {
            this.node = code + '.' + this._prefix;
            return (T)(CommandDefinition)this;
        }

        internal T WithPermissionNode()
        {
            this.node = ComponentNodePrefix + this._prefix;
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets the callback for the command
        /// </summary>
        /// <param name="callback">Callback.</param>
        public T Calls(Action<ISender, ArgumentList> callback)
        {
            tokenCallback = callback;
            return (T)(CommandDefinition)this;
        }

        /// <summary>
        /// Sets the callback for the command
        /// </summary>
        /// <param name="callback">Callback.</param>
        public T Calls(Action<ISender, string> callback)
        {
            stringCallback = callback;
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
        internal string description;
        internal List<string> helpText = new List<string>();
        internal string node;
        internal Action<ISender, ArgumentList> tokenCallback;
        internal Action<ISender, string> stringCallback;

        internal event Action<CommandDefinition> BeforeEvent;
        internal event Action<CommandDefinition> AfterEvent;

        internal string _prefix;
        internal bool _defaultHelp;
        internal bool _oldHelpStyle;

        internal NLua.LuaFunction LuaCallback;

        internal OTA.Plugin.BasePlugin Plugin { get; set; }

        internal bool running, paused;

        internal const String ComponentNodePrefix = "ota.";

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public string Prefix
        {
            get
            { return _prefix; }
        }

        /// <summary>
        /// Gets the permission node.
        /// </summary>
        /// <value>The node.</value>
        public string Node
        {
            get
            { return node; }
        }

        internal CommandDefinition(string prefix)
        {
            _prefix = prefix;
        }

        internal void InitFrom(CommandDefinition other)
        {
            description = other.description;
            helpText = other.helpText;
            tokenCallback = other.tokenCallback;
            stringCallback = other.stringCallback;
            LuaCallback = other.LuaCallback;
            ClearEvents();
        }

        internal void ClearCallbacks()
        {
            tokenCallback = null;
            stringCallback = null;
            LuaCallback = null;
        }

        /// <summary>
        /// Shows help message for a sender
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="noHelp">If set to <c>true</c> no help.</param>
        public void ShowHelp(ISender sender, bool noHelp = false)
        {
            if (helpText.Count == 0 && noHelp)
            {
                // Disabled this since it's not needed. There will usually be a description. But there should be some checks on if these are actually set, especially for plugins.
                //sender.SendMessage("No help text specified.");
                return;
            }

            if (!_oldHelpStyle)
            {
                const String Push = "       ";
                string command = (sender is Terraria.Player ? "/" : String.Empty) + _prefix;
                if (_defaultHelp)
                    sender.SendMessage("Usage: " + command);

                bool first = !_defaultHelp;
                foreach (var line in helpText)
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
                foreach (var line in helpText)
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
            for (var x = 0; x < padd - this._prefix.Length; x++)
                space += ' ';

            sender.SendMessage((sender is Terraria.Player ? "/" : String.Empty) + _prefix +
                space + " - " + (this.description ?? "No description specified")
            );
        }

        internal void Run(ISender sender, string args)
        {
            if (BeforeEvent != null)
                BeforeEvent(this);

            try
            {
                if (stringCallback != null)
                    stringCallback(sender, args);
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
                if (tokenCallback != null)
                    tokenCallback(sender, args);
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

