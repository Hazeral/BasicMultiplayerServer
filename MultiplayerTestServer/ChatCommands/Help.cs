using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandHelp(string[] args, Player author, Player target)
        {
            if (args.Length != 0)
            {
                Command cmd = GetRestricted(author, args[0]);
                if (cmd != null)
                {
                    Log(author, $"{(author != null ? "/" : "")}{cmd.Name}{cmd.UsageText} - {cmd.Description}{(cmd.Aliases.Length != 0 ? $" [Aliases: {string.Join(", ", cmd.Aliases)}]" : "")}");
                } else
                {
                    Log(author, $"The command [{args[0]}] does not exist");
                }

                return;
            }

            Command[] commands = GetAllCommandsRestricted(author);

            if (commands == null)
            {
                Log(author, "Help >\nNo commands available");
                return;
            }

            Log(author, $"Help >\n" + string.Join("\n",
                commands.Select(
                    cmd => $"{(author != null ? "/" : "")}{cmd.Name}{cmd.UsageText} - {cmd.Description}{(cmd.Aliases.Length != 0 ? $" [Aliases: {string.Join(", ", cmd.Aliases)}]" : "")}")
                ));
        }

        static Command[] GetAllCommandsRestricted(Player player)
        {
            Command[] output = null;
            if (player == null) output = allCommands;
            else
            {
                if (player.admin) output = allCommands.Where(cmd => !cmd.ServerOnly).ToArray();
                else output = allCommands.Where(cmd => !cmd.ServerOnly && !cmd.AdminOnly).ToArray();
            }

            return output;
        }
    }
}