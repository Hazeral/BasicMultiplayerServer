namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdAnnounce = new Command(
            "announce", 
            "Announce a message", 
            new string[] { "broadcast" }, 
            new CommandArgument[] {
                new CommandArgument("message", false, CommandArgumentType.String)
            },

            delegate(string[] args, Player author, Player target)
            {
                string message = string.Join(" ", args);

                Server.broadcast(Protocol.PacketType.ServerMessage, message);
                Log(author, $"Announced message [{message}]", false);
            }
        );
    }
}
