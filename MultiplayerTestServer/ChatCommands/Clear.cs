namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdClear = new Command(
            "clear", 
            "Clear player messages", 
            null, 
            new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID)
            },

            delegate(string[] args, Player author, Player target)
            {
                Server.broadcast(Protocol.PacketType.ClearMessages, target.ID);
                Log(author, $"Cleared messages from [{target.ID}]");
            }
        );
    }
}
