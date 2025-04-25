using System.Collections.Generic;

namespace RL_API
{
    public class RLObservation
    {
        // Player movement
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }

        // Tile scan
        public List<TileInfo> TilesAround { get; set; }

        // Combat info
        public int Health { get; set; }
        public int Mana { get; set; }

        // Equipment
        public int HeadArmorType { get; set; }
        public int ChestArmorType { get; set; }
        public int LegArmorType { get; set; }
        public int TotalDefense { get; set; }

        // Accessories
        public List<int> AccessoryTypes { get; set; }

        // Buffs and Debuffs
        public List<int> ActiveBuffTypes { get; set; }
        public List<int> ActiveDebuffTypes { get; set; }

        // Summons
        public int ActiveMinionsCount { get; set; }

        // Events / Bosses
        public bool IsBloodMoon { get; set; }
        public bool IsBossActive { get; set; }

        // Player states
        public bool IsFalling { get; set; }
        public bool IsHooked { get; set; }
        public bool IsKnockedBack { get; set; }

        // Ammo
        public int ArrowCount { get; set; }
        public int GelCount { get; set; }

        // Held item
        public int HeldItemType { get; set; }

        // Enemies nearby
        public int NearbyEnemiesCount { get; set; }

        // Player direction
        public int FacingDirection { get; set; } // -1 = left, 1 = right

        // Biome info
        public List<string> CurrentBiomes { get; set; }

        public int HookType { get; set; }
        public int MountType { get; set; }
        public int LightPetType { get; set; }
        public int PetType { get; set; }
    }
}
