using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace RL_API
{
	public static class Tile_Scan
	{
		public static List<TileInfo> scanTiles(Player player, int radius){

            //int tileSize = 16;
            int px = (int)(player.Center.X / 16f);
            int py = (int)(player.Center.Y / 16f);

            //var tileGrid = new List<List<object>>();
            var tiles = new List<TileInfo>();
            
            try{
                for (int y = -radius; y <= radius; y++)
                {
                    var row = new List<TileInfo>();
                    for (int x = -radius; x <= radius; x++)
                    {
                        int tx = px + x;
                        int ty = py + y;
                        //ModContent.GetInstance<RL_API>().Logger.Info("tx:"+tx+" ty:"+ty + " px:"+px + " py:"+py + " x:"+x + " y:"+y);
                        TileInfo tile = EncodeTile(tx,ty);
                        row.Add(tile);
                    }
                }
            }
            catch(Exception e){
                ModContent.GetInstance<RL_API>().Logger.Info("Error in scan loop.");
                ModContent.GetInstance<RL_API>().Logger.Error(e.Message);
            }
            return tiles; 
		}
        private static TileInfo EncodeTile(int tx, int ty)
        {
            float brightness = Lighting.Brightness(tx, ty);

            if (brightness >= 0.2f)
            {
                Tile tile = Main.tile[tx, ty];
                int tileType = tile.HasTile ? tile.TileType : -1;
                tileType += 2;
                int liquidType = tile.LiquidType + 1;
                int liquidAmount = tile.LiquidAmount + 1;

                return new TileInfo(tileType, liquidType, liquidAmount, brightness);
            }
            else
            {
                return new TileInfo(0, 0, 0, 0);
            }
        }

	}
}