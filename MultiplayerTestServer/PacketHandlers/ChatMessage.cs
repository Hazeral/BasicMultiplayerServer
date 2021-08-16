using System.Linq;
using System.Text.RegularExpressions;

namespace MultiplayerTestServer
{
    partial class PacketHandler
    {
        public static void ChatMessage(Player player, string data)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -/.]");
            string input = rgx.Replace(data.Trim(), string.Empty);
            if (input.Length != 0)
            {
                player.Log("Chat message", input);
                if (player.muted)
                {
                    player.sendServerMessage("You are muted");
                    return;
                }
                if (input.StartsWith('/'))
                {
                    if (player.admin)
                    {
                        string command = input.Split()[0].Replace("/", "");
                        string[] cmdArgs = input.Split().Skip(1).ToArray();

                        Command cmd = Commands.Get(command);
                        if (cmd != null) cmd.Run(cmdArgs, player);
                    }
                }
                else Server.broadcast(Protocol.PacketType.ChatMessage, $"{player.ID}:{input}");
            }
        }
    }
}
