using System;
using System.Linq;
using OTA.Command;
using System.Collections.Generic;

namespace OTA.Commands
{
    public class HelpCommand : OTACommand
    {
        public override void Initialise()
        {
            AddCommand("help")
                .WithDescription("Displays the commands available to the user.")
                .WithPermissionNode("ota.help")
                .Calls(ShowHelp);
        }

        public void ShowHelp(ISender sender, ArgumentList args)
        {
            var commands = CommandManager.Parser.GetCommandsForSender(sender).ToList();
            if (commands != null && commands.Count > 0)
            {
                int page = 0;
                if (!args.TryGetInt(0, out page))
                {
                    if (args.Count > 0)
                    {
                        var command = args.GetString(0).ToLower();
                        var cmd = commands.SingleOrDefault(x => x.Aliases.Contains(command));
                        if (cmd != null)
                        {
                            sender.SendMessage(cmd._description);
                            cmd.ShowHelp(sender, true);
                            return;
                        }
                        else
                            throw new CommandError("No such command: " + command);
                    }
                }
                else
                {
                    page--;
                }

                //              const Int32 MaxLines = 5;
                var maxLines = sender is BasePlayer ? 5 : 15;
                var lineOffset = page * maxLines;
                var maxPages = (int)Math.Ceiling(commands.Count / (double)maxLines);

                if (page >= 0 && page < maxPages)
                {
                    var cmds = new List<CommandDefinition>();
                    var sorted = commands
                        .OrderBy(x => x.DefaultAlias)
                        .ToArray();
                    for (var i = lineOffset; i < lineOffset + maxLines; i++)
                    {
                        if (i < sorted.Length)
                            cmds.Add(sorted[i]);
                    }

                    var prefixMax = cmds
                        .Select(x => x.DefaultAlias.Length)
                        .OrderByDescending(x => x)
                        .First();
                    foreach (var cmd in cmds)
                        cmd.ShowDescription(sender, prefixMax);

                    sender.SendMessage(String.Format("[Page {0} / {1}]", page + 1, maxPages));
                }
                else
                {
                    sender.SendMessage("Usage:");
                    sender.SendMessage("    help <command> - Get help for a command.");
                    sender.SendMessage("    help <page> - View a list of commands. Valid page numbers are 1 to " + maxPages + ".");
                    sender.SendMessage("Examples:");
                    sender.SendMessage("    help motd");
                    sender.SendMessage("    help 1");
                }
            }
            else
            {
                sender.SendMessage("You have no available commands.");
            }
        }
    }
}

