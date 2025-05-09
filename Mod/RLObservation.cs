using System;
using System.Collections.Generic;

namespace RL_API
{
    public class RLObservation
    {
        //possible crafts
        //sort inv button / quick stack // shift delete trashcan
        //add small incentive for exploring a tile every x tiles, maybe 25?
        //glowstick on the ground? spike balls
        //ensure gravestones are counted in tileScan

        // Combat info
        public float Health { get; set; }
        public float MaxHealth { get; set; }
        public float Mana { get; set; }
        public float MaxMana { get; set; }
        
        // Player movement
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }

        // Status
        public float IsInventoryOpen { get; set; }
        public float IsInTown { get; set; }
        public float IsChestOpen { get; set; }
        public float Breath { get; set; }
        public float IsWet { get; set; }
        public float HasGills { get; set; }
        public float HasNightVision { get; set; }
        public float HasNoFallDmg { get; set; }
        public float HasNoKnockback { get; set; }
        public float IsHooked { get; set; }
        public float IsBurning { get; set; } 
        public float FacingDirection { get; set; } // -1 = left, 1 = right

        // Summons
        public float ActiveMinionsCount { get; set; }

        // Equipment
        public float HeadArmorType { get; set; }
        public float ChestArmorType { get; set; }
        public float LegArmorType { get; set; }
        public float TotalDefense { get; set; }

        // Accessories
        public float[] AccessoryTypes { get; set; }

        // Ammo / WRONG! NEED MOAR! SILLY AI
        public float ArrowCount { get; set; }
        public float GelCount { get; set; }

        // Held item
        public float HeldItemType { get; set; }    

        public float PetType { get; set; }
        public float LightPetType { get; set; }
        public float Cart { get; set; }
        public float MountType { get; set; }
        public float HookType { get; set; }

        // Events / Bosses / WRONG! NEED MOAR! SILLY AI
        public float IsBloodMoon { get; set; }
        public float IsBossActive { get; set; }

        // Day or Night
        public float TimeCycle { get; set; } // -1 if unknown, 0–1 = day, 1–2 = night

        // Enemies nearby
        public float NearbyEnemiesCount { get; set; }
        public float[] NearbyEnemyInfo { get; set; } // flat array: [x1, y1, id1, ...]  

        // Biome info
        public float WeatherEventType { get; set; }
        public float[] CurrentBiomes { get; set; }

        // Buffs and Debuffs
        public float[] BuffsVector { get; set; }

        // Nearby dropped items/coins/loot
        public float[] NearItems { get; set; } // List<RLItemObservation>

        // Current inventory
        public InventoryState InvState { get; set;}

        // Nearby blocks
        public float[] TilesAround { get; set; } // List<TileInfo>
    }
}
