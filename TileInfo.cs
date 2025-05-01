
using System;

namespace RL_API{
    public class TileInfo(int tileType, int liquidType, int liquidAmount, float brightness)
    {
        private int tileType = tileType;
        private int liquidType = liquidType;
        private int liquidAmount = liquidAmount;
        private float brightness = brightness;

        public byte TileType { get; internal set; }

        public int LiquidType { get; internal set; }

        public int LiquidAmount { get; internal set; }

        public float Brightness { get; internal set; }

        internal double getBrightness()
        {
            return Math.Round(brightness,2);
        }
    }
}