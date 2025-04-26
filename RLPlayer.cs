using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.Map;
using Terraria.ModLoader;

namespace RL_API
{
    class RLPlayer() : ModPlayer
    {
        List<TileInfo> tiles;
        int hertz = 1;
        //private static string lastKnownAction = "none";
        String latestMove = "still";
        bool latestShift = false;

        float rewardAccumulator = 0;
        private InventoryState lastInventoryState = new InventoryState();
        public override void SetControls()
		{
            var action = ConnectionManager.PeekAction();


            //shift modifier
            if(action!= null && latestShift != action.shift) latestShift = action.shift;
            if(action!= null && action.shift) Player.controlSmart = true;

            if(action!= null && latestMove!=action.move) latestMove = action.move;  
            // Handle movement
            switch (latestMove)
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

            if (action == null) return; //was up top but I want to hold movement and shift keys for more ticks

            switch (action.action)
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
                    Player.controlThrow = true;
                    rewardAccumulator -= 1f;
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
                    int hotbarSlot = int.Parse(action.action.Split('_')[1]);
                    Player.selectedItem = hotbarSlot;
                    break;
                case "none":
                    // Do nothing
                    break;
            }

            UpdateCursor(Player,action.cursor[0],action.cursor[1]);
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
            if(Main.GameUpdateCount % hertz != 0) return;
            //var data = "{obs: test}";

            InventoryState newInventoryState = new InventoryState();
            newInventoryState.Capture(Player);

            rewardAccumulator += CalculatePickupReward(Player,newInventoryState);

            RLObservation obs = RLObsCollector.CollectObservation(Player);
            obs.addInventoryState(newInventoryState);
            
            var compressedObs = RLObsCompressor.Compress(obs);

            var EnvStep = new {
                obs = compressedObs,
                reward = rewardAccumulator
            };

            string json = JsonSerializer.Serialize(EnvStep);
            ConnectionManager.EnqueueObservation(json);

            rewardAccumulator = 0;
            lastInventoryState = newInventoryState;
            var action = ConnectionManager.ConsumeAction();
            //Main.dedServ=true;
            //Main.drawSkip = true;
            //Lighting.Mode = LightMode.White;
			//base.PostUpdate();
            /*if(Main.GameUpdateCount % 60 == 0){
				tiles = Tile_Scan.scanTiles(Main.LocalPlayer,4);
			}
			if(Main.GameUpdateCount % 60 == 0){
				Main.NewText("Printing... " + Main.GameUpdateCount/60);
				String t = "";
				tiles.ForEach(o => {
                    t+= o.getBrightness() > 0.2f ?
                    o.getBrightness()+"," :
                    "" ;
            });
				Main.NewText(t);
			}*/
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
        public static void UpdateCursor(Player player, float deltaX, float deltaY, float maxDistance = 300f)
        {
            Vector2 playerCenter = player.Center;
            Vector2 desiredWorldPos = playerCenter + new Vector2(deltaX, deltaY) * maxDistance;

            // Clamp to circle
            float dist = Vector2.Distance(playerCenter, desiredWorldPos);
            if (dist > maxDistance)
            {
                Vector2 dir = Vector2.Normalize(desiredWorldPos - playerCenter);
                desiredWorldPos = playerCenter + dir * maxDistance;
            }

            // Convert to screen coords
            Vector2 cursorScreenPos = desiredWorldPos - Main.screenPosition;

            // Clamp inside screen
            int margin = 50;
            cursorScreenPos.X = Math.Clamp(cursorScreenPos.X, margin, Main.screenWidth - margin);
            cursorScreenPos.Y = Math.Clamp(cursorScreenPos.Y, margin, Main.screenHeight - margin);

            // Set Terraria mouse position
            Main.mouseX = (int)cursorScreenPos.X;
            Main.mouseY = (int)cursorScreenPos.Y;
        }
        public float CalculatePickupReward(Player player, InventoryState currentInventory)
        {
            float reward = 0f;

            for (int slot = 0; slot < 50; slot++)
            {
                var previousSlot = lastInventoryState.Slots[slot];
                var currentSlot = currentInventory.Slots[slot];

                if (currentSlot.ItemType == previousSlot.ItemType)
                {
                    int pickedUp = currentSlot.StackSize - previousSlot.StackSize;
                    if (pickedUp > 0)
                    {
                        reward += BlockGatherReward.GetRewardForPickup(currentSlot.ItemType, currentSlot.StackSize) * pickedUp;
                    }
                }
                else
                {
                    // Slot changed to a different item — treat whole stack as pickup
                    if (currentSlot.StackSize > 0)
                    {
                        reward += BlockGatherReward.GetRewardForPickup(currentSlot.ItemType, currentSlot.StackSize);
                    }
                }
            }

            return reward;
        }


    }
}