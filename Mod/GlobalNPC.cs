using Terraria;
using Terraria.ModLoader;

namespace RL_API
{
    public class NoRenderNPC : GlobalNPC
    {
        public override bool PreDraw(NPC npc, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.Vector2 screenPos, Microsoft.Xna.Framework.Color drawColor)
        {
            // Skip rendering for all NPCs and mobs
            // No skipDrawing parameter in the corrected method signature
            
            return false;
             
        }

        public override void DrawEffects(NPC npc, ref Microsoft.Xna.Framework.Color drawColor)
        {
            // Skip effects and set draw color to transparent
            drawColor = Microsoft.Xna.Framework.Color.Transparent;
        }

    }
}