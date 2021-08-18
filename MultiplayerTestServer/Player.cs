using System;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

namespace MultiplayerTestServer
{
    class Player
    {
        public TcpClient Socket { get; private set; }
        public NetworkStream Stream { get; private set; }
        public float[] Position { get; set; }
        public string ID { get; private set; }
        public string encryptionKey { get; private set; }
        public bool Listening;
        public double lastPing;
        public bool encrypt = false;
        public bool admin = false;
        public bool muted = false;
        private Thread listeningThread;

        public Player(TcpClient _socket)
        {
            Socket = _socket;
            Stream = Socket.GetStream();

            Position = Map.RandomCoordinates();

            Random r = new Random();
            ID = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz0123456789", 4).Select(s => s[r.Next(s.Length)]).ToArray());
            encryptionKey = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8).Select(s => s[r.Next(s.Length)]).ToArray());

            Log("Status", "Client has connected" + (Server.encrypted ? $", encryption key: [{encryptionKey}]" : ""));
            sendPacket((int)(Protocol.PacketType.PlayerWelcome), ID + (Server.encrypted ? $":{encryptionKey}" : ""));

            listeningThread = new Thread(readPackets);
            Listening = true;
            listeningThread.Start();
            lastPing = Server.ConvertToUnixTimestamp(DateTime.Now);
        }

        public void sendPacket(Protocol.PacketType type, string payload)
        {
            try
            {
                byte[] pack = Protocol.XOR(this, Protocol.ConstructPacket(type, payload));

                Stream.Write(pack, 0, pack.Length);

                if (Server.encrypted) encrypt = true;
                if (Server.verboseLogs) Log("Sent packet", $"{type} > {payload}");
            }
            catch (Exception eX)
            {
                Log("Socket error", "Error sending packet");
                Log("Socket error", eX.Message);
            }
        }

        private void readPackets()
        {
            Log("Listening for packets", "Started");
            while (Listening)
            {
                if (Stream == null) continue;
                Tuple<byte, string> packet = null;

                try
                {
                    packet = Protocol.GetPacket(this);

                    Action<Player, string> handler = PacketHandler.GetHandler(packet.Item1);

                    if (handler != null)
                    {
                        handler(this, packet.Item2);
                    } else
                    {
                        Log("Unknown packet", $"{packet.Item1} > {packet.Item2}");
                    }
                }
                catch (Exception ex)
                {
                    if (packet == null) continue;
                    Log("Error parsing packet", $"{packet.Item1} > {packet.Item2}");
                    Log("Error", ex.Message);
                }

                double currentTime = Server.ConvertToUnixTimestamp(DateTime.Now);
                if ((currentTime - lastPing) > Server.pingTimeout)
                {
                    Log("Status", "Client has timed out");
                    disconnect();
                }
            }
            Log("Listening for packets", "Stopped");
        }

        public void disconnect(bool broadcast = true)
        {
            Server.players.Remove(ID);
            if (broadcast) Server.broadcast(Protocol.PacketType.PlayerDisconnect, ID);
            sendServerMessage("You have been disconnected");
            Listening = false;
            Stream.Close();
            Socket.Close();
        }

        public void sendServerMessage(string msg)
        {
            sendPacket(Protocol.PacketType.ServerMessage, msg);
            Log("Sent server message", msg);
        }

        public void Log(string type, string message)
        {
            string output = $"[{DateTime.UtcNow}] <{ID}> {type}: {message}";

            if (Server.LOG_CONSOLE) Console.WriteLine(output);

            try
            {
                if (Server.LOG_FILE) Server.LOG_FILE_WRITER.WriteLine(output);
            }
            catch (Exception eX)
            {
                if (Server.LOG_CONSOLE) Console.WriteLine(eX.Message);
                if (Server.LOG_FILE) Server.LOG_FILE_WRITER.WriteLine(eX.Message);
            }
        }
    }
}
