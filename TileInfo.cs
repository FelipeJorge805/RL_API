
namespace RL_API{
    public class TileInfo(int tileType, int liquidType, int liquidAmount)
    {
        private int tileType = tileType;
        private int liquidType = liquidType;
        private int liquidAmount = liquidAmount;

        public byte TileType { get; internal set; }
    }
}