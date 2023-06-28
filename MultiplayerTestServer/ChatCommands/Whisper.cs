using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        public static Player serverLastWhispered = null;  // The player last whispered to through the console

        static Command cmdWhisper = new Command(
            "whisper", 
            "Send a private message", 
            new string[] { "w", "pm" }, new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID),
                new CommandArgument("message", false, CommandArgumentType.String)
            },

            delegate(string[] args, Player author, Player target)
            {
                string message = string.Join(" ", args.Skip(1)).Trim();

                if (author != null && author == target)
                {
                    Log(author, "You cannot whisper to yourself");
                    return;
                }

                if (author != null)
                {
                    author.sendPacket(Protocol.PacketType.PrivateChatMessage, $"{author.ID}>{target.ID}:{message}");
                    author.lastWhispered = target;
                }
                else serverLastWhispered = target;
                target.sendPacket(Protocol.PacketType.PrivateChatMessage, $"{(author != null ? author.ID : "SERVER")}>{target.ID}:{message}");
                target.lastWhispered = author;

                Log(author, $"Whispered to [{args[0]}]: [{message}]", false);
            }, 
            adminOnly: false
        );
    }
}
