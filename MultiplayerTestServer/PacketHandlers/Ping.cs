using System;

namespace MultiplayerTestServer
{
    partial class PacketHandler
    {
        public static void Ping(Player player, string data)
        {
            player.lastPing = Server.ConvertToUnixTimestamp(DateTime.Now);
            player.sendPacket(Protocol.PacketType.Ping, "");
        }
    }
}
