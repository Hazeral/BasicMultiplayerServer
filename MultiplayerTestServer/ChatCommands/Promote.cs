namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandPromote(string[] args, Player author, Player target)
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
    }
}
