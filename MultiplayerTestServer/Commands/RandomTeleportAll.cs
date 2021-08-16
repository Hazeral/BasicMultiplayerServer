using System;
using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandRandomTeleportAll(string[] args, Player author, Player target)
        {
            Random r = new Random();

            foreach (Player p in Server.players.Values.ToList())
            {
                int x = r.Next(Map.Bounds[0, 0], Map.Bounds[0, 1]);
                int y = r.Next(Map.Bounds[1, 0], Map.Bounds[1, 1]);

                p.Position[0] = x;
                p.Position[1] = y;
            }

            Server.newPositionPlayers.Clear();
            Server.newPositionPlayers.AddRange(Server.players.Values.ToList());
            Server.broadcast(PacketType.ServerMessage, "You have been teleported");

            Log(author, $"Randomly teleported all players");
        }
    }
}
