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
        public override void Load()
        {
            DisableUselessSettings();
        }

        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
            DisableUselessSettings();
            try{
                string[] args = Environment.GetCommandLineArgs();
                var idPort = 0;
                foreach(string arg in args){
                    //Main.NewText(arg);
                    if(arg.Contains("--agent")) int.TryParse(arg.Split('=')[1], out idPort);
                }
                ConnectionManager.Initialize("127.0.0.1", 5000+idPort);
            }
            catch(Exception e){
                Main.NewText("Socket Connection Error: "+e);
            }
        }
        public override void PreUpdateEntities()
        {
            Main.hasFocus = true;
            DisableUselessSettings();
        }
        public override void OnWorldUnload()
        {
            base.OnWorldUnload();
            ConnectionManager.Close();
        }
        private void DisableUselessSettings()
        {
            // Audio
            Main.soundVolume = 0f;
            Main.musicVolume = 0f;

            // Lighting
            Lighting.Mode = Terraria.Graphics.Light.LightMode.White; // 0 = white, 1 = retro, 2 = color
            Lighting.GlobalBrightness = 1f;

            // Drawing
            Main.drawSkip = true; // May not work in all tML versions
            Main.renderCount = 0;
            Main.drawToScreen = false;

            // Disable fancy extras
            Main.maxQ = false; // Older version cloud draw cap
            Main.cloudAlpha = 0f;
            Main.windSpeedCurrent = 0f;
            Main.windSpeedTarget = 0f;

            Main.render = false;
            Main.skipMenu = true; // Stops menu animations
        }
    }
}
