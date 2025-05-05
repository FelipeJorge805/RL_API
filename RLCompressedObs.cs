using System;
using System.Collections.Generic;

namespace RL_API
{
    public class RLCompressedObs
    {
        public float[] NearItems { get; set; }
        public float[] InvState { get; set; }
        public float[] PlayerInfo { get; set; }    // [VelocityX, VelocityY, PosX, PosY, HealthRatio, ManaRatio]
        public float[] TilesAround { get; set; }       // Flattened 1D array of tile types around the player
        public float HeldItemType { get; set; }
        public float NearbyEnemies { get; set; }
        public float FacingDirection { get; set; }  // -1 = left, 1 = right
        public float TotalDefense { get; set; }
        public float[] BuffIds { get; set; }
        public float[] AccessoryIds { get; set; }
        public float[] CurrentBiome { get; set; } 
        public float HookType { get; set; }
        public float MountType { get; set; }
        public float LightPetType { get; set; }
        public float PetType { get; set; }     // Single biome ID (for now, or most important one)
        public float[] Flatten()
        {
            var floats = new List<float>();

            if (NearItems != null)
                floats.AddRange(NearItems);

            if (InvState != null)
                floats.AddRange(InvState);

            if (PlayerInfo != null)
                floats.AddRange(PlayerInfo);

            if (TilesAround != null)
                floats.AddRange(Array.ConvertAll(TilesAround, b => (float)b));

            floats.Add((float)HeldItemType);
            floats.Add((float)NearbyEnemies);
            floats.Add((float)FacingDirection);
            floats.Add((float)TotalDefense);

            if (BuffIds != null)
                floats.AddRange(Array.ConvertAll(BuffIds, b => (float)b));

            if (AccessoryIds != null)
                floats.AddRange(Array.ConvertAll(AccessoryIds, b => (float)b));

            floats.AddRange(CurrentBiome);
            floats.Add((float)HookType);
            floats.Add((float)MountType);
            floats.Add((float)LightPetType);
            floats.Add((float)PetType);

            return floats.ToArray();
        }

    }
    
}