using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace RL_API{
    
    public class WorldSystem : ModSystem
    {
        public override void PostWorldGen()
        {
            int replacedTrees = 0;

            for (int x = 10; x < Main.maxTilesX - 10; x++)
            {
                for (int y = 10; y < Main.maxTilesY - 10; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile == null || !tile.HasTile) continue;

                    // 🌳 Tree normalization
                    if (tile.TileType == TileID.Trees)
                    {
                        int treeType = tile.TileFrameX / 22;
                        if (treeType != 0)
                        {
                            tile.TileFrameX = 0;
                            tile.TileFrameY = 0;
                            replacedTrees++;
                        }
                        continue; // skip other checks for tree tiles
                    }

                    string tileName = TileLoader.GetTile(tile.TileType)?.Name?.ToLowerInvariant() ?? "";

                    if (tileName.Contains("wood"))
                    {
                        // wood block
                        if (tile.TileType == TileID.WoodBlock)
                        {
                            tile.TileType = TileID.WoodBlock;
                            tile.TileFrameX = 0;
                            tile.TileFrameY = 0;
                        }
                        else if (tile.TileType == TileID.Trees)
                        {
                            tile.TileFrameX = 0;
                        }
                        else if (tileName.Contains("door"))
                        {
                            tile.TileType = TileID.ClosedDoor;
                            tile.TileFrameX = 0;
                            tile.TileFrameY = 0;
                        }
                        else if (tileName.Contains("chair"))
                        {
                            tile.TileType = TileID.Chairs;
                            tile.TileFrameX = 0;
                            tile.TileFrameY = 0;
                        }
                        else if (tileName.Contains("table"))
                        {
                            tile.TileType = TileID.Tables;
                            tile.TileFrameX = 0;
                            tile.TileFrameY = 0;
                        }
                        else if (tileName.Contains("workbench"))
                        {
                            tile.TileType = TileID.WorkBenches;
                            tile.TileFrameX = 0;
                            tile.TileFrameY = 0;
                        }
                        else if (tileName.Contains("beam"))
                        {
                            tile.TileType = TileID.WoodenBeam;
                            tile.TileFrameX = 0;
                            tile.TileFrameY = 0;
                        }
                    }

                    // Torch replacement (biome torch to regular)
                    if (tileName.Contains("torch"))
                    {
                        tile.TileType = TileID.Torches;
                        tile.TileFrameX = 0;
                        tile.TileFrameY = 0;
                    }

                    // Wall replacement (outside tile check)
                    string wallName = WallLoader.GetWall(tile.WallType)?.Name?.ToLowerInvariant();
                    if (wallName != null && wallName.Contains("wood"))
                    {
                        tile.WallType = WallID.Wood;
                    }
                }
            }
            ModContent.GetInstance<RL_API>().Logger.Info($"[WorldGen] Replaced {replacedTrees} non-forest trees.");
            WorldGen.RangeFrame(0, 0, Main.maxTilesX, Main.maxTilesY); // Refresh visuals
        }
    }
}