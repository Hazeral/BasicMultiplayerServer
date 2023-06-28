namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdTeleport = new Command(
            "teleport", 
            "Teleport a player", 
            new string[] { "tp" }, 
            new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID),
                new CommandArgument("x", false, CommandArgumentType.Float),
                new CommandArgument("y", false, CommandArgumentType.Float)
            },

            delegate(string[] args, Player author, Player target)
            {
                float x = float.Parse(args[1]);
                float y = float.Parse(args[2]);

                target.Position[0] = x;
                target.Position[1] = y;
                if (!Server.newPositionPlayers.Contains(target)) Server.newPositionPlayers.Add(target);

                Log(author, $"Teleported [{target.ID}] to [{x},{y}]");
                target.sendServerMessage("You have been teleported");
            }
        );
    }
}
