using System;
using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        private static Command[] activeCommands = {
            cmdStop,
            cmdDisconnect,
            cmdDisconnectAll,
            cmdPromote,
            cmdDemote,
            cmdList,
            cmdMute,
            cmdUnmute,
            cmdClear,
            cmdTeleport,
            cmdTeleportAll,
            cmdAnnounce,
            cmdHelp,
            cmdRandomTeleport,
            cmdRandomTeleportAll,
            cmdWhisper,
            cmdReply
        };

        public static Command Get(string name)
        {
            name = name.ToLower();
            Command output = null;

            foreach (Command cmd in activeCommands)
            {
                if (cmd.Name == name || cmd.Aliases.Contains(name))
                {
                    output = cmd;
                    break;
                }
            }

            return output;
        }

        public static Command GetRestricted(Player player, string name)
        {
            name = name.ToLower();
            Command output = null;

            foreach (Command cmd in activeCommands)
            {
                if (cmd.Name == name || cmd.Aliases.Contains(name))
                {
                    if (player != null)
                    {
                        if (cmd.ServerOnly) break;
                        if (cmd.AdminOnly && !player.admin) break;
                    }

                    output = cmd;
                    break;
                }
            }

            return output;
        }

        public static void Log(Player player, string message, bool informPlayer = true)
        {
            if (player == null)
            {
                Server.Log("Command", message);
            }
            else
            {
                player.Log("Command", message);
                if (informPlayer) player.sendServerMessage(message);
            }
        }
    }
}
