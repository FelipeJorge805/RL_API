using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;

public class RLItemObservation
{
    public int ItemId { get; set; }
    public int StackSize { get; set; }
    public float DistanceToPlayer { get; set; }
    public bool IsPickupReady { get; set; }
}

namespace RL_API
    {
    public static class ItemScanner
    {
        public static List<RLItemObservation> ScanNearbyItems(Player player, float scanRadius, int maxItems)
        {
            var nearbyItems = new List<RLItemObservation>();

            foreach (var item in Main.item)
            {
                if (item.active)
                {
                    float dist = Vector2.Distance(item.position, player.Center);

                    if (dist <= scanRadius)
                    {
                        nearbyItems.Add(new RLItemObservation
                        {
                            ItemId = item.type,
                            StackSize = item.stack,
                            DistanceToPlayer = dist,
                            IsPickupReady = item.noGrabDelay == 0
                        });
                    }
                }
            }

            // Sort by closest first
            var sortedItems = nearbyItems.OrderBy(x => x.DistanceToPlayer).Take(maxItems).ToList();

            // Pad if fewer than maxItems
            while (sortedItems.Count < maxItems)
            {
                sortedItems.Add(new RLItemObservation
                {
                    ItemId = -1, // Special ID for "no item"
                    StackSize = 0,
                    DistanceToPlayer = -1f,
                    IsPickupReady = false
                });
            }

            return sortedItems;
        }
    }
}