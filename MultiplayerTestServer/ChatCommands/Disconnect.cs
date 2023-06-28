using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdDisconnect = new Command(
            "disconnect",
            "Disconnect a player",
            new string[] { "kick" },
            new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID),
                new CommandArgument("reason", true, CommandArgumentType.String)
            },

            delegate(string[] args, Player author, Player target)
            {
                string reason = string.Join(" ", args.Skip(1)).Trim();

                if (reason.Length != 0)
                {
                    target.sendServerMessage($"You have been kicked for: {reason}");
                }
                target.disconnect();
                Server.broadcast(Protocol.PacketType.ServerMessage, $"Disconnected [{args[0]}] for [{reason}]");
                Log(author, $"Disconnected [{args[0]}] for [{reason}]", false);
            }
        );
    }
}
