using System;
using System.Text;
using Terraria.ModLoader;

namespace RL_API
{
    public static class RLObsLogger
    {
        public static void Log(RLObservation obs, string prefix = "[RLObs]")
        {
            string log = obs != null ? ToSimpleString(obs) : "NULL OBSERVATION";
            ModContent.GetInstance<RL_API>().Logger.Info($"{prefix} {log}");
        }

        public static string ToSimpleString(RLObservation obs)
        {
            var sb = new StringBuilder();

            // --- Combat ---
            sb.Append("Health:").Append(obs.Health).Append(", ");
            sb.Append("MaxHealth:").Append(obs.MaxHealth).Append(", ");
            sb.Append("Mana:").Append(obs.Mana).Append(", ");
            sb.Append("MaxMana:").Append(obs.MaxMana).Append(", ");

            // --- Movement ---
            sb.Append("Vel:").Append(obs.VelocityX).Append(",").Append(obs.VelocityY).Append(", ");
            sb.Append("Pos:").Append(obs.PositionX).Append(",").Append(obs.PositionY).Append(", ");

            // --- Status ---
            sb.Append("InventoryOpen:").Append(obs.IsInventoryOpen).Append(", ");
            sb.Append("InTown:").Append(obs.IsInTown).Append(", ");
            sb.Append("ChestOpen:").Append(obs.IsChestOpen).Append(", ");
            sb.Append("Breath:").Append(obs.Breath).Append(", ");
            sb.Append("Wet:").Append(obs.IsWet).Append(", ");
            sb.Append("Gills:").Append(obs.HasGills).Append(", ");
            sb.Append("NightVision:").Append(obs.HasNightVision).Append(", ");
            sb.Append("NoFall:").Append(obs.HasNoFallDmg).Append(", ");
            sb.Append("NoKnockback:").Append(obs.HasNoKnockback).Append(", ");
            sb.Append("Hooked:").Append(obs.IsHooked).Append(", ");
            sb.Append("Burning:").Append(obs.IsBurning).Append(", ");
            sb.Append("Facing:").Append(obs.FacingDirection).Append(", ");

            // --- Summons ---
            sb.Append("Minions:").Append(obs.ActiveMinionsCount).Append(", ");

            // --- Equipment ---
            sb.Append("Armor(H/C/L):").Append(obs.HeadArmorType).Append(',')
              .Append(obs.ChestArmorType).Append(',').Append(obs.LegArmorType).Append(", ");
            sb.Append("Defense:").Append(obs.TotalDefense).Append(", ");

            // --- Accessories ---
            sb.Append("Accessories:").Append(string.Join("|", obs.AccessoryTypes ?? [])).Append(", ");

            // --- Ammo ---
            sb.Append("Arrows:").Append(obs.ArrowCount).Append(", Gel:").Append(obs.GelCount).Append(", ");

            // --- Held item ---
            sb.Append("Held:").Append(string.Join("|", obs.HeldItemType ?? [])).Append(", ");

            // --- Mount/Pet ---
            sb.Append("Hook:").Append(obs.HookType).Append(", Mount:").Append(obs.MountType)
              .Append(", LightPet:").Append(obs.LightPetType).Append(", Pet:").Append(obs.PetType)
              .Append(", Cart:").Append(obs.Cart).Append(", ");

            // --- Events ---
            sb.Append("BloodMoon:").Append(obs.IsBloodMoon).Append(", BossActive:").Append(obs.IsBossActive).Append(", ");

            // --- Time/Biome ---
            sb.Append("TimeCycle:").Append(obs.TimeCycle).Append(", Weather:").Append(obs.WeatherEventType).Append(", ");
            sb.Append("Biomes:").Append(string.Join("|", obs.CurrentBiomes ?? [])).Append(", ");

            // --- Enemies ---
            sb.Append("NearbyEnemies:").Append(obs.NearbyEnemiesCount).Append(", EnemyVec:")
              .Append(string.Join(" ", obs.NearbyEnemyInfo ?? [])).Append(", ");

            // --- Buffs ---
            sb.Append("Buffs:").Append(string.Join("|", obs.BuffsVector ?? [])).Append(", ");

            // --- Items ---
            sb.Append("NearItems:").Append(obs.NearItems?.Length ?? 0).Append(", ");

            // --- Inventory ---
            //sb.Append("InvSlots:").Append(obs.InvState?.Flattened?.Length ?? 0).Append(", ");

            // --- Tiles ---
            sb.Append("TilesAround:").Append(string.Join("|", obs.TilesAround ?? [])).Append(", ");

            // --- Crafts ---
            sb.Append("Crafts:").Append(string.Join("|", obs.Crafts ?? [])).Append(", ");

            return sb.ToString();
        }
    }
}
