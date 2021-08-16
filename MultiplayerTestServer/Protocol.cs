using System;

namespace MultiplayerTestServer
{
    class Protocol
    {
        public enum PacketType
        {
            PlayerWelcome,
            PlayerJoin,
            PositionsUpdate,
            PlayerInput,
            PlayerDisconnect,
            Ping,
            ChatMessage,
            ServerMessage,
            ClearMessages
        }

        public static char PayloadSeparator = '$';
        public static char PacketSuffix = '~';

        public static string ConstructPacket(PacketType type, string payload)
        {
            return $"{(int)type}{PayloadSeparator}{payload}{PacketSuffix}";
        }

        public static string GetPacket(Player player)
        {
            try
            {
                string packet = "";
                bool reading = true;

                while (reading)
                {
                    int currentByte = player.Stream.ReadByte();
                    char byteToChar = DecryptChar(player, System.Text.Encoding.ASCII.GetString(new[] { (byte)currentByte }).ToCharArray()[0], packet.Length);

                    if (currentByte == -1 || byteToChar == PacketSuffix) reading = false;
                    else
                    {
                        packet += byteToChar;
                    }
                }

                if (packet == "") throw new Exception("Empty packet");
                if (Server.verboseLogs) player.Log("Received packet", packet);
                return packet;
            }
            catch (Exception eX)
            {
                player.Log("Socket error", $"Error reading packets, {(eX.Message.Contains("WSACancelBlockingCall") ? "WSACancelBlockingCall" : eX.Message)}");
                if (eX.Message.Contains("An existing connection was forcibly closed by the remote host") ||
                    eX.Message.Contains("An established connection was aborted by the software in your host machine") ||
                    eX.Message.Contains("Empty packet"))
                {
                    player.Log("Status", "Client has disconnected");
                    player.disconnect();
                }
            }

            return "";
        }

        public static string EncryptData(Player player, string data)
        {
            if (player.encrypt)
            {
                char[] output = data.ToCharArray();

                for (int i = 0; i < data.ToCharArray().Length; i++)
                {
                    output[i] = (char)(data.ToCharArray()[i] ^ player.encryptionKey[i % (player.encryptionKey.Length / sizeof(char))]);
                }

                return new string(output);
            }
            if (Server.encrypted) player.encrypt = true; // start encrypting only after the first packet (welcome packet incl. encryption key)

            return data;
        }

        public static char DecryptChar(Player player, char data, int length)
        {
            if (player.encrypt)
            {
                char output = (char)(data ^ player.encryptionKey[length % (player.encryptionKey.Length / sizeof(char))]);

                return output;
            }

            return data;
        }
    }
}
