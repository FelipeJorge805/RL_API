using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            RLObservation obs = RLObsCollector.CollectObservation(Player);
            var compressedObs = RLObsCompressor.Compress(obs);
            string json = JsonSerializer.Serialize(compressedObs);
            ConnectionManager.EnqueueObservation(json);

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

        public float CalculatePickupReward(Player player)
        {
            float reward = 0f;

            var currentInventory = new InventoryState();
            currentInventory.Capture(player);

            var pickups = InventoryState.Compare(lastInventoryState, currentInventory);

            foreach (var (itemType, amountPickedUp) in pickups)
            {
                int newCount = currentInventory.GetCount(itemType);
                reward += BlockGatherReward.GetRewardForPickup(itemType, newCount) * amountPickedUp;
            }

            lastInventoryState = currentInventory; // Update snapshot

            return reward;
        }

    }
}