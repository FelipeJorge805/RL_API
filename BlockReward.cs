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
        //TODO:
        //item type for most items should be reduced to categories: isPickaxe, isAxe, isBlock, etc.
        //reward for putting a pickaxe in the hotbar should be given according to pickaxe power
        //rewarding a pickaxe swap in the same slot should be calculated based on pickaxe power
        //new axe/sword in inventory? calculate reward based on axe power or damage (or also the difference in power based on existing tools)

        // Special pickups
        if (itemType == ItemID.Heart)
            return 5f; // Heart pickup reward
        if (itemType == ItemID.Star)
            return 2f; // Mana pickup reward

        // Coin pickup reward
        if (itemType == ItemID.CopperCoin)
            return 0.01f * inventoryAmount;
        if (itemType == ItemID.SilverCoin)
            return 0.1f * inventoryAmount;
        if (itemType == ItemID.GoldCoin)
            return 1f * inventoryAmount;
        if (itemType == ItemID.PlatinumCoin)
            return 100f * inventoryAmount; 

        // REMOVE THIS LATER
        // Normal block gathering
        if (inventoryAmount <= 0)
            return 1.0f * inventoryAmount;

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
