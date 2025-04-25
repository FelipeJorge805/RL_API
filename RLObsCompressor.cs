using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace RL_API
    {
    public static class RLObsCompressor
    {
        public static RLCompressedObs Compress(RLObservation full)
        {
            //if(full==null) throw new Exception("Exception in Compress(): obs is null");

            try{ 
                var compressed = new RLCompressedObs
                {
                    PlayerInfo =
                    [
                        full.VelocityX,
                        full.VelocityY,
                        full.PositionX,
                        full.PositionY,
                        full.Health / 500f,  // Assuming max HP 500 normalized
                        full.Mana / 200f     // Assuming max Mana 200 normalized
                    ],

                    TileTypes = full.TilesAround != null
                        ? full.TilesAround.ConvertAll(t => (byte)t.TileType).ToArray()
                        : [],

                    HeldItemType = (byte)full.HeldItemType,

                    NearbyEnemies = (byte)Math.Min(full.NearbyEnemiesCount, 255), // cap at 255

                    FacingDirection = (sbyte)full.FacingDirection,

                    TotalDefense = (byte)Math.Min(full.TotalDefense, 255), // cap

                    BuffIds = full.ActiveBuffTypes.ConvertAll(b => (byte)b).ToArray(),

                    AccessoryIds = full.AccessoryTypes.ConvertAll(a => (byte)a).ToArray(),

                    CurrentBiome = MapBiomeToId(full.CurrentBiomes),

                    HookType = (byte)full.HookType,
                    MountType = (byte)full.MountType,
                    LightPetType = (byte)full.LightPetType,
                    PetType = (byte)full.PetType,

                };

                return compressed;
            }
            catch(Exception e){
                ModContent.GetInstance<RL_API>().Logger.Info("Error in Compress: "+e.Message);
            }
            return null;
        }

        private static byte MapBiomeToId(List<string> biomes)
        {
            if (biomes.Contains("Jungle")) return 1;
            if (biomes.Contains("Desert")) return 2;
            if (biomes.Contains("Snow")) return 3;
            if (biomes.Contains("Hallow")) return 4;
            if (biomes.Contains("Corruption")) return 5;
            if (biomes.Contains("Crimson")) return 6;
            if (biomes.Contains("Dungeon")) return 7;
            if (biomes.Contains("Underworld")) return 8;
            if (biomes.Contains("Sky")) return 9;
            if (biomes.Contains("Glowshroom")) return 10;
            return 0; // Default to Forest
        }
    }
}