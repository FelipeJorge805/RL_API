using System;
using System.Collections.Generic;
using Terraria.ID;

public static class BlockGatherReward
{
    private static readonly HashSet<int> LinearRewardBlocks = new HashSet<int>
    {
        TileID.Copper, TileID.Tin, TileID.Iron, TileID.Lead,
        TileID.Silver, TileID.Tungsten, TileID.Gold, TileID.Platinum,
        TileID.Demonite, TileID.Crimtane, TileID.Meteorite, TileID.Hellstone,
        TileID.WoodBlock, TileID.PalmTree, TileID.RichMahogany, TileID.Ebonwood, TileID.Shadewood
    };

    private static readonly float linearDecayRate = 500f;
    private static readonly float expoDecayRate = -0.01f;

    public static float GetRewardForPickup(int itemType, int inventoryAmount)
    {
        // Special pickups
        if (itemType == ItemID.Heart)
            return 5f; // Heart pickup reward
        if (itemType == ItemID.Star)
            return 2f; // Mana pickup reward

        // Coin pickup reward
        if (itemType == ItemID.CopperCoin)
            return 0.1f;
        if (itemType == ItemID.SilverCoin)
            return 1f;
        if (itemType == ItemID.GoldCoin)
            return 100f;
        if (itemType == ItemID.PlatinumCoin)
            return 10_000f; 

        // REMOVE THIS LATER BRUV AAHHH
        // Normal block gathering
        if (inventoryAmount <= 0)
            return 1.0f;

        if (LinearRewardBlocks.Contains(itemType))
        {
            // Linear decay if its a ore/wood
            return MathF.Max(1f - (inventoryAmount / linearDecayRate), 0f);
        }
        else
        {
            // Exponential decay if its a sh!t block
            return MathF.Exp(expoDecayRate * inventoryAmount);
        }
    }
}
