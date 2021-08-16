using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandHelp(string[] args, Player author, Player target)
        {
            if (args.Length != 0)
            {
                Command cmd = Get(args[0]);
                if (cmd != null)
                {
                    Log(author, $"{(author != null ? "/" : "")}{cmd.Name}{(cmd.Aliases.Length != 0 ? $" [{string.Join(", ", cmd.Aliases)}]" : "")}{cmd.UsageText} - {cmd.Description}");
                } else
                {
                    Log(author, $"The command [{args[0]}] does not exist");
                }

                return;
            }

            Log(author, $"Help >\n" + string.Join("\n",
                allCommands.Select(
                    cmd => $"{(author != null ? "/" : "")}{cmd.Name}{(cmd.Aliases.Length != 0 ? $" [{string.Join(", ", cmd.Aliases)}]" : "")}{cmd.UsageText} - {cmd.Description}")
                ));
        }
    }
}