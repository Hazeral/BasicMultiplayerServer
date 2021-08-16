using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandDisconnectAll(string[] args, Player author, Player target)
        {
            string reason = string.Join(" ", args).Trim();
            int playersKicked = 0;

            foreach (Player p in Server.players.Values.ToList())
            {
                if (p == author) continue;

                if (reason.Length != 0)
                {
                    p.sendServerMessage($"You have been kicked for: {reason}");
                }
                p.disconnect();
                playersKicked++;
            }

            Log(author, $"Disconnected {playersKicked} players");
        }
    }
}
