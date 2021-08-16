using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandRandomTeleportAll(string[] args, Player author, Player target)
        {
            foreach (Player p in Server.players.Values.ToList())
            {
                p.Position = Map.RandomCoordinates();
            }

            Server.newPositionPlayers.Clear();
            Server.newPositionPlayers.AddRange(Server.players.Values.ToList());
            Server.broadcast(Protocol.PacketType.ServerMessage, "You have been teleported");

            Log(author, $"Randomly teleported all players");
        }
    }
}
