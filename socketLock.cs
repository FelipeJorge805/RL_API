// File: RLAgentClient.cs
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace RL_API
{
    public static class ConnectionManagerLock
    {
        // queue of JSON observations to send
        private static readonly BlockingCollection<string> _obsQueue = 
            new BlockingCollection<string>(new ConcurrentQueue<string>());

        // the latest action received from Python  
        private static string _latestAction = "none";  
        private static readonly object _actionLock = new();

        private static TcpClient _client;
        private static StreamReader _reader;
        private static StreamWriter _writer;

        public static void Initialize(string host = "127.0.0.1", int port = 5000)
        {
            if (_client != null && _client.Connected) return;

            try
            {
                _client = new TcpClient(host, port);
                var stream = _client.GetStream();
                _reader = new StreamReader(stream);
                _writer = new StreamWriter(stream) { AutoFlush = true };
                ModContent.GetInstance<RL_API>().Logger.Info($"[RL] Connected to {host}:{port}");

                // start the single background thread
                Task.Run(() => IoLoop());
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<RL_API>().Logger.Error($"[RL] Socket init failed: {ex.Message}");
            }
        }

        

        // producer: called from PostUpdate()
        public static void EnqueueObservation(string obsJson)
        {
            // will never block the game thread
            _obsQueue.Add(obsJson);
        }

        // consumer: called from SetControls()
        public static string ConsumeAction()
        {
            lock (_actionLock)
            {
                var a = _latestAction;
                _latestAction = "none";
                return a;
            }
        }

        // the only place we ever do socket I/O—and it runs off the game thread
        private static void IoLoop()
        {
            foreach (var obs in _obsQueue.GetConsumingEnumerable())
            {
                try
                {
                    // 1) send obs
                    _writer.WriteLine(obs);

                    // 2) block here *inside this background thread* until Python replies
                    string reply = _reader.ReadLine();

                    // 3) store for next tick
                    lock (_actionLock)
                        _latestAction = JsonSerializer.Deserialize<ReplyMsg>(reply).Action.ToString();
                }
                catch (Exception ex)
                {
                    ModContent.GetInstance<RL_API>()
                              .Logger.Error($"[RL] I/O loop error: {ex.Message}");
                    break;
                }
            }
        }

        // helper for deserializing Python’s reply
        private class ReplyMsg { public int Action { get; set; } }
    }
}
