namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandClear(string[] args, Player author, Player target)
        {
            Server.broadcast(Protocol.PacketType.ClearMessages, target.ID);
            Log(author, $"Cleared messages from [{target.ID}]");
        }
    }
}
