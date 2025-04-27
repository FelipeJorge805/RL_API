using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace RL_API
{
    class RLPlayer() : ModPlayer
    {
        List<TileInfo> tiles;
        int hertz = 1;
        //private static string lastKnownAction = "none";
        AgentAction lastAction;
        AgentAction newAction;

        int discardCooldown = 600;

        float rewardAccumulator = 0;
        private InventoryState lastInventoryState;

        public override void OnEnterWorld()
        {
            base.OnEnterWorld();
            lastInventoryState = new InventoryState(Player.inventory);
            lastAction = new AgentAction("still","none",[0f,0f],false);
        }
        public override void SetControls()
		{
            if(discardCooldown>=0)discardCooldown--;
            newAction = ConnectionManager.PeekAction();
            //Main.NewText("action: " + action.action + " move: " + action.move + " shift: " + action.shift + " cursor: " + action.cursor);

            //shift modifier
            if(newAction!= null && lastAction.Shift != newAction.Shift) lastAction.Shift = newAction.Shift;
            if(newAction!= null && newAction.Shift) Player.controlSmart = true;

            if(newAction!= null && lastAction.Move!=newAction.Move) lastAction.Move = newAction.Move; 
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

            switch (newAction.Action)
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
                    int hotbarSlot = int.Parse(newAction.Action.Split('_')[1]);
                    Player.selectedItem = hotbarSlot;
                    Item selectedItem = Player.inventory[Player.selectedItem];
                    bool isHotbarSlotEmpty = selectedItem == null || selectedItem.stack == 0 || selectedItem.type == ItemID.None;
                    if(isHotbarSlotEmpty){
                        rewardAccumulator -= 1f;
                    }
                    break;
                case "none":
                    // Do nothing
                    break;
            }

        }

        public override void PreUpdateBuffs()
        {
            base.PreUpdateBuffs();
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
        }

        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
        }
		public override void PreUpdate()
    	{
			//base.PreUpdate();
			
		}
		public override void PostUpdate()
		{
            //run every tick
            if(newAction==null && lastAction!=null){
                Main.mouseX = (int)lastAction.Cursor[0];
                Main.mouseY = (int)lastAction.Cursor[1];
            }else if(newAction!=null){
                (Main.mouseX , Main.mouseY) = MapCursorBounds(newAction.Cursor[0],newAction.Cursor[1],Player.Center,16f*16f); //16 blocks * 16 pixels per block: radius
            }
            
            if(Main.GameUpdateCount % hertz != 0) return;

            //Check new inventory and calculate pick-up rewards
            InventoryState newInventoryState = new(Player.inventory);
            if(lastInventoryState==null){
                lastInventoryState = newInventoryState;
            }
            else
                rewardAccumulator += CalculatePickupReward(previousInv:lastInventoryState,
                                                        currentInv:newInventoryState);

            //Gather Obs and send
            RLObservation obs = RLObsCollector.CollectObservation(Player);
            obs.InvState=newInventoryState;
            
            var compressedObs = RLObsCompressor.Compress(obs);
            var FlatObs = compressedObs.Flatten(); //might be useless
            //Main.NewText(FlatObs.Length);
            Main.NewText("Reward: " + rewardAccumulator);

            var EnvStep = new {
                obs = FlatObs,
                reward = rewardAccumulator
            };
            
            string json = JsonSerializer.Serialize(EnvStep);
            ConnectionManager.EnqueueObservation(json);

            //update State
            rewardAccumulator = 0f;
            lastInventoryState = newInventoryState;
            lastAction = ConnectionManager.ConsumeAction();
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



    }
}