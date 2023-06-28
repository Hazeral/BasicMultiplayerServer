using System.Linq;
using System.Numerics;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdHelp = new Command(
            "help", 
            "Information on commands", 
            null, 
            new CommandArgument[] {
                new CommandArgument("command", true, CommandArgumentType.String)
            },

            delegate(string[] args, Player author, Player target)
            {
                if (args.Length != 0)
                {
                    Command cmd = GetRestricted(author, args[0]);
                    if (cmd != null)
                    {
                        Log(author, $"{(author != null ? "/" : "")}{cmd.Name}{cmd.UsageText} - {cmd.Description}{(cmd.Aliases.Length != 0 ? $" [Aliases: {string.Join(", ", cmd.Aliases)}]" : "")}");
                    }
                    else
                    {
                        Log(author, $"The command [{args[0]}] does not exist");
                    }

                    return;
                }

                Command[] commands = null;

                if (author == null) commands = activeCommands;
                else
                {
                    if (author.admin) commands = activeCommands.Where(cmd => !cmd.ServerOnly).ToArray();
                    else commands = activeCommands.Where(cmd => !cmd.ServerOnly && !cmd.AdminOnly).ToArray();
                }

                if (commands == null)
                {
                    Log(author, "Help >\nNo commands available");
                    return;
                }

                Log(author, $"Help >\n" + string.Join("\n",
                    commands.Select(
                        cmd => $"{(author != null ? "/" : "")}{cmd.Name}{cmd.UsageText} - {cmd.Description}{(cmd.Aliases.Length != 0 ? $" [Aliases: {string.Join(", ", cmd.Aliases)}]" : "")}")
                    ));
            }, 
            adminOnly: false
        );
    }
}