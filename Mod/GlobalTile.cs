using Terraria;
using Terraria.ModLoader;

namespace RL_API
{
    public class NoRenderTile : GlobalTile
    {
        public override bool PreDraw(int i, int j, int type, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            return false; // Skip tile rendering and associated lighting
        }
    }

    public class NoRenderWall : GlobalWall
    {
        public override bool PreDraw(int i, int j, int type, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            return false; // Skip wall rendering and lighting
        }
    }
}