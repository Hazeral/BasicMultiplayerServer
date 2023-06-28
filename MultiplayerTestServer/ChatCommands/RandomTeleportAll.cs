using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdRandomTeleportAll = new Command(
            "randomteleportall",
            "Randomly teleport all player",
            new string[] { "rtpall" },
            null,

            delegate(string[] args, Player author, Player target)
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
        );
    }
}
