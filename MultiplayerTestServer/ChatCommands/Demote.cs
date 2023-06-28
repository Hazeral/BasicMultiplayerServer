namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdDemote = new Command(
            "demote", 
            "Demote a player", 
            new string[] { "deop" }, 
            new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID)
            }, 
            
            delegate(string[] args, Player author, Player target)
            {
                if (target.admin)
                {
                    target.admin = false;
                    Log(author, $"Demoted [{target.ID}]");
                    target.sendServerMessage("You have been demoted");
                }
                else
                {
                    Log(author, $"[{target.ID}] is already not an admin");
                }
            }
        );
    }
}