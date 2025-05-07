
using System;

namespace RL_API{
    public class TileInfo(float tileType, float liquidType, float liquidAmount, float brightness)
    {
        public float tileType { get; set; } = tileType;
        public float liquidType { get; set; } = liquidType;
        public float liquidAmount { get; set; } = liquidAmount;
        public float brightness { get; set; } = brightness;
    }
}