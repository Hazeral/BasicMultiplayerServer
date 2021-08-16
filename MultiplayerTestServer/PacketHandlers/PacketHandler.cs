using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerTestServer
{
    partial class PacketHandler
    {
        private static Dictionary<int, Action<Player, string>> Handlers = new Dictionary<int, Action<Player, string>>()
        {
            { (int)Protocol.PacketType.PlayerInput, PlayerInput },
            { (int)Protocol.PacketType.Ping, Ping },
            { (int)Protocol.PacketType.ChatMessage, ChatMessage }
        };

        public static Action<Player, string> GetHandler(int ID)
        {
            try
            {
                return Handlers.Where(h => h.Key == ID).First().Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
