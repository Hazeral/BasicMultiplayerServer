namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandAnnounce(string[] args, Player author, Player target)
        {
            string message = string.Join(" ", args);

            Server.broadcast(Protocol.PacketType.ServerMessage, message);
            Log(author, $"Announced message [{message}]", false);
        }
    }
}
