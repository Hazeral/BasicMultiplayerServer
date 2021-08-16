using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.IO;

namespace MultiplayerTestServer
{
    class Server
    {
        public static bool LOG_CONSOLE = true;
        public static bool LOG_FILE = true;
        public static StreamWriter LOG_FILE_WRITER;
        public static string LOG_FILE_PATH;

        public static Dictionary<string, Player> players = new Dictionary<string, Player>();
        public static List<Player> newPositionPlayers = new List<Player>();
        public static TcpListener server;
        private static Thread serverThread;
        private static Thread broadcastPositionsThread;
        public static bool running;
        public static bool encrypted = true;
        public static bool verboseLogs = false;

        private static int port = 2330;
        public static int pingTimeout = 10;

        public static void broadcast(Protocol.PacketType type, string payload)
        {
            Log("Broadcast", $"{type} > {payload}");
            for(int i = 0; i < players.Count; i++)
            {
                players.ElementAt(i).Value.sendPacket(type, payload);
            }
        }

        private static void broadcastPositions()
        {
            while (true)
            {
                if (players.Count < 1) continue;
                string payload = "";

                try
                {
                    foreach (Player p in newPositionPlayers.ToList())
                    {
                        if (payload != "") payload += ';';
                        payload += $"{p.ID}:{p.Position[0]},{p.Position[1]}";
                    }

                    if (payload != "")
                    {
                        Log("Broadcasting positions update", payload);
                        broadcast(Protocol.PacketType.PositionsUpdate, payload);
                        newPositionPlayers.Clear();
                    }
                }
                catch (Exception eX)
                {
                    Log("Broadcast error", "Error building payload");
                    Log("Broadcast error", eX.Message);
                }
            }
        }

        public static void runServer()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();

                Log("Status", $"Running on port {port}");

                broadcastPositionsThread = new Thread(broadcastPositions);
                broadcastPositionsThread.Start();

                while (running)
                {
                    try
                    {
                        TcpClient client = server.AcceptTcpClient();
                        Player player = new Player(client);
                        foreach (Player p in players.Values.ToList())
                        {
                            player.sendPacket(Protocol.PacketType.PlayerJoin, $"{p.ID}:{p.Position[0]},{p.Position[1]}");
                        }
                        players.Add(player.ID, player);
                        broadcast(Protocol.PacketType.PlayerJoin, $"{player.ID}:{player.Position[0]},{player.Position[1]}");
                    } catch
                    {
                        Log("Error", "Cannot accept clients");
                    }
                }
            }
            catch (SocketException e)
            {
                Log("Socket error", e.Message);
            }
            finally
            {
                server.Stop();
                Log("Status", $"Stopped");
            }
        }

        public static void Main(string[] args)
        {
            if (args.Contains("--help"))
            {
                Console.WriteLine("--unencrypted                Disable encryption after welcome packet");
                Console.WriteLine("--verbose                    Log all exchanged packets");
                Console.WriteLine("--disable-log-console        Disable console log");
                Console.WriteLine("--disable-log-file           Disable file log");
                return;
            }
            if (args.Contains("--unencrypted")) encrypted = false;
            if (args.Contains("--verbose")) verboseLogs = true;
            if (args.Contains("--disable-log-console")) LOG_CONSOLE = false;
            if (args.Contains("--disable-log-file")) LOG_FILE = false;
            else
            {
                Directory.CreateDirectory("logs");
                LOG_FILE_PATH = Path.Combine("logs", $"{ConvertToUnixTimestamp(DateTime.Now)}.log");
                LOG_FILE_WRITER = File.AppendText(LOG_FILE_PATH);
            }

            running = true;
            serverThread = new Thread(runServer);
            serverThread.Start();

            while (running)
            {
                string input = Console.ReadLine();
                string command = input.Split()[0];
                string[] cmdArgs = input.Split().Skip(1).ToArray();

                Command cmd = Commands.Get(command);
                if (cmd != null) cmd.Run(cmdArgs);
            }
        }

        public static void Log(string type, string message)
        {
            string output = $"[{DateTime.UtcNow}] <SERVER> {type}: {message}";

            if (LOG_CONSOLE) Console.WriteLine(output);

            try
            {
                if (LOG_FILE) LOG_FILE_WRITER.WriteLine(output);
            } catch (Exception eX)
            {
                if (LOG_CONSOLE) Console.WriteLine(eX.Message);
            }
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
