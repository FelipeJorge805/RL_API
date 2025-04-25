using System;
using System.Net.Sockets;
using System.IO;
using Terraria.ModLoader;

namespace RL_API
{
    public class ConnectionManager : Mod
    {
        //Socket socket = s;
        String lastAction = "";

        public String ConsumeAction(){
            String temp = lastAction;
            lastAction = "";
            return temp;
        }

        public void SendData(){

        }
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
                ModContent.GetInstance<ConnectionManager>().Logger.Info($"Connected to RL server at {host}:{port}");
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<ConnectionManager>().Logger.Error($"Failed to connect to RL server: {ex.Message}");
                _client = null;
            }
        }

        /// <summary>
        /// Sends a JSON string to Python and reads a single-line response.
        /// Blocks until a line is received.
        /// </summary>
        public static string SendAndReceive(string json)
        {
            if (_writer == null || _reader == null)
                throw new InvalidOperationException("RLAgentClient not initialized.");

            // send
            _writer.WriteLine(json);

            // receive
            return _reader.ReadLine();
        }
    }
}