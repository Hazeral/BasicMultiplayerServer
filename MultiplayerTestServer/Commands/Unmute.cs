namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandUnmute(string[] args, Player author, Player target)
        {
            if (target.muted) {
                target.muted = false;

                target.sendServerMessage($"You have been unmuted");
                Log(author, $"Unmuted [{target.ID}]", false);
                Server.broadcast(PacketType.ServerMessage, $"Unmuted [{target.ID}]");
            } else
            {
                Log(author, $"[{target.ID}] is already unmuted");
            }
        }
    }
}