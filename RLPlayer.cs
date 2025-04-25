using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace RL_API
{
    class RLPlayer() : ModPlayer
    {

		Tile_Scan tileScanner = new();
        List<object> tiles;
        int hertz = 1;
        private static string lastKnownAction = "none";
        public override void SetControls()
		{
            string action = ConnectionManager.PeekAction()?.Trim();
    
            if (Main.GameUpdateCount % hertz == 0)
            {
                ModContent.GetInstance<RL_API>().Logger.Info("Updated Action: "+action);

                if (!string.IsNullOrEmpty(action))
                    lastKnownAction = action;
            }

            switch (lastKnownAction)
            {
                case "left":
                    Player.controlLeft = true;
                    break;
                case "right":
                    Player.controlRight = true;
                    break;
                case "jump":
                    Player.controlJump = true;
                    break;
                case "use_item":
                    Player.controlUseItem = true;
                    break;
                case "none":
                case null:
                case "":
                    // Do nothing
                    break;
                default:
                    // Unknown action, maybe log?
                    Main.NewText($"Unknown RL action: {lastKnownAction}");
                    break;

            }
        }

        public override void PreUpdateBuffs()
        {
            base.PreUpdateBuffs();
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
        }

        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
        }
		public override void PreUpdate()
    	{
			//base.PreUpdate();
			if(Main.GameUpdateCount % hertz == 0){
				tiles = tileScanner.scanTiles(Main.LocalPlayer,3);
			}
		}
		public override void PostUpdate()
		{
            if(Main.GameUpdateCount % hertz != 0) return;
            //var data = "{obs: test}";
            var obsJson = JsonSerializer.Serialize(new { obs = "test" });
            ConnectionManager.EnqueueObservation(obsJson);  // just queue, no await/read here

            var action = ConnectionManager.ConsumeAction();
			//base.PostUpdate();
			/*if(loopTicks % 60 == 0){
				Main.NewText("Printing... " + loopTicks/60);
				String t = "";
				tiles.ForEach(o => t+=o+",");
				Main.NewText(t);
			}*/
		}
    }
}