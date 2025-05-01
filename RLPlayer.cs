using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Configuration;

namespace RL_API
{
    class RLPlayer() : ModPlayer
    {
        float[] prevObs;
        //bool isDone = false;
        //List<TileInfo> tiles;
        int hertz = 1;
        //private static string lastKnownAction = "none";
        AgentAction lastAction;
        AgentAction newAction;

        int discardCooldown = 600;

        float rewardAccumulator = 0;
        private InventoryState lastInventoryState;

        public bool IsDeadLastTick;

        public override void OnEnterWorld()
        {
            base.OnEnterWorld();
            while(Player == null && Player.inventory == null) { } //wait for inventory to be initialized
            lastInventoryState = new InventoryState(Player.inventory);
            lastAction = AgentAction.Create("still","none",[0f,0f],false);
        }
        
        /*public override void CopyClientState(ModPlayer targetCopy)
        {
            base.CopyClientState(targetCopy);
            while(Player == null && targetCopy == null && Player.active) { } //wait for inventory to be initialized
        }*/
        public override void SetControls()
		{
            if (Player.whoAmI != Main.myPlayer) return;
            Main.hasFocus = true; // Set focus to true to prevent the game from pausing when alt-tabbing
            if(discardCooldown>=0)discardCooldown--;
            newAction = ConnectionManager.PeekAction();
            
            if(lastAction==null){ // this is for edge cases like initial world join
                lastAction=newAction;
                return;
            }
            
            if(newAction!=null){
                lastAction.Move = newAction.Move;
                lastAction.Action = newAction.Action;
                lastAction.Cursor = newAction.Cursor;
                lastAction.Shift = newAction.Shift;
            }

            //shift modifier
            if(lastAction.Shift) Player.controlSmart = true;

            // Handle movement
            switch (lastAction.Move)
            {
                case "left":
                    Player.controlLeft = true;
                    break;
                case "right":
                    Player.controlRight = true;
                    break;
                case "up":
                    Player.controlUp = true;
                    break;
                case "down":
                    Player.controlDown = true;
                    break;
                case "still":
                    // Do nothing
                    break;
            }

            if (newAction == null) return; //was up top but I want to hold movement and shift keys for more ticks

            switch (newAction.Action) //deliberate use of newAction instead of last
            {
                case "use_item":
                    Player.controlUseItem = true;
                    break;
                case "jump":
                    Player.controlJump = true;
                    break;
                case "quick_heal":
                    Player.QuickHeal();
                    break;
                case "quick_mana":
                    Player.QuickMana();
                    break;
                case "quick_buff":
                    Player.QuickBuff();
                    break;
                case "swap_hotbar":
                    //SwapHotbarItems(); // Your custom function
                    break;
                case "discard":
                    rewardAccumulator -= 0.1f;
                    break;
                    if(discardCooldown<0){
                        Player.controlThrow = true;
                        rewardAccumulator -= 2f;
                        discardCooldown = 600;
                    }
                    break;
                case "grapple":
                    Player.controlHook = true;
                    break;
                case "interact":
                    Player.controlUseTile = true;
                    break;
                case "mount":
                    Player.controlMount = true;
                    break;
                case "hotbar_0":
                case "hotbar_1":
                case "hotbar_2":
                case "hotbar_3":
                case "hotbar_4":
                case "hotbar_5":
                case "hotbar_6":
                case "hotbar_7":
                case "hotbar_8":
                case "hotbar_9":
                    rewardAccumulator += hotbarSwapSelectedReward(newAction.Action);
                    break;
                case "none":
                    // Do nothing
                    break;
            }

        }

        public override void PreUpdateBuffs()
        {
            base.PreUpdateBuffs();
            if (Player.whoAmI != Main.myPlayer) return;
            Main.hasFocus = true;
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
            if (Player.whoAmI != Main.myPlayer) return;
            Main.hasFocus = true;
        }

        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (Player.whoAmI != Main.myPlayer) return;
            Main.hasFocus = true;
        }
		public override void PreUpdate()
    	{
			//base.PreUpdate();
			if (Player.whoAmI != Main.myPlayer) return;
            Main.hasFocus = true;
		}
		public override void PostUpdate()
		{
            //Main.NewText($"[Tick {Main.GameUpdateCount}] hasFocus: {Main.hasFocus}");
            if (Player.whoAmI != Main.myPlayer) return;
            Main.hasFocus = true;
            //Main.NewText($"Focus: {Main.hasFocus}, player: {Player.whoAmI == Main.myPlayer}");
            bool isDeadNow = Player.dead;
            //Player.whoAmI = Main.LocalPlayer

            if (!IsDeadLastTick && isDeadNow)
            {
                Main.NewText("Dead! haha noob AI");
                rewardAccumulator -= 10f;
                SendObservation(isDone: true);
                UpdateState();                
                IsDeadLastTick = isDeadNow;
                return;
            }

            //run every tick
            if (lastAction?.Cursor != null)
            {
                (Main.mouseX, Main.mouseY) = MapCursorBounds(
                    lastAction.Cursor[0],
                    lastAction.Cursor[1],
                    Player.Center,
                    8f * 16f // 8 tiles * 16 pixels
                );
            }


            // Normal hertz control
            if (Main.GameUpdateCount % hertz != 0)
                return;

            //Main.NewText($"Agent running for player {Player.name}, myPlayer: {Main.myPlayer}");
            //ModContent.GetInstance<RL_API>().Logger.Info($"Agent running for player {Player.name}, myPlayer: {Main.myPlayer}");
            //Main.NewText("Reward: " + rewardAccumulator);
            SendObservation(isDone: false);
            UpdateState();

            IsDeadLastTick = isDeadNow;
		}
        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
            int damageTaken = info.Damage;

