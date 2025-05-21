//calculate rewards based on last Obs and new Obs..
//take in 2 obs objects and return reward for changes
//stuff like hp increased, call block mined reward from here
//inventory change calculation from here
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace RL_API
{
    public static class RewardCalculator
    {
        public static bool quickBuffPressed = false;
        public static float Calculate(RLObservation previous, RLObservation current)
        {
            float reward = 0f;

            // Example reward for increased HP
            //if (current.Health > previous.Health)
            //    reward += 0.1f * (current.Health - previous.Health);

            // Example reward for block/item pickup
            reward += CalculatePickupReward(previous.InvState, current.InvState);

            reward += CalculateBuffReward(prevBuffs: previous.BuffsVector, currBuffs: current.BuffsVector);

            // Max Health Reward
            if (current.MaxHealth > previous.MaxHealth) reward += (current.MaxHealth - previous.MaxHealth) / 2f;

            // Max Mana Reward
            if (current.MaxMana > previous.MaxMana) reward += (current.MaxMana - previous.MaxMana) / 2f;

            // Minion Reward
            if (current.ActiveMinionsCount > previous.ActiveMinionsCount) reward += 5f;

            if (current.TotalDefense > previous.TotalDefense) reward += current.TotalDefense - previous.TotalDefense;

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
        public static float CalculateBuffReward(float[] prevBuffs, float[] currBuffs)
        {
            float reward = 0f;
            HashSet<float> seen = [];

            int len = Math.Min(prevBuffs.Length, currBuffs.Length);

            for (int i = 0; i < len; i += 2)
            {
                float currType = currBuffs[i];
                float currTime = currBuffs[i + 1];
                float prevType = prevBuffs[i];
                float prevTime = prevBuffs[i + 1];

                if (currType == 0f)
                    continue;

                float absType = Math.Abs(currType);
                if (!seen.Add(absType))
                    continue;

                bool isBuff = currType > 0f;
                bool isDebuff = currType < 0f;

                if (isBuff)
                {
                    if (prevType == 0f) // If this buff is new
                    {
                        reward += 0.05f;
                        if (quickBuffPressed) // if it's new through quickBuff()
                            reward += 0.1f;
                    }
                    else if (currType == prevType && currTime > prevTime + 0.01f) // Not new but was refreshed
                    {
                        reward += 0.02f;
                    }
                }
                else if (isDebuff)
                {
                    if (prevType == 0f) // if debuff is new
                    {
                        reward -= 0.1f;
                    }
                    else if (currType == prevType && currTime > prevTime) // Not new but was refreshed
                    {
                        reward -= 0.05f;
                    }
                }
            }
            ModContent.GetInstance<RL_API>().Logger.Info("Buff reward: " + reward);
            quickBuffPressed = false;
            return reward;
        }

    }
}
