using System;
using System.Linq;

namespace MultiplayerTestServer
{
    partial class Commands
    {
        private static Command[] allCommands = {
            new Command("stop", "Stop the server", null, null, CommandStop, true),
            new Command("disconnect", "Disconnect a player", new string[]{ "kick" }, new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID),
                new CommandArgument("reason", true, CommandArgumentType.String)
            }, CommandDisconnect),
            new Command("disconnectall", "Disconnect all players", new string[]{ "kickall" }, new CommandArgument[] {
                new CommandArgument("reason", true, CommandArgumentType.String)
            }, CommandDisconnectAll),
            new Command("promote", "Promote a player to admin", new string[]{ "op" }, new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID)
            }, CommandPromote),
            new Command("demote", "Demote a player", new string[]{ "deop" }, new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID)
            }, CommandDemote),
            new Command("list", "List all players", new string[]{ "players" }, null, CommandList, false, false),
            new Command("mute", "Mute a player", null, new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID),
                new CommandArgument("reason", true, CommandArgumentType.String)
            }, CommandMute),
            new Command("unmute", "Unmute a player", null, new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID)
            }, CommandUnmute),
            new Command("clear", "Clear player messages", null, new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID)
            }, CommandClear),
            new Command("teleport", "Teleport a player", new string[]{ "tp" }, new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID),
                new CommandArgument("x", false, CommandArgumentType.Float),
                new CommandArgument("y", false, CommandArgumentType.Float)
            }, CommandTeleport),
            new Command("teleportall", "Teleport all players", new string[]{ "tpall" }, new CommandArgument[] {
                new CommandArgument("x", false, CommandArgumentType.Float),
                new CommandArgument("y", false, CommandArgumentType.Float)
            }, CommandTeleportAll),
            new Command("announce", "Announce a message", new string[]{ "broadcast" }, new CommandArgument[] {
                new CommandArgument("message", false, CommandArgumentType.String)
            }, CommandAnnounce),
            new Command("help", "Information on commands", null, new CommandArgument[] {
                new CommandArgument("command", true, CommandArgumentType.String)
            }, CommandHelp, false, false),
            new Command("randomteleport", "Randomly teleport a player", new string[]{ "rtp" }, new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID)
            }, CommandRandomTeleport),
            new Command("randomteleportall", "Randomly teleport all player", new string[]{ "rtpall" }, null, CommandRandomTeleportAll),
            new Command("whisper", "Send a private message", new string[]{ "w", "pm" }, new CommandArgument[] {
                new CommandArgument("player", false, CommandArgumentType.PlayerID),
                new CommandArgument("message", false, CommandArgumentType.String)
            }, CommandWhisper, false, false),
            new Command("reply", "Reply to last private message", new string[]{ "r" }, new CommandArgument[] {
                new CommandArgument("message", false, CommandArgumentType.String)
            }, CommandReply, false, false)
        };

        public static Command Get(string name)
        {
            name = name.ToLower();
            Command output = null;

            foreach (Command cmd in allCommands)
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

            foreach (Command cmd in allCommands)
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
