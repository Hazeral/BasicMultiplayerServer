using System;
using System.IO;

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

        public static byte[] ConstructPacket(PacketType type, string payload)
        {
            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);

            writer.Write((ushort)(payload.Length + 1)); // + 1 to accommodate for the packet type byte
            writer.Write((byte)type);
            writer.Write(System.Text.Encoding.UTF8.GetBytes(payload));

            return stream.ToArray();
        }

        public static Tuple<byte, string> GetPacket(Player player)
        {
            try
            {
                byte[] packetSize = new byte[2];
                if (player.Stream.Read(packetSize, 0, packetSize.Length) == 0) throw new Exception("0 bytes read");

                ushort size = BitConverter.ToUInt16(XOR(player, packetSize));
                if (size > 1000) throw new Exception("Packet size limit exceeded");

                byte[] buffer = new byte[size];
                player.Stream.Read(buffer, 0, buffer.Length);

                buffer = XOR(player, buffer, 2);

                using MemoryStream stream2 = new MemoryStream(buffer);
                using BinaryReader reader2 = new BinaryReader(stream2);

                byte type = reader2.ReadByte();
                string payload = System.Text.Encoding.UTF8.GetString(reader2.ReadBytes(size));

                if (Server.verboseLogs) player.Log("Received packet", $"{(PacketType)type} > {payload}");

                return new Tuple<byte, string>(type, payload);
            }
            catch (Exception eX)
            {
                player.Log("Socket error", $"Error reading packets, {(eX.Message.Contains("WSACancelBlockingCall") ? "WSACancelBlockingCall" : eX.Message)}");
                if (eX.Message.Contains("An existing connection was forcibly closed by the remote host") ||
                    eX.Message.Contains("An established connection was aborted by the software in your host machine") ||
                    eX.Message.Contains("0 bytes read") ||
                    eX.Message.Contains("Packet size limit exceeded"))
                {
                    player.Log("Status", "Client has disconnected");
                    player.disconnect();
                }
            }

            return null;
        }

        public static byte[] XOR(Player player, byte[] data, int keyOffset = 0)
        {
            if (player.encrypt)
            {
                byte[] output = data;

                for (int i = 0; i < data.Length; i++)
                {
                    output[i] = (byte)(data[i] ^ player.encryptionKey[(i + keyOffset) % (player.encryptionKey.Length / sizeof(byte))]);
                }

                return output;
            }

            return data;
        }
    }
}
