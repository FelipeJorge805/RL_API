using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Configuration;
using Terraria.DataStructures;
using System.Diagnostics.Tracing;
using Terraria.Graphics.Effects;
using Terraria.GameContent.Achievements;

namespace RL_API
{
    class RLPlayer() : ModPlayer
    {
        private RLObservation prevObs;
        private RLObservation currentObs;
        private float[] prevCompressedObs;        
        private float[] currentCompressedObs;
        private readonly int hertz = 6; // This means 10 ticks per second (60fps/6)
        private AgentAction lastAction;
        private AgentAction newAction;

        private int discardCooldown = 600;

        private float rewardAccumulator = 0;

        private  bool IsDeadLastTick;

        public override void OnEnterWorld()
        {
            base.OnEnterWorld();
            while(Player == null && Player.inventory == null) { } //wait for inventory to be initialized
            //lastInventoryState = new InventoryState(Player.inventory);
            lastAction = AgentAction.Create("still","none",[0f,0f],false);
        }
        
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

            if (lastAction?.Cursor != null)
            {
                var (mouseX, mouseY, tileTarget) = MapCursorBounds(
                    lastAction.Cursor[0], lastAction.Cursor[1], Player.Center, 8f * 16f
                );
                
                //Set every tick
                Player.tileTargetX = tileTarget.X;
                Player.tileTargetY = tileTarget.Y;
                
                if (Main.GameUpdateCount % hertz != 0) return;
                //set only every hertz tick (10/s currently)
                Main.mouseX = mouseX;
                Main.mouseY = mouseY;
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
                case "esc":
                    Main.playerInventory = !Main.playerInventory; // toggle inv
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
                case "craft":
                    CraftRecipe();
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
                    //rewardAccumulator += hotbarSwapSelectedReward(newAction.Action);
                    break;
                case "scroll_up":
                    {
                        if(Main.playerInventory)
                            Main.focusRecipe = Math.Max(0, Main.focusRecipe - 1);
                        else
                            rewardAccumulator += hotbarSwapSelectedReward(newAction.Action);
                    }
                    break;
                case "scroll_down":
                    {
                        if(Main.playerInventory)
                            Main.focusRecipe = Math.Min(Main.availableRecipe.Length - 1, Main.focusRecipe + 1);
                        else
                            rewardAccumulator += hotbarSwapSelectedReward(newAction.Action);
                    }
                    break;
                case "none":
                    // Do nothing
                    break;
            }

        }

