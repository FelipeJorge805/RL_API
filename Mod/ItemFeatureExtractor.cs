using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace RL_API
{
    public static class ItemFeatureExtractor
    {
        private static readonly HashSet<int> UsefulStations =
        [
            TileID.WorkBenches,
            TileID.Furnaces,
            TileID.Anvils,
            TileID.CookingPots,
            TileID.Campfire,
            TileID.Bottles,
            TileID.Tables,
            TileID.Chairs,
        ];

        public const int VectorSize = 8;

        /// <summary>
        /// Extracts a normalized 8-float feature vector from a Terraria Item.
        /// </summary>
        /// <param name="item">The Terraria Item object.</param>
        /// <returns>float[8] representing [stack, is_block, is_usable, power, use_time, quality, is_buildable, is_consumable]</returns>
        public static float[] ExtractItemData(Item item)
        {
            float stack = Math.Clamp(item.stack / 999f, 0f, 1f);

            float isBlock = item.createTile > 0 ? 1f : 0f;

            float isUsable = (item.pick > 0 || item.axe > 0 || item.hammer > 0 || item.damage > 0 || item.useStyle > 0) ? 1f : 0f;

            float power = MathF.Max(item.pick, MathF.Max(item.axe, MathF.Max(item.hammer, item.damage))) / 300f;

            float useTime = item.useTime / 60f;

            float rarity = item.rare / 12f;
            float value = MathF.Log(item.value + 1) / 10f;
            float quality = (rarity + value) / 2f;

            float isBuildable = (item.createTile >= 0 && UsefulStations.Contains(item.createTile)) ? 1f : 0f;

            float isConsumable = item.consumable ? 1f : 0f;

            return
            [
                stack,
                isBlock,
                isUsable,
                power,
                useTime,
                quality,
                isBuildable,
                isConsumable
            ];
        }
    }
}
