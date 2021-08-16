namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandDemote(string[] args, Player author, Player target)
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
    }
}