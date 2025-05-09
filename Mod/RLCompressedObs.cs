using System;
using System.Collections.Generic;


// This is a static class to flatten the observation into a float[]
// It follows the format of the RLObservation attributes from top to bottom.
namespace RL_API
{
    public class RLCompressedObs
    {
        //public float[] Data { get; } = Flatten(obs);

        public static float[] Flatten(RLObservation obs)
        {
            var floats = new List<float>();

            // --- Combat Info ---
            floats.Add(obs.Health);
            floats.Add(obs.MaxHealth);
            floats.Add(obs.Mana);
            floats.Add(obs.MaxMana);

            // --- Player Movement ---
            floats.Add(obs.VelocityX);
            floats.Add(obs.VelocityY);
            floats.Add(obs.PositionX);
            floats.Add(obs.PositionY);

            // --- Status Flags ---
            floats.Add(obs.IsInventoryOpen);
            floats.Add(obs.IsInTown);
            floats.Add(obs.IsChestOpen);
            floats.Add(obs.Breath);
            floats.Add(obs.IsWet);
            floats.Add(obs.HasGills);
            floats.Add(obs.HasNightVision);
            floats.Add(obs.HasNoFallDmg);
            floats.Add(obs.HasNoKnockback);
            floats.Add(obs.IsHooked);
            floats.Add(obs.IsBurning);
            floats.Add(obs.FacingDirection);

            // --- Summons ---
            floats.Add(obs.ActiveMinionsCount);

            // --- Equipment ---
            floats.Add(obs.HeadArmorType);
            floats.Add(obs.ChestArmorType);
            floats.Add(obs.LegArmorType);
            floats.Add(obs.TotalDefense);

            // --- Accessories ---
            if (obs.AccessoryTypes != null)
                floats.AddRange(obs.AccessoryTypes);

            // --- Ammo ---
            floats.Add(obs.ArrowCount);
            floats.Add(obs.GelCount);

            // --- Held Item ---
            floats.Add(obs.HeldItemType);

            floats.Add(obs.PetType);
            floats.Add(obs.LightPetType);
            floats.Add(obs.Cart);
            floats.Add(obs.MountType);
            floats.Add(obs.HookType);

            // --- Events / Bosses ---
            floats.Add(obs.IsBloodMoon);
            floats.Add(obs.IsBossActive);

            // --- Time ---
            floats.Add(obs.TimeCycle);

            // --- Enemies ---
            floats.Add(obs.NearbyEnemiesCount);
            if (obs.NearbyEnemyInfo != null)
                floats.AddRange(obs.NearbyEnemyInfo);

            // --- Biomes / Weather ---
            floats.Add(obs.WeatherEventType);
            if (obs.CurrentBiomes != null)
                floats.AddRange(obs.CurrentBiomes);

            // --- Buffs ---
            if (obs.BuffsVector != null)
                floats.AddRange(obs.BuffsVector);

            // --- Nearby Items ---
            if (obs.NearItems != null)
                floats.AddRange(obs.NearItems);

            // --- Inventory ---
            if (obs.InvState != null)
                floats.AddRange(CompressInventory(obs.InvState));

            // --- Tiles ---
            if (obs.TilesAround != null)
                floats.AddRange(obs.TilesAround);

            return [.. floats];
        }
        private static float[] CompressInventory(InventoryState invState)
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
    }
}
