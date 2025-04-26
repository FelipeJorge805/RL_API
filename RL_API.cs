using Terraria.ModLoader;

namespace RL_API
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class RL_API : Mod
	{
		public override void Load()
        {
			base.Load();
        }
        /*public void saveLog(){
			Logger.Info(playerPos);
			ModContent.GetInstance<RL_API>().Logger.Info("This is a log message.");
		}*/
        public override void Unload()
        {
            base.Unload();
			ConnectionManager.Close();
        }
	}
}