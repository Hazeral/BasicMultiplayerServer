using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandDisconnect(string[] args, Player author, Player target)
        {
            string reason = string.Join(" ", args.Skip(1)).Trim();

            if (reason.Length != 0)
            {
                target.sendServerMessage($"You have been kicked for: {reason}");
            }
            target.disconnect();
            Server.broadcast(PacketType.ServerMessage, $"Disconnected [{args[0]}] for [{reason}]");
            Log(author, $"Disconnected [{args[0]}] for [{reason}]", false);
        }
    }
}