        private void CraftRecipe()
        {
            if (Main.playerInventory &&
                Main.focusRecipe >= 0 &&
                Main.focusRecipe < Main.availableRecipe.Length)
            {
                var recipe = Main.recipe[Main.availableRecipe[Main.focusRecipe]];

                recipe.Create();
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
            
            // Handle movement
            if(lastAction==null)return;
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
        }
		public override void PreUpdate()
    	{
			//base.PreUpdate();
			if (Player.whoAmI != Main.myPlayer) return;
            Main.hasFocus = true;

            // Handle movement
            if(lastAction==null)return;
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
                UpdateObs();                
                SendObservation(isDone: true);
                UpdateState();
                IsDeadLastTick = isDeadNow;
                return;
            }

            // Normal hertz control
            if (Main.GameUpdateCount % hertz != 0)
                return;

            //Main.NewText($"Agent running for player {Player.name}, myPlayer: {Main.myPlayer}");
            //ModContent.GetInstance<RL_API>().Logger.Info($"Agent running for player {Player.name}, myPlayer: {Main.myPlayer}");
            //Main.NewText("Reward: " + rewardAccumulator);
            UpdateObs();
            if(prevObs!=null) 
                rewardAccumulator += RewardCalculator.Calculate(prevObs,currentObs);
            SendObservation(isDone: false);
            UpdateState();

            if(Main.GameUpdateCount % 600 == 0) //every 10 seconds
            {
                RLObsLogger.Log(currentObs, prefix: "[RLObs]");
            }
            IsDeadLastTick = isDeadNow;
		}
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            //base.HideDrawLayers(drawInfo);
            drawInfo.cHead = 0;
            drawInfo.hideEntirePlayer = true;
            drawInfo.hideHair = true;
            drawInfo.hidesBottomSkin = true;
            drawInfo.hidesTopSkin = true;
            drawInfo.hideCompositeShoulders = true;
            drawInfo.DrawDataCache.Clear();
        }
        /*public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            //base.ModifyDrawInfo(ref drawInfo);
            Player.hideMisc[0] = true;
            Player.hideVisibleAccessory = [];
        }*/
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            //base.ModifyDrawInfo(ref drawInfo);
            drawInfo.hideEntirePlayer = true;
            //drawInfo.drawPlayer = null; // lol segfault
            drawInfo.DrawDataCache.Clear();
        }
        public override void FrameEffects()
        {
            //base.FrameEffects();
            Player.head = -1;
            Player.body = -1;
            Player.legs = -1;
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            //base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            drawInfo.DrawDataCache.Clear();
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
        
        public static (int mouseX, int mouseY, Point tileTarget) MapCursorBounds(
            float nnX, float nnY, Vector2 playerCenter, float radiusPx)
        {
            nnX = Math.Clamp(nnX, -1f, 1f);
            nnY = Math.Clamp(nnY, -1f, 1f);

            float offsetX = nnX * radiusPx;
            float offsetY = nnY * radiusPx;

            float absX = Math.Abs(offsetX);
            float absY = Math.Abs(offsetY);
            if (absX + absY > radiusPx)
            {
                float scale = radiusPx / (absX + absY);
                offsetX *= scale;
                offsetY *= scale;
            }

            float worldX = playerCenter.X + offsetX;
            float worldY = playerCenter.Y + offsetY;

            int mouseX = (int)(worldX - Main.screenPosition.X);
            int mouseY = (int)(worldY - Main.screenPosition.Y);
            Point tileTarget = new((int)(worldX / 16f), (int)(worldY / 16f));

            return (mouseX, mouseY, tileTarget);
        }

        private void UpdateObs()
        {
            // Gather full Observation
            currentObs = RLObsCollector.CollectObservation(Player);

            // Compress
            currentCompressedObs = RLCompressedObs.Flatten(currentObs);

            /*if (prevObs == null)
            {
                prevObs = currentObs;
                prevCompressedObs = currentCompressedObs;
            }
            else
            {
                //rewardAccumulator += RewardCalculator.CalculatePickupReward(previousInv: lastInventoryState, currentInv: currentInventoryState);
                rewardAccumulator += RewardCalculator.Calculate(previous:prevObs,current:currentObs); 
                //wrong, should not be here anymore
            }*/
        }

        private void SendObservation(bool isDone)
        {
            if (prevCompressedObs == null)//(This is for the first tick on spawn)
            {
                prevCompressedObs = currentCompressedObs;
                string inputjson = "{\"input_size\":"+prevCompressedObs.Length+"}";
                //ModContent.GetInstance<RL_API>().Logger.Info("Sending Packet: "+inputjson);
                ConnectionManager.EnqueueObservation(inputjson); //send only the input size on the first tick of world join
                return; 
            }

            var EnvStepPacket = new RLStepPacket
            {
                Obs = prevCompressedObs,
                Reward = rewardAccumulator,
                NextObs = currentCompressedObs,
                Done = isDone
            };

            string json = JsonSerializer.Serialize(EnvStepPacket);
            //ModContent.GetInstance<RL_API>().Logger.Info("Sending Packet: "+json);
            ConnectionManager.EnqueueObservation(json);
        }
        public void UpdateState(){
            // Update state for next tick
            prevObs = currentObs;
            prevCompressedObs = currentCompressedObs;
            rewardAccumulator = 0f;
            lastAction = ConnectionManager.ConsumeAction();
        }
        public override void Unload()
        {
            RLObsLogger.Log(currentObs, prefix: "[RLObs]");
            base.Unload();
            ConnectionManager.Close();
        }

        public float hotbarSwapSelectedReward(string action){
            int hotbarSlot = action.Split('_')[1] == "up" ? +1 : -1; // scroll up or down
            Player.selectedItem = ( Player.selectedItem + hotbarSlot + 10 ) % 10; // wrap around (+10 is for edge case of -1)
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
