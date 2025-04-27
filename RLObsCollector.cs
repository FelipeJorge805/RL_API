using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RL_API;
using Terraria;
using Terraria.ID;

namespace RL_API
{
    public static class RLObsCollector
    {
        public static RLObservation CollectObservation(Player player)
        {
            var observation = new RLObservation
            {
                //Nearby items
                NearItems = ItemScanner.ScanNearbyItems(player, 10, 10),

                // Movement
                VelocityX = player.velocity.X,
                VelocityY = player.velocity.Y,
                PositionX = player.position.X,
                PositionY = player.position.Y,

                // Tiles around the player (replace this with your tile scanning code)
                TilesAround = Tile_Scan.scanTiles(player, 5),

                // Combat info
                Health = player.statLife,
                Mana = player.statMana,

                // Armor
                HeadArmorType = player.armor[0].type,
                ChestArmorType = player.armor[1].type,
                LegArmorType = player.armor[2].type,
                TotalDefense = player.statDefense,

                // Accessories
                AccessoryTypes = GetAccessoryTypes(player),

                // Buffs and Debuffs
                ActiveBuffTypes = GetBuffs(player, positive: true),
                ActiveDebuffTypes = GetBuffs(player, positive: false),

                // Summons
                ActiveMinionsCount = player.numMinions,

                // Events / Boss
                IsBloodMoon = Main.bloodMoon,
                IsBossActive = NPC.AnyNPCs(NPCID.EyeofCthulhu) || NPC.AnyNPCs(NPCID.KingSlime) /* etc */,

                // Player states
                IsFalling = player.velocity.Y > 0f && !player.mount.Active,
                IsHooked = player.grappling[0] != -1,
                IsKnockedBack = player.controlHook, // simple guess: holding hook button

                // Ammo (you can refine this more later)
                ArrowCount = player.CountItem(ItemID.WoodenArrow),
                GelCount = player.CountItem(ItemID.Gel),

                // Held item
                HeldItemType = player.HeldItem?.type ?? 0,

                // Enemies nearby
                NearbyEnemiesCount = CountNearbyEnemies(player, radius: 300f), // 300f pixels ≈ ~19 tiles

                // Facing direction
                FacingDirection = player.direction,

                // Biomes
                CurrentBiomes = GetCurrentBiomes(player),

                HookType = player.armor[10].type,
                MountType = player.armor[11].type,
                LightPetType = player.armor[12].type,
                PetType = player.armor[13].type,
            };

            return observation;
        }

        private static List<int> GetAccessoryTypes(Player player)
        {
            var accessories = new List<int>();
            for (int i = 3; i <= 8; i++) // slots 3 to 8 are accessories by default
            {
                if (player.armor[i].type > ItemID.None)
                    accessories.Add(player.armor[i].type);
            }
            return accessories;
        }

        private static List<int> GetBuffs(Player player, bool positive)
        {
            var buffs = new List<int>();
            for (int i = 0; i < player.buffType.Length; i++)
            {
                int buff = player.buffType[i];
                if (buff > 0)
                {
                    bool isDebuff = Main.debuff[buff];
                    if ((positive && !isDebuff) || (!positive && isDebuff))
                        buffs.Add(buff);
                }
            }
            return buffs;
        }

        private static int CountNearbyEnemies(Player player, float radius)
        {
            int count = 0;
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

        private static List<string> GetCurrentBiomes(Player player)
        {
            var biomes = new List<string>();
            if (player.ZoneForest) biomes.Add("Forest");
            if (player.ZoneCorrupt) biomes.Add("Corruption");
            if (player.ZoneCrimson) biomes.Add("Crimson");
            if (player.ZoneJungle) biomes.Add("Jungle");
            if (player.ZoneSnow) biomes.Add("Snow");
            if (player.ZoneDesert) biomes.Add("Desert");
            if (player.ZoneHallow) biomes.Add("Hallow");
            if (player.ZoneDungeon) biomes.Add("Dungeon");
            if (player.ZoneUnderworldHeight) biomes.Add("Underworld");
            if (player.ZoneSkyHeight) biomes.Add("Sky");
            if (player.ZoneGlowshroom) biomes.Add("Glowshroom");
            // Add more zones if needed
            return biomes;
        }
    }
}
