using System;
using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        static void CommandStop(string[] args, Player author, Player target)
        {
            foreach (Player p in Server.players.Values.ToList())
            {
                Log(author, $"Disconnected {p.ID}");
                p.sendServerMessage("Server closing");
                p.disconnect(false);
            }

            Server.server.Stop();
            Server.running = false;

            if (Server.LOG_FILE) Server.Log("Logs", Server.LOG_FILE_PATH);
            Server.LOG_FILE_WRITER.Close();

            Environment.Exit(0);
        }
    }
}
