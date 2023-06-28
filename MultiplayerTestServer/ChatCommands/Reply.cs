namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdReply = new Command(
            "reply", 
            "Reply to last private message", 
            new string[] { "r" }, 
            new CommandArgument[] {
                new CommandArgument("message", false, CommandArgumentType.String)
            },

            delegate(string[] args, Player author, Player target)
            {
                string message = string.Join(" ", args).Trim();
                if (author != null)
                {
                    if (author.lastWhispered != null && !author.lastWhispered.Listening) author.lastWhispered = null;

                    target = author.lastWhispered;
                }
                else target = serverLastWhispered;

                if (target == null)
                {
                    Log(author, "No previous message found");
                    return;
                }

                if (author != null) author.sendPacket(Protocol.PacketType.PrivateChatMessage, $"{author.ID}>{target.ID}:{message}");
                else serverLastWhispered = target;

                target.sendPacket(Protocol.PacketType.PrivateChatMessage, $"{(author != null ? author.ID : "SERVER")}>{target.ID}:{message}");
                target.lastWhispered = author;

                Log(author, $"Whispered to [{target.ID}]: [{message}]", false);
            }, 
            adminOnly: false
        );
    }
}
