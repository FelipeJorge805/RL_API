using Terraria;

public class InventorySlotInfo
{
    public int ItemType { get; set; }
    public int StackSize { get; set; }
}

namespace RL_API
{
    public class InventoryState
    {
        public InventorySlotInfo[] Slots = new InventorySlotInfo[50];
        public InventoryState()
        {
            for (int i = 0; i < 50; i++)
            {
                Slots[i] = new InventorySlotInfo
                {
                    ItemType = 0,
                    StackSize = 0
                };
            }
        }
        public void Capture(Player player)
        {
            for (int slot = 0; slot < 50; slot++)
            {
                var item = player.inventory[slot];

                Slots[slot] = new InventorySlotInfo
                {
                    ItemType = item?.type ?? 0,
                    StackSize = item?.stack ?? 0
                };
            }
        }

        public int GetItemTypeAtSlot(int slot)
        {
            return (slot >= 0 && slot < Slots.Length) ? Slots[slot].ItemType : 0;
        }

        public int GetStackSizeAtSlot(int slot)
        {
            return (slot >= 0 && slot < Slots.Length) ? Slots[slot].StackSize : 0;
        }
    }
}