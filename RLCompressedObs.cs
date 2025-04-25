public class RLCompressedObs
{
    public float[] PlayerInfo { get; set; }    // [VelocityX, VelocityY, PosX, PosY, HealthRatio, ManaRatio]
    public byte[] TileTypes { get; set; }       // Flattened 1D array of tile types around the player
    public byte HeldItemType { get; set; }
    public byte NearbyEnemies { get; set; }
    public sbyte FacingDirection { get; set; }  // -1 = left, 1 = right
    public byte TotalDefense { get; set; }
    public byte[] BuffIds { get; set; }
    public byte[] AccessoryIds { get; set; }
    public byte CurrentBiome { get; set; } 
    public byte HookType { get; set; }
    public byte MountType { get; set; }
    public byte LightPetType { get; set; }
    public byte PetType { get; set; }     // Single biome ID (for now, or most important one)
}
