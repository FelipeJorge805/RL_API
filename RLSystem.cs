using Terraria.ModLoader;
using System;
using System.Linq;
using System.Configuration;
using System.CommandLine.Parsing;
using Terraria;

namespace RL_API
{
    public class RLSystem : ModSystem
    {
        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
            try{
                string[] args = Environment.GetCommandLineArgs();
                var idPort = 0;
                foreach(string arg in args){
                    Main.NewText(arg);
                    if(arg.Contains("--agent")) int.TryParse(arg.Split('=')[1], out idPort);
                }
                ConnectionManager.Initialize("127.0.0.1", 5000+idPort);
            }
            catch(Exception e){
                Main.NewText("Socket Connection Error: "+e);
            }
        }
        public override void OnWorldUnload()
        {
            base.OnWorldUnload();
            ConnectionManager.Close();
        }
    }
}
