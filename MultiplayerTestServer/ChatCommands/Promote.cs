namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdPromote = new Command(
            "promote", 
            "Promote a player to admin", 
            new string[] { "op" }, 
            new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID)
            }, 
            
            delegate(string[] args, Player author, Player target)
            {
                if (!target.admin)
                {
                    target.admin = true;
                    Log(author, $"Promoted [{target.ID}]");
                    target.sendServerMessage("You have been promoted");
                }
                else
                {
                    Log(author, $"[{target.ID}] is already an admin");
                }
            }
        );
    }
}
