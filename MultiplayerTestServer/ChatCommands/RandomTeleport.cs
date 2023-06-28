namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdRandomTeleport = new Command(
            "randomteleport", 
            "Randomly teleport a player", 
            new string[] { "rtp" }, 
            new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID)
            },

            delegate(string[] args, Player author, Player target)
            {
                target.Position = Map.RandomCoordinates();
                if (!Server.newPositionPlayers.Contains(target)) Server.newPositionPlayers.Add(target);

                Log(author, $"Teleported [{target.ID}] to [{target.Position[0]},{target.Position[1]}]");
                target.sendServerMessage("You have been teleported");
            }
        );
    }
}
