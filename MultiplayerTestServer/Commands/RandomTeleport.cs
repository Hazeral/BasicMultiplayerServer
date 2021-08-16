using System;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandRandomTeleport(string[] args, Player author, Player target)
        {
            Random r = new Random();
            int x = r.Next(Map.Bounds[0, 0], Map.Bounds[0, 1]);
            int y = r.Next(Map.Bounds[1, 0], Map.Bounds[1, 1]);

            target.Position[0] = x;
            target.Position[1] = y;
            if (!Server.newPositionPlayers.Contains(target)) Server.newPositionPlayers.Add(target);

            Log(author, $"Teleported [{target.ID}] to [{x},{y}]");
            target.sendServerMessage("You have been teleported");
        }
    }
}
