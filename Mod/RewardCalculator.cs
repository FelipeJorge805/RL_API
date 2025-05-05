//calculate rewards based on last Obs and new Obs..
//take in 2 obs objects and return reward for changes
//stuff like hp increased, call block mined reward from here
//inventory change calculation from here
using System;

namespace RL_API
{
    public static class RewardCalculator
    {
        public static float Calculate(RLObservation previous, RLObservation current)
        {
            float reward = 0f;

            // Example reward for increased HP
            //if (current.Health > previous.Health)
            //    reward += 0.1f * (current.Health - previous.Health);

            // Example reward for block/item pickup
            reward += CalculatePickupReward(previous.InvState, current.InvState);

            return reward;
        }

        private static float CalculatePickupReward(InventoryState previousInv, InventoryState currentInv)
        {
            float reward = 0f;

            var changes = currentInv.CompareTo(previousInv); // List<(int itemType, int amountPickedUp)>

            foreach (var (itemType, amountPickedUp) in changes)
            {
                float rewardPerItem = BlockGatherReward.GetRewardForPickup(itemType, amountPickedUp);
                reward += rewardPerItem * amountPickedUp;
            }

            return reward;
        }
    }
}