            if (damageTaken > 0)
            {
                rewardAccumulator -= damageTaken * 0.2f; // Penalize 0.2 per damage taken
            }
        }
        /*public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (victim is NPC targetNpc)
            {
                if (targetNpc.damage > 0)
                {
                    rewardAccumulator += damage * 0.05f;
                }

                if (targetNpc.life <= 0 && !targetNpc.active)
                {
                    rewardAccumulator += 5f;
                }
            }
        }*/

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPCWithItem(item, target, hit, damageDone);
            if (damageDone > 0)
            {
                rewardAccumulator += damageDone * 0.1f; // Reward 0.1 per damage done
            }

            // Bonus for kill
            if (target.life <= 0 && target.active == false)
            {
                rewardAccumulator += 5f; // Big reward for killing enemy
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPCWithProj(proj, target, hit, damageDone);
            if (damageDone > 0)
            {
                rewardAccumulator += damageDone * 0.1f; // Reward 0.1 per damage done
            }

            // Bonus for kill
            if (target.life <= 0 && target.active == false)
            {
                rewardAccumulator += 5f; // Big reward for killing enemy
            }
        }
        public static (int, int) MapCursorBounds(float nnX, float nnY, Vector2 playerCenter, float radius)
        {
            // Clamp inputs just in case (optional)
            nnX = Math.Clamp(nnX, -1f, 1f);
            nnY = Math.Clamp(nnY, -1f, 1f);

            // Map [-1, 1] to circle around player
            float offsetX = nnX * radius;
            float offsetY = nnY * radius;

            float cursorX = playerCenter.X + offsetX;
            float cursorY = playerCenter.Y + offsetY;

            return ((int)(cursorX - Main.screenPosition.X), (int)(cursorY-Main.screenPosition.Y));
        }

        public static float CalculatePickupReward(InventoryState previousInv, InventoryState currentInv)
        {
            float reward = 0f;

            var changes = currentInv.CompareTo(previousInv); // List<(int itemType, int amountPickedUp)>

            foreach (var (itemType, amountPickedUp) in changes)
            {
                float rewardPerItem = BlockGatherReward.GetRewardForPickup(itemType,amountPickedUp);
                reward += rewardPerItem * amountPickedUp;
            }

            return reward;
        }
        private void UpdateState()
        {
            // Capture new inventory
            currentInventoryState = new(Player.inventory);

            if (lastInventoryState == null)
            {
                lastInventoryState = currentInventoryState;
            }
            else
            {
                rewardAccumulator += CalculatePickupReward(previousInv: lastInventoryState, currentInv: currentInventoryState);
            }

            // Gather full Observation
            RLObservation currentObs = RLObsCollector.CollectObservation(Player);
            currentObs.InvState = currentInventoryState;

            // Compress
            var compressed = RLObsCompressor.Compress(currentObs);
            currentCompressedObs = compressed.Flatten(); // flatten if needed
        }
        float[] currentCompressedObs;
        InventoryState currentInventoryState;
        private void SendObservation(bool isDone)
        {
            if (prevObs == null)
            {
                prevObs = currentCompressedObs;
                return; // Wait another tick before sending (This is for the first tick on spawn)
            }

            var EnvStepPacket = new RLStepPacket
            {
                Obs = prevObs,
                Reward = rewardAccumulator,
                NextObs = currentCompressedObs,
                Done = isDone
            };

            string json = JsonSerializer.Serialize(EnvStepPacket);
            ConnectionManager.EnqueueObservation(json);

            // Update state for next tick
            prevObs = currentCompressedObs;
            rewardAccumulator = 0f;
            lastInventoryState = currentInventoryState;
            lastAction = ConnectionManager.ConsumeAction();
        }
        public override void Unload()
        {
            base.Unload();
            ConnectionManager.Close();
        }

        public float hotbarSwapSelectedReward(string action){
            int hotbarSlot = int.Parse(action.Split('_')[1]);
            Player.selectedItem = hotbarSlot;
            Item selectedItem = Player.inventory[Player.selectedItem];
            bool isHotbarSlotEmpty = selectedItem == null || selectedItem.stack == 0 || selectedItem.type == ItemID.None;
            if(isHotbarSlotEmpty){
                return -0.2f;
            } else return .1f; //can't be same otherwise model stuck swaping focus all the time
        }
    }
}
public class RLStepPacket
{
    public float[] Obs { get; set; }
    public float Reward { get; set; }
    public float[] NextObs { get; set; }
    public bool Done { get; set; }
}
