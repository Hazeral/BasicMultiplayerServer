using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;

namespace MultiplayerTestServer
{
    class Player
    {
        public TcpClient Socket { get; private set; }
        public NetworkStream Stream { get; private set; }
        public float[] Position { get; private set; }
        public string ID { get; private set; }
        public string encryptionKey { get; private set; }
        public bool Listening;
        private Thread listeningThread;
        private double lastPing;
        public bool encrypt = false;
        public bool admin = false;
        public bool muted = false;

        public Player(TcpClient _socket)
        {
            Socket = _socket;
            Stream = Socket.GetStream();

            Random r = new Random();
            int rX = r.Next(Map.Bounds[0, 0], Map.Bounds[0, 1]);
            int rY = r.Next(Map.Bounds[1, 0], Map.Bounds[1, 1]);

            Position = new float[] { rX, rY };

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
                string pack = Protocol.ConstructPacket(type, payload);

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(Protocol.EncryptData(this, pack));
                Stream.Write(msg, 0, msg.Length);
                if (Server.verboseLogs) Log("Sent packet", $"{type} > {payload}");
            }
            catch
            {
                Log("Socket error", "Error sending packet");
            }
        }

        private void readPackets()
        {
            Log("Listening for packets", "Started");
            while (Listening)
            {
                if (Stream == null) continue;

                string packet = Protocol.GetPacket(this);
                string[] data = packet.Split(Protocol.PayloadSeparator); // id$content

                try
                {
                    switch (int.Parse(data[0]))
                    {
                        case (int)(Protocol.PacketType.PlayerInput):
                            updatePosition(data[1]);
                            break;
                        case (int)(Protocol.PacketType.Ping):
                            ping();
                            break;
                        case (int)(Protocol.PacketType.ChatMessage):
                            newChatMessage(data[1]);
                            break;
                        default:
                            Log("Unknown packet", packet);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (packet.Trim() == "") continue;
                    Log("Error parsing packet", packet);
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

        private void ping()
        {
            lastPing = Server.ConvertToUnixTimestamp(DateTime.Now);
            sendPacket(Protocol.PacketType.Ping, "");
        }

        private void newChatMessage(string msg)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -/.]");
            string input = rgx.Replace(msg.Trim(), string.Empty);
            if (input.Length != 0)
            {
                Log("Chat message", input);
                if (muted)
                {
                    sendServerMessage("You are muted");
                    return;
                }
                if (input.StartsWith('/'))
                {
                    if (admin)
                    {
                        string command = input.Split()[0].Replace("/", "");
                        string[] cmdArgs = input.Split().Skip(1).ToArray();

                        Command cmd = Commands.Get(command);
                        if (cmd != null) cmd.Run(cmdArgs, this);
                    }
                } else Server.broadcast(Protocol.PacketType.ChatMessage, $"{ID}:{input}");
            }
        }

        public void sendServerMessage(string msg)
        {
            sendPacket(Protocol.PacketType.ServerMessage, msg);
            Log("Sent server message", msg);
        }

        private void updatePosition(string data)
        {
            string[] info = data.Split(':');
            float delta = float.Parse(info[0]);
            List<char> keys = new List<char>(info[1].ToCharArray());

            if (keys.Contains('W'))
            {
                Position[1] += 1 * delta;
            }
            if (keys.Contains('A'))
            {
                Position[0] -= 1 * delta;
            }
            if (keys.Contains('S'))
            {
                Position[1] -= 1 * delta;
            }
            if (keys.Contains('D'))
            {
                Position[0] += 1 * delta;
            }

            if (!Server.newPositionPlayers.Contains(this)) Server.newPositionPlayers.Add(this);
            Log("Updated position", $"{Position[0]}, {Position[1]}");
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
