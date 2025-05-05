using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using RL_API;
using Terraria;
using Terraria.ID;

namespace RL_API
{
    // The main purpose of this class is to collect all the data form the player and store it in RLObservation
    // It collects player stats, buffs/debuffs, inventory, nearby tiles, nearby enemies, armor/accessories, weather/biome/time and more
    public static class RLObsCollector
    {
        private static readonly string[] BiomeOrder = [
            "Forest", "Corruption", "Crimson", "Jungle", "Snow",
            "Desert", "Hallow", "Dungeon", "Underworld", "Sky", "Glowshroom"
        ];

        //This function collects the data from the player and returns a RLObservation object.
        //It also normalizes the data based on max values, ready for RL.
        public static RLObservation CollectObservation(Player player)
        {
            var observation = new RLObservation
            {
                // Combat info
                Health = player.statLife / (float)player.statLifeMax2,
                MaxHealth = player.statLifeMax2 / 500f,
                Mana = player.statMana / (float)player.statManaMax2,
                MaxMana = player.statManaMax2 / 200f,

                // Movement
                VelocityX = player.velocity.X / 30f,
                VelocityY = player.velocity.Y / 30f,
                PositionX = player.position.X / 16f / 5000f, //divided by pixels (16) to get tiles, then divided by world size
                PositionY = player.position.Y / 16f / 2000f, //divided by pixels (16) to get tiles, then divided by world size

                // Player status
                IsInventoryOpen = Main.playerInventory ? 1f : 0f,
                IsInTown = player.townNPCs>2 ? 1f : 0f,
                IsChestOpen = Main.editChest ? 1f : 0f,
                Breath = player.breathMax > 0 ? (float)player.breath / player.breathMax : 1f,
                IsWet = player.wet ? 1f : 0f,
                HasGills = player.gills ? 1f : 0f,
                HasNightVision = player.nightVision ? 1f : 0f,
                HasNoFallDmg = player.noFallDmg ? 1f : 0f,
                HasNoKnockback = player.noKnockback ? 1f : 0f,
                IsHooked = player.grappling[0] != -1 ? 1f : 0f,
                IsBurning = (player.frostBurn || player.venom || player.burned || player.poisoned) ? 1f : 0f,
                FacingDirection = player.direction,

                // Summons
                ActiveMinionsCount = player.numMinions / (float)player.maxMinions,

                // Armor
                HeadArmorType = player.armor[0].type / 7000f,
                ChestArmorType = player.armor[1].type / 7000f,
                LegArmorType = player.armor[2].type / 7000f,
                TotalDefense = player.statDefense / 120f,

                // Accessories
                AccessoryTypes = GetAccessoryTypes(player),

                // Ammo (you can refine this more later)
                ArrowCount = player.CountItem(ItemID.WoodenArrow) / 999f * 4f,
                GelCount = player.CountItem(ItemID.Gel) / 999f * 4f,

                // Held item
                HeldItemType = (player.HeldItem?.type ?? 0) / 7000f,

                HookType = player.armor[10].type / 7000f,
                MountType = player.armor[11].type / 7000f,
                LightPetType = player.armor[12].type / 7000f,
                PetType = player.armor[13].type / 7000f,
                Cart = player.armor[14].type / 7000f,

                // Events / Boss
                IsBloodMoon = Main.bloodMoon ? 1f : 0f,
                IsBossActive = ( NPC.AnyNPCs(NPCID.EyeofCthulhu) 
                                || NPC.AnyNPCs(NPCID.KingSlime) ) 
                                ? 1f : 0f /* etc */,

                // Day or Night cycle
                TimeCycle = GetEncodedTime(player),

                // Enemies nearby
                NearbyEnemiesCount = CountNearbyEnemies(player, radius: 300f), // 300f pixels ≈ ~19 tiles
                NearbyEnemyInfo = GetNearbyEnemyInfo(player,10,10),

                // Biomes
                WeatherEventType = GetWeatherEventType(player),
                CurrentBiomes = GetBiomeOneHot(player),
                
                // Buffs and Debuffs
                BuffsVector = GetBuffVector(player, 22), //max debuffs allowed for now (yes max is 44)
                
                //Nearby items
                NearItems = CompressNearbyItems(ItemScanner.ScanNearbyItems(player, 10, 10),10),

                // Tiles around the player
                TilesAround = CompressTilesAround(TileScanner.scanTiles(player, 8)),

                //InvState
                InvState = new(player.inventory),
            };

            return observation;
        }
        private static float[] GetBuffVector(Player player, int maxPairs)
        {
            var result = new List<float>(maxPairs * 2);
            for (int i = 0; i < player.buffType.Length && result.Count < maxPairs * 2; i++)
            {
                int type = player.buffType[i];
                int time = player.buffTime[i];
                if (type <= 0 || time <= 0) continue;

                bool isDebuff = Main.debuff[type];
                float normType = type / 300f * (isDebuff ? -1f : 1f);
                float normTime = Math.Min(time / 54000f, 1f);

                result.Add(normType);
                result.Add(normTime);
            }

            while (result.Count < maxPairs * 2)
                result.Add(0f);

            return [.. result];
        }

