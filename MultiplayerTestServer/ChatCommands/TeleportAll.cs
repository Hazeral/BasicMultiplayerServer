using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdTeleportAll = new Command(
            "teleportall", 
            "Teleport all players", 
            new string[] { "tpall" }, 
            new CommandArgument[] {
                new CommandArgument("x", false, CommandArgumentType.Float),
                new CommandArgument("y", false, CommandArgumentType.Float)
            },

            delegate(string[] args, Player author, Player target)
            {
                float x = float.Parse(args[0]);
                float y = float.Parse(args[1]);

                foreach (Player p in Server.players.Values.ToList())
                {
                    p.Position[0] = x;
                    p.Position[1] = y;
                }

                Server.newPositionPlayers.Clear();
                Server.newPositionPlayers.AddRange(Server.players.Values.ToList());
                Server.broadcast(Protocol.PacketType.ServerMessage, "You have been teleported");

                Log(author, $"Teleported all players to [{x},{y}]");
            }
        );
    }
}
