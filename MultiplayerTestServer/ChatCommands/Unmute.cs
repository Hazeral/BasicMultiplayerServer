namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdUnmute = new Command(
            "unmute", 
            "Unmute a player", 
            null, 
            new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID)
            },

            delegate(string[] args, Player author, Player target)
            {
                if (target.muted)
                {
                    target.muted = false;

                    target.sendServerMessage($"You have been unmuted");
                    Log(author, $"Unmuted [{target.ID}]", false);
                    Server.broadcast(Protocol.PacketType.ServerMessage, $"Unmuted [{target.ID}]");
                }
                else
                {
                    Log(author, $"[{target.ID}] is already unmuted");
                }
            }
        );
    }
}