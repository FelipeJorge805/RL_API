using System;
using System.Collections.Generic;
using Terraria.ModLoader;

// Deprecated: Will no longer be used
namespace RL_API
    {
    public static class RLObsCompressor
    {
        public static RLCompressedObs Compress(RLObservation full)
        {
            try
            { 
                /*var compressed = new RLCompressedObs
                {
                    NearItems = full.NearItems, // Example scan radius

                    //InvState = full.InvState,
                            
                    PlayerInfo =
                    [
                        full.VelocityX / 30f,
                        full.VelocityY / 30f,
                        full.PositionX / 5000f,
                        full.PositionY / 2000f,
                        full.Health / 500f,
                        full.Mana / 200f
                    ],

                    TilesAround = full.TilesAround,

                    HeldItemType = (float)full.HeldItemType / 7000f,

                    NearbyEnemies = Math.Min(full.NearbyEnemiesCount, 255) / 255f,

                    FacingDirection = (float)full.FacingDirection,

                    TotalDefense = Math.Min(full.TotalDefense, 200) / 200f,

                    BuffIds = [.. full.BuffsVector],

                    AccessoryIds = [.. full.AccessoryTypes],

                    CurrentBiome = full.CurrentBiomes,

                    HookType = (float)full.HookType / 30f,
                    MountType = (float)full.MountType / 40f,
                    LightPetType = (float)full.LightPetType / 7000f,
                    PetType = (float)full.PetType / 7000f,
                };

                return compressed;*/
            }
            catch (Exception e)
            {
                ModContent.GetInstance<RL_API>().Logger.Info("Error in Compress: " + e.Message);
            }
            return null;
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

            return [.. compressed];
        }
        public static float[] CompressInventory(InventoryState invState)
        {
            var compressed = new float[50 * 2]; // 50 slots × (stack size + item type)

            int index = 0;

            foreach (var (itemType, totalStackSize) in invState.GetInventoryMap())
            {
                if (index >= 50)
                    break;

                compressed[index * 2] = totalStackSize / 999f; // Stack size normalized
                compressed[index * 2 + 1] = itemType / 7000f;  // Item type normalized

                index++;
            }

            return compressed;
        }


        public static float[] CompressTilesAround(List<TileInfo> tiles)
        {
            var compressed = new List<float>();

            foreach (var tile in tiles)
            {
                compressed.Add(tile.tileType / 255f);    // Normalize tile type
                compressed.Add(tile.liquidType / 2f);     // Normalize liquid type
                compressed.Add(tile.liquidAmount / 255f); // Normalize liquid amount
                compressed.Add(tile.brightness); // Already 0-1
            }

            return [.. compressed];
        }

    }
}