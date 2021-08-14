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
        private bool encrypt = false;
        public bool admin = false;
        public bool muted = false;

        public Player(TcpClient _socket)
        {
            Socket = _socket;
            Stream = Socket.GetStream();

            Random r = new Random();
            int rX = r.Next(-8, 8);
            int rY = r.Next(-4, 4);

            Position = new float[] { rX, rY };

            ID = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz0123456789", 4).Select(s => s[r.Next(s.Length)]).ToArray());
            encryptionKey = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8).Select(s => s[r.Next(s.Length)]).ToArray());

            Log("Status", "Client has connected" + (Server.encrypted ? $", encryption key: [{encryptionKey}]" : ""));
            sendPacket((int)(PacketType.PlayerWelcome), ID + (Server.encrypted ? $":{encryptionKey}" : ""));

            listeningThread = new Thread(readPackets);
            Listening = true;
            listeningThread.Start();
            lastPing = Server.ConvertToUnixTimestamp(DateTime.Now);
        }

        public void sendPacket(PacketType type, string payload)
        {
            try
            {
                string pack = $"{(int)type}${payload}~";

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(encryptData(pack));
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

                string packet = getPacket();
                string[] data = packet.Split('$'); // id$content

                try
                {
                    switch (int.Parse(data[0]))
                    {
                        case (int)(PacketType.PlayerInput):
                            updatePosition(data[1]);
                            break;
                        case (int)(PacketType.Ping):
                            ping();
                            break;
                        case (int)(PacketType.ChatMessage):
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
            if (broadcast) Server.broadcast(PacketType.PlayerDisconnect, ID);
            sendServerMessage("You have been disconnected");
            Listening = false;
            Stream.Close();
            Socket.Close();
        }

        private void ping()
        {
            lastPing = Server.ConvertToUnixTimestamp(DateTime.Now);
            sendPacket(PacketType.Ping, "");
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
                } else Server.broadcast(PacketType.ChatMessage, $"{ID}:{input}");
            }
        }

        public void sendServerMessage(string msg)
        {
            sendPacket(PacketType.ServerMessage, msg);
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

        private string getPacket()
        {
            try
            {
                string packet = "";
                bool reading = true;

                while (reading)
                {
                    int currentByte = Stream.ReadByte();
                    char byteToChar = decryptChar(System.Text.Encoding.ASCII.GetString(new[] { (byte)currentByte }).ToCharArray()[0], packet.Length);

                    if (currentByte == -1 || byteToChar == '~') reading = false;
                    else
                    {
                        packet += byteToChar;
                    }
                }

                if (packet == "") throw new Exception("Empty packet");
                if (Server.verboseLogs) Log("Received packet", packet);
                return packet;
            }
            catch (Exception eX)
            {
                Log("Socket error", $"Error reading packets, {(eX.Message.Contains("WSACancelBlockingCall") ? "WSACancelBlockingCall" : eX.Message)}");
                if (eX.Message.Contains("An existing connection was forcibly closed by the remote host") || 
                    eX.Message.Contains("An established connection was aborted by the software in your host machine") ||
                    eX.Message.Contains("Empty packet"))
                {
                    Log("Status", "Client has disconnected");
                    disconnect();
                }
            }

            return "";
        }

        private string encryptData(string data)
        {
            if (encrypt)
            {
                char[] output = data.ToCharArray();

                for (int i = 0; i < data.ToCharArray().Length; i++)
                {
                    output[i] = (char)(data.ToCharArray()[i] ^ encryptionKey[i % (encryptionKey.Length / sizeof(char))]);
                }

                return new string(output);
            }
            if (Server.encrypted) encrypt = true;

            return data;
        }

        private char decryptChar(char data, int length)
        {
            if (encrypt)
            {
                char output = (char)(data ^ encryptionKey[length % (encryptionKey.Length / sizeof(char))]);

                return output;
            }

            return data;
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
