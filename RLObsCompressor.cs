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
                    NearItems = CompressNearbyItems(full.NearItems,10),

                    InvState = CompressInventory(full.InvState),
                    
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
        public static float[] CompressNearbyItems(List<RLItemObservation> items, float scanRadius)
        {
            var compressed = new List<float>();

            foreach (var item in items)
            {
                // Normalize ItemID
                float normalizedId = item.ItemId / 7000f; // Terraria ItemID max (safe overestimate)
                // Normalize StackSize
                float normalizedStack = item.StackSize / 999f; // Max typical stack
                // Normalize distance
                float normalizedDistance = item.DistanceToPlayer / scanRadius;
                // Pickup ready (already 0 or 1)

                compressed.Add(normalizedId);
                compressed.Add(normalizedStack);
                compressed.Add(normalizedDistance);
                compressed.Add(item.IsPickupReady ? 1f : 0f);
            }

            return compressed.ToArray();
        }
        public static float[] CompressInventory(InventoryState invState)
        {
            var compressed = new List<float>();

            for (int slot = 0; slot < 50; slot++)
            {
                var slotInfo = invState.Slots[slot];

                // Normalize stack size
                float normalizedStack = slotInfo.StackSize / 999f;
                compressed.Add(normalizedStack);

                // (Optional) Normalize ItemType if you want
                float normalizedItemType = slotInfo.ItemType / 7000f;
                compressed.Add(normalizedItemType);
            }

            return compressed.ToArray();
        }


    }
}