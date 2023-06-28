using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static Command cmdList = new Command(
            "list", 
            "List all players", 
            new string[] { "players" }, 
            null,

            delegate(string[] args, Player author, Player target)
            {
                Log(author, "Players >\n" + string.Join("\n", Server.players.Values.Select(p => $"> {p.ID}{(p.admin ? " (Admin)" : "")}{(author != null && !author.admin ? "" : $": [{p.Position[0]}, {p.Position[1]}]")}")));
            }, 
            adminOnly: false
        );
    }
}