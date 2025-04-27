using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using rail;
using Stubble.Core;
using Terraria.ModLoader;

namespace RL_API
{
    public static class ConnectionManager
    {
        private static TcpClient _client;
        private static StreamReader _reader;
        private static StreamWriter _writer;

        private static readonly BlockingCollection<string> _obsQueue =
            new BlockingCollection<string>(new ConcurrentQueue<string>());

        // The most recently received action from the Python agent
        private static volatile AgentAction _latestAction;

        /// <summary>
        /// Called from Mod.Load(). Connects to the Python server and starts the I/O loop.
        /// </summary>
        public static void Initialize(string host = "127.0.0.1", int port = 5000)
        {
            if (_client != null && _client.Connected)
                return;

            try
            {
                _client = new TcpClient(host, port);
                var stream = _client.GetStream();
                _reader = new StreamReader(stream);
                _writer = new StreamWriter(stream) { AutoFlush = true };

                ModContent.GetInstance<RL_API>().Logger.Info($"[RL] Connected to {host}:{port}");

                Task.Run(() => IoLoop());
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<RL_API>().Logger.Error($"[RL] Connection error: {ex.Message}");
            }
        }

        //FOR MULTIPLE CONNECTIONS AT A TIME
        /*public static void Initialize()
        {
            int port = 5000; // Default port
            var envPort = Environment.GetEnvironmentVariable("TERRALPHA_PORT");
            if (int.TryParse(envPort, out int parsedPort))
            {
                port = parsedPort;
            }

            // Now connect using the dynamic port
            _client = new TcpClient("127.0.0.1", port);
            // (then your normal reader/writer init)
        }*/

        /// <summary>
        /// Called from PostUpdate to enqueue the latest observation (as a JSON string).
        /// </summary>
        public static void EnqueueObservation(string obsJson)
        {
            _obsQueue.Add(obsJson);
        }

        /// <summary>
        /// Called from SetControls to read the latest action (without clearing).
        /// </summary>
        public static AgentAction PeekAction()
        {
            //ModContent.GetInstance<RL_API>().Logger.Info("Peeked Action: " + _latestAction);
            return _latestAction;
        }

        /// <summary>
        /// Called from PostUpdate (or SetControls) to consume the action.
        /// </summary>
        public static AgentAction ConsumeAction()
        {
            //ModContent.GetInstance<RL_API>().Logger.Info("Consumed Action: " + _latestAction);
            AgentAction a = _latestAction;
            _latestAction = null;
            return a;
        }

        /// <summary>
        /// Handles blocking send/receive loop on a background thread.
        /// </summary>
        private static void IoLoop()
        {
            foreach (var obs in _obsQueue.GetConsumingEnumerable())
            {
                try
                {
                    // Send the observation to Python
                    _writer.WriteLine(obs);
                    //ModContent.GetInstance<RL_API>().Logger.Info("Sent obs: " + obs);

                    // Read Python's action response (also JSON)
                    string reply = _reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(reply))
                    {
                        _latestAction = JsonSerializer.Deserialize<AgentAction>(reply);
                    }
                    //ModContent.GetInstance<RL_API>().Logger.Info("Received Action: " + _latestAction);
                }
                catch (Exception ex)
                {
                    ModContent.GetInstance<RL_API>().Logger.Error($"[RL] I/O error: {ex.Message}");
                    break;
                }
            }
        }

        public static void Close()
        {
            try
            {
                if (_client != null)
                {
                    _obsQueue.CompleteAdding();  // stops the IoLoop
                    _reader?.Close();
                    _writer?.Close();
                    _client?.Close();
                    _client = null;

                    ModContent.GetInstance<RL_API>()
                        .Logger.Info("[RL] Disconnected from Python server.");
                }
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<RL_API>()
                    .Logger.Warn($"[RL] Error closing connection: {ex.Message}");
            }
        }


        private class AgentReply
        {
            public string action { get; set; }
        }
    }
    public class AgentAction
        {
            public AgentAction(string Move, string Action, float[] Cursor, bool Shift){
                this.Move=Move;
                this.Action=Action;
                this.Cursor=Cursor;
                this.Shift=Shift;
            }
            public string Move { get; set; }    // "left", "right", "still"
            public string Action { get; set; }  // "use_item", "jump", etc.
            public float[] Cursor { get; set; } // [deltaX, deltaY]

            public bool Shift { get; set; } // use shift
        }
}