        private static float[] GetNearbyEnemyInfo(Player player, float radius, int maxEnemies = 10)
        {
            var nearest = new SortedList<float, float[]>(); // distance → [x, y, id]

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (!npc.active || !npc.CanBeChasedBy(player)) continue;

                float dist = Vector2.Distance(player.Center, npc.Center);
                if (dist > radius) continue;

                float normX = (npc.Center.X - player.Center.X) / radius;
                float normY = (npc.Center.Y - player.Center.Y) / radius;
                float typeId = npc.type / 1000f; // normalized to [0,1] if desired

                // Avoid duplicate keys in SortedList (rare edge case)
                while (nearest.ContainsKey(dist)) dist += 0.0001f;

                nearest[dist] = [normX, normY, typeId];

                if (nearest.Count > maxEnemies)
                    nearest.RemoveAt(nearest.Count - 1); // remove farthest
            }

            var result = new List<float>();
            foreach (var vec in nearest.Values)
                result.AddRange(vec);

            // Pad with 0s if not enough enemies
            while (result.Count < maxEnemies * 3)
                result.Add(0f);

            return [.. result];
        }

        private static float[] GetAccessoryTypes(Player player)
        {
            var accessories = new List<float>();
            for (int i = 3; i <= 8; i++) // slots 3 to 8 are accessories by default
            {
                if (player.armor[i].type > ItemID.None)
                    accessories.Add(player.armor[i].type);
            }
            return [.. accessories];
        }

        private static float CountNearbyEnemies(Player player, float radius)
        {
            float count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active && npc.CanBeChasedBy(player))
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= radius)
                        count++;
                }
            }
            return count;
        }

        private static float[] GetBiomeOneHot(Player player)
        {
            float[] vec = new float[BiomeOrder.Length];

            if (player.ZoneForest) vec[0] = 1f;
            if (player.ZoneCorrupt) vec[1] = 1f;
            if (player.ZoneCrimson) vec[2] = 1f;
            if (player.ZoneJungle) vec[3] = 1f;
            if (player.ZoneSnow) vec[4] = 1f;
            if (player.ZoneDesert) vec[5] = 1f;
            if (player.ZoneHallow) vec[6] = 1f;
            if (player.ZoneDungeon) vec[7] = 1f;
            if (player.ZoneUnderworldHeight) vec[8] = 1f;
            if (player.ZoneSkyHeight) vec[9] = 1f;
            if (player.ZoneGlowshroom) vec[10] = 1f;

            return vec;
        }
        private static float GetWeatherEventType(Player player)
        {
            if (Main.raining)
            {
                if (player.ZoneSnow) return 2f; // Blizzard
                if (player.ZoneDesert && Main.windSpeedCurrent > 0.3f) return 3f; // Sandstorm-like
                return 1f; // Rain
            }
            if (Main.slimeRain) return 4f;
            return 0f; // None
        }

        private static float GetEncodedTime(Player player)
        {
            bool canSeeTime = player.accWatch > 0 || !player.ZoneRockLayerHeight;

            if (!canSeeTime)
                return -1f;

            if (Main.dayTime)
                return (float)(Main.time / 54000f);         // 0.0 to 1.0
            else
                return 1.0f + (float)(Main.time / 32400f);   // 1.0 to 2.0
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
                compressed.Add(tile.TileType / 255f);    // Normalize tile type
                compressed.Add(tile.LiquidType / 2f);     // Normalize liquid type
                compressed.Add(tile.LiquidAmount / 255f); // Normalize liquid amount
                compressed.Add((float)tile.getBrightness()); // Already 0-1
            }

            return [.. compressed];
        }

    }
}
