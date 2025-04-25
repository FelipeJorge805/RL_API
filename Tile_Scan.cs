using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace RL_API
{
	public class Tile_Scan : Mod
	{
		Vector2 playerPos;

        public Tile_Scan(){}
		public List<object> scanTiles(Player player, int radius){
            //playerPos = player_position;

            //int tileSize = 16;
            int px = (int)(player.Center.X /16f);
            int py = (int)(player.Center.Y / 16f);

            //var tileGrid = new List<List<object>>();
            var tiles = new List<Object>();
            
            try{
                for (int y = -radius; y <= radius; y++)
                {
                    var row = new List<object>();
                    for (int x = -radius; x <= radius; x++)
                    {
                        int tx = px + x;
                        int ty = py + y;
                        ModContent.GetInstance<Tile_Scan>().Logger.Info("tx:"+tx+" ty:"+ty + " px:"+px + " py:"+py + " x:"+x + " y:"+y);
                        Tile tile = Main.tile[tx, ty];

                        int tileType = tile.HasTile ? tile.TileType : -1;
                        int liquid = tile.LiquidType;
                        int liquidAmount = tile.LiquidAmount;

                        //row.Add(new object[] { tileType, liquid, liquidAmount });
                        tiles.Add(tileType);
                    }
                    //tileGrid.Add(row);
                }
            }
            catch(Exception e){
                ModContent.GetInstance<Tile_Scan>().Logger.Info("Error in scan loop.");
                ModContent.GetInstance<Tile_Scan>().Logger.Error(e.Message);
            }
            
            
            /*try{// Save to JSON file that Python can read
                string path = "C:\\Users\\felip\\OneDrive\\Documents\\My Games\\Terraria\\tModLoader\\Mods";
                File.WriteAllText(path, JsonSerializer.Serialize(tiles));

                ModContent.GetInstance<Tile_Scan>().Logger.Info("Tile scan saved to RLTiles.json");
            }
            catch(Exception e){
                ModContent.GetInstance<Tile_Scan>().Logger.Info("Error in writing to file");
                ModContent.GetInstance<Tile_Scan>().Logger.Error(e.Message);
            }*/

            return tiles; 
		}
	}
}