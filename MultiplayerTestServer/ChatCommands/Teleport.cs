namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandTeleport(string[] args, Player author, Player target)
        {
            float x = float.Parse(args[1]);
            float y = float.Parse(args[2]);

            target.Position[0] = x;
            target.Position[1] = y;
            if (!Server.newPositionPlayers.Contains(target)) Server.newPositionPlayers.Add(target);

            Log(author, $"Teleported [{target.ID}] to [{x},{y}]");
            target.sendServerMessage("You have been teleported");
        }
    }
}
