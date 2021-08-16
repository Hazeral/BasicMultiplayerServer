using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandMute(string[] args, Player author, Player target)
        {
            string reason = string.Join(" ", args.Skip(1)).Trim();

            if (!target.muted) {
                target.muted = true;

                if (reason.Length != 0) target.sendServerMessage($"You have been muted for: {reason}");
                else target.sendServerMessage($"You have been muted");

                Log(author, $"Muted [{target.ID}] for [{reason}]", false);
                Server.broadcast(Protocol.PacketType.ServerMessage, $"Muted [{target.ID}] for [{reason}]");
            } else
            {
                Log(author, $"[{target.ID}] is already muted");
            }
        }
    }
}