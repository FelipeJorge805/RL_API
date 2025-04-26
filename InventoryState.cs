using System.Collections.Generic;
using Terraria;

namespace RL_API
{
    public class InventoryState
    {
        public Dictionary<int, int> ItemCounts { get; private set; } = new Dictionary<int, int>();

        public void Capture(Player player)
        {
            ItemCounts.Clear();

            foreach (var item in player.inventory)
            {
                if (item != null && item.stack > 0)
                {
                    if (!ItemCounts.ContainsKey(item.type))
                        ItemCounts[item.type] = 0;

                    ItemCounts[item.type] += item.stack;
                }
            }
        }

        public int GetCount(int itemType)
        {
            return ItemCounts.TryGetValue(itemType, out var count) ? count : 0;
        }

        public static List<(int itemType, int amountPickedUp)> Compare(InventoryState previous, InventoryState current)
        {
            var pickups = new List<(int, int)>();

            foreach (var (type, countNow) in current.ItemCounts)
            {
                int countBefore = previous.GetCount(type);
                if (countNow > countBefore)
                {
                    int amountPickedUp = countNow - countBefore;
                    pickups.Add((type, amountPickedUp));
                }
            }

            return pickups;
        }
    }
}