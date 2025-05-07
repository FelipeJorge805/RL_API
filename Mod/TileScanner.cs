using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace RL_API
{
	public static class TileScanner
	{
        private static int prevPX = -1;
        private static int prevPY = -1;
        private static List<TileInfo> prevList = null;
		public static List<TileInfo> scanTiles(Player player, int radius){

            // Simulate lighting before scanning
            //Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.5f, 0.5f, 0.5f);
            //Lighting.Initialize();

            int px = (int)(player.Center.X / 16f);
            int py = (int)(player.Center.Y / 16f);

            if(prevPX == px && prevPY == py && prevList != null) 
                return prevList;

            //Main.NewText("Tile Scanner Running");
            var tiles = new List<TileInfo>();
            
            try{
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        int tx = px + x;
                        int ty = py + y;
                        //ModContent.GetInstance<RL_API>().Logger.Info("tx:"+tx+" ty:"+ty + " px:"+px + " py:"+py + " x:"+x + " y:"+y);
                        TileInfo tile = EncodeTile(tx,ty);
                        tiles.Add(tile);
                    }
                }
            }
            catch(Exception e){
                ModContent.GetInstance<RL_API>().Logger.Info("Error in scan loop.");
                ModContent.GetInstance<RL_API>().Logger.Error(e.Message);
            }
            prevPX = px;
            prevPY = py;
            prevList = tiles;
            return tiles; 
		}
        private static TileInfo EncodeTile(int tx, int ty)
        {
            float brightness = Lighting.Brightness(tx, ty);

            if (brightness >= 0.15f){
                Tile tile = Main.tile[tx, ty];
                float tileType = tile.HasTile ? tile.TileType+2f : 1f; // Adding so no info can be 0, air is 1, and dirt(id==0) is 2
                float liquidType = tile.LiquidAmount > 0 ? tile.LiquidType + 1f : 0f;
                float liquidAmount = tile.LiquidAmount;

                return new TileInfo(tileType, liquidType, liquidAmount, brightness);
            }
            else
            {
                return new TileInfo(0f, 0f, 0f, 0f);
            }
        }

	}
}