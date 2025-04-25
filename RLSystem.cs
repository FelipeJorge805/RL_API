using Terraria.ModLoader;

namespace RL_API
{
    public class RLSystem : ModSystem
    {
        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
            ConnectionManager.Initialize("127.0.0.1", 5000);
        }
        public override void OnWorldUnload()
        {
            base.OnWorldUnload();
            ConnectionManager.Close();
        }
    }
}
