using System.IO;
using System.Text.Json;
using Terraria;
using Terraria.ModLoader;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace RL_API
{
    public class RL_API : Mod
    {
        private static bool LoopHasBeenCalled = false;
        public override void Load()
        {
            bool isAgent = Environment.GetCommandLineArgs().Any(arg => arg.Contains("--agent"));
            if (!isAgent && !LoopHasBeenCalled)
            {
                Logger.Info("\n\nNon Agent mode detected, skipping join loop.\n");
                return;
            }else{
                StartJoinLoop();
                LoopHasBeenCalled = true;
                Logger.Info("\n\nAgent mode detected, starting join loop.\n");
            }
        }

        private async void StartJoinLoop()
        {
            
            
            string configPath;
            try{
                configPath = Path.Combine(Main.SavePath, "client_config.json");
                if (!File.Exists(configPath))
                {
                    Logger.Warn("No client_config.json found, skipping.");
                    Logger.Warn("Path: "+Main.SavePath);
                    return;
                }
            }catch (System.Exception ex)
            {
                Logger.Error($"Error loading config: {ex}");
                return;
            }

            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<ClientConfig>(json);

            Logger.Info("Waiting for menu to join...");
			Logger.Info($"Joining {config.server_ip}:{config.server_port}");

            while (true)
            {
				Logger.Info($"Joining {config.server_ip}:{config.server_port}");

                await Task.Delay(10_000);

                if (Main.gameMenu)
                {
                    try
                    {
                        Logger.Info($"Joining {config.server_ip}:{config.server_port}");

                        Netplay.ServerIP = System.Net.IPAddress.Parse("127.0.0.1");        // string to IPAddress
                        //Netplay.serverServerPort = config.server_port;    // int
                        Netplay.StartTcpClient();                  // trigger connection
                        Main.autoJoin = true;

                        ConnectionManager.Initialize(config.server_ip, config.agent_port);

                        Logger.Info("Started join + socket.");
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Error($"Error starting join: {ex}");
                    }

                    break;
                }
                else
                {
                    Logger.Info("Not in main menu yet, waiting...");
                }
            }
        }

        public override void Unload()
        {
            base.Unload();
            ConnectionManager.Close();
        }

        private class ClientConfig
        {
            public string server_ip { get; set; }
            public int server_port { get; set; }
            public int agent_port { get; set; }
        }
    }
}
