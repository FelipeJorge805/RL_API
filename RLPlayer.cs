
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RL_API
{
    class RLPlayer() : ModPlayer
    {
        

		Tile_Scan tileScanner = new();
        List<object> tiles;
		int loopTicks = 0;
        public override void SetControls()
		{
			// Example: Simulate moving left
			Player.controlLeft = true;

			// Example: Simulate jumping
			if (Player.velocity.Y == 0f) // Ensure the player is grounded
			{
				Player.controlJump = true;
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
			loopTicks++;
			if(loopTicks % 5 == 0){
				tiles = tileScanner.scanTiles(Main.LocalPlayer,3);
			}
		}
		public override void PostUpdate()
		{
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