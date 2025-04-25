using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace RL_API
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class RL_API : ModPlayer
	{
		Vector2 playerPos = Main.LocalPlayer.position;
		List<object> tiles;
		int loopTicks = 0;
		Tile_Scan tileScanner = new();

		/*public void saveLog(){
			Logger.Info(playerPos);
			ModContent.GetInstance<RL_API>().Logger.Info("This is a log message.");
		}*/
		
		
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
		
		public override void PreUpdate()
    	{
			loopTicks++;
			if(loopTicks % 5 == 0){
				tiles = tileScanner.scanTiles(Main.LocalPlayer,3);
				
				//Player.controlRight = true;
				//Player.controlLeft = true;
				//Player.velocity.X = -2f;
				//Player.velocity.Y = -10f;
			}
			//Player.controlJump = true;
			
			/*if(loopTicks % 60 == 0){
				Main.NewText("Printing... " + loopTicks/60);
				String t = "";
				tiles.ForEach(o => t+=o+",");
				Main.NewText(t);
			}*/
		}
		public override void PostUpdate()
		{
			//Player.controlJump = true;
			//Player.controlLeft = true;
			//Player.velocity.X = -2f;
		}
	}
}