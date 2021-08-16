using System.Collections.Generic;

namespace MultiplayerTestServer
{
    partial class PacketHandler
    {
        public static void PlayerInput(Player player, string data)
        {
            string[] info = data.Split(':');
            float delta = float.Parse(info[0]);
            List<char> keys = new List<char>(info[1].ToCharArray());

            if (keys.Contains('W'))
            {
                player.Position[1] += 1 * delta;
            }
            if (keys.Contains('A'))
            {
                player.Position[0] -= 1 * delta;
            }
            if (keys.Contains('S'))
            {
                player.Position[1] -= 1 * delta;
            }
            if (keys.Contains('D'))
            {
                player.Position[0] += 1 * delta;
            }

            if (!Server.newPositionPlayers.Contains(player)) Server.newPositionPlayers.Add(player);
            player.Log("Updated position", $"{player.Position[0]}, {player.Position[1]}");
        }
    }
}
