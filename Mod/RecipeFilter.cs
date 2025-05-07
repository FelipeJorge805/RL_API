using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RL_API
{
    public class RecipeFilter : ModSystem
    {
        public override void PostAddRecipes()
        {
            var allowedItems = new HashSet<int>
            {
                // --- Blocks ---
                ItemID.DirtBlock,
                ItemID.StoneBlock,
                ItemID.SandBlock,
                ItemID.ClayBlock,
                ItemID.MudBlock,

                // --- Wood ---
                ItemID.Wood,
                ItemID.WoodPlatform,
                ItemID.WoodenChair,
                ItemID.WoodenTable,
                ItemID.WorkBench,
                ItemID.WoodenDoor,
                ItemID.WoodenBow,
                ItemID.WoodenSword,

                // --- Copper ---
                ItemID.CopperOre,
                ItemID.CopperBar,
                ItemID.CopperPickaxe,
                ItemID.CopperAxe,
                ItemID.CopperShortsword,
                ItemID.CopperBroadsword,
                ItemID.CopperHelmet,
                ItemID.CopperChainmail,
                ItemID.CopperGreaves,

                // --- Tin ---
                ItemID.TinOre,
                ItemID.TinBar,
                ItemID.TinPickaxe,
                ItemID.TinAxe,
                ItemID.TinShortsword,
                ItemID.TinBroadsword,
                ItemID.TinHelmet,
                ItemID.TinChainmail,
                ItemID.TinGreaves,

                // --- Iron ---
                ItemID.IronOre,
                ItemID.IronBar,
                ItemID.IronPickaxe,
                ItemID.IronAxe,
                ItemID.IronShortsword,
                ItemID.IronBroadsword,
                ItemID.IronHelmet,
                ItemID.IronChainmail,
                ItemID.IronGreaves,
                ItemID.IronAnvil,

                // --- Crafting Stations ---
                ItemID.WorkBench,
                ItemID.Furnace,
                ItemID.IronAnvil,
                ItemID.LeadAnvil,

                // --- Ammo ---
                ItemID.WoodenArrow,
                ItemID.FlamingArrow,
                ItemID.Shuriken,
                ItemID.ThrowingKnife,

                // --- Potions ---
                ItemID.LesserHealingPotion,
                ItemID.HealingPotion,
                ItemID.ManaPotion,
                ItemID.LesserManaPotion,
                ItemID.RegenerationPotion,
                ItemID.SwiftnessPotion,
                ItemID.IronskinPotion,

                // --- Potion Materials ---
                ItemID.Mushroom,
                ItemID.Daybloom,
                ItemID.Moonglow,
                ItemID.Blinkroot,
                ItemID.Gel,
                ItemID.BottledWater,
                ItemID.Bottle,

                // --- Lead ---
                ItemID.LeadOre,
                ItemID.LeadBar,
                ItemID.LeadPickaxe,
                ItemID.LeadAxe,
                ItemID.LeadShortsword,
                ItemID.LeadBroadsword,
                ItemID.LeadHelmet,
                ItemID.LeadChainmail,
                ItemID.LeadGreaves,

                // --- Silver ---
                ItemID.SilverOre,
                ItemID.SilverBar,
                ItemID.SilverPickaxe,
                ItemID.SilverAxe,
                ItemID.SilverShortsword,
                ItemID.SilverBroadsword,
                ItemID.SilverHelmet,
                ItemID.SilverChainmail,
                ItemID.SilverGreaves,

                // --- Tungsten ---
                ItemID.TungstenOre,
                ItemID.TungstenBar,
                ItemID.TungstenPickaxe,
                ItemID.TungstenAxe,
                ItemID.TungstenShortsword,
                ItemID.TungstenBroadsword,
                ItemID.TungstenHelmet,
                ItemID.TungstenChainmail,
                ItemID.TungstenGreaves,

                // --- Gold ---
                ItemID.GoldOre,
                ItemID.GoldBar,
                ItemID.GoldPickaxe,
                ItemID.GoldAxe,
                ItemID.GoldShortsword,
                ItemID.GoldBroadsword,
                ItemID.GoldHelmet,
                ItemID.GoldChainmail,
                ItemID.GoldGreaves,

                // --- Platinum ---
                ItemID.PlatinumOre,
                ItemID.PlatinumBar,
                ItemID.PlatinumPickaxe,
                ItemID.PlatinumAxe,
                ItemID.PlatinumShortsword,
                ItemID.PlatinumBroadsword,
                ItemID.PlatinumHelmet,
                ItemID.PlatinumChainmail,
                ItemID.PlatinumGreaves,

                // --- Bars (for crafting stations and weapons) ---
                ItemID.TinBar,
                ItemID.CopperBar,
                ItemID.IronBar,
                ItemID.LeadBar,
                ItemID.SilverBar,
                ItemID.TungstenBar,
                ItemID.GoldBar,
                ItemID.PlatinumBar,

                // --- Other Crafting Materials ---
                ItemID.Glass,
                ItemID.WoodWall,
                ItemID.GlowingMushroom,
                ItemID.Chain,

                // --- More Potion Materials ---
                ItemID.Deathweed,
                ItemID.Waterleaf,
                ItemID.Fireblossom,
                ItemID.Cactus,
                ItemID.BottledHoney,
                ItemID.Stinger,
                ItemID.Vine,
                ItemID.JungleSpores,

                // --- Additional Potions ---
                ItemID.NightOwlPotion,
                ItemID.ArcheryPotion,
                ItemID.HunterPotion,
                ItemID.RecallPotion,
                ItemID.FeatherfallPotion,
                ItemID.ObsidianSkinPotion,
                ItemID.MagicPowerPotion,
                ItemID.MiningPotion,

                // --- Meteorite ---
                ItemID.Meteorite,
                ItemID.MeteoriteBar,
                ItemID.MeteorHelmet,
                ItemID.MeteorSuit,
                ItemID.MeteorLeggings,
                ItemID.SpaceGun,

                // --- Hellstone ---
                ItemID.Hellstone,
                ItemID.HellstoneBar,
                ItemID.MoltenHelmet,
                ItemID.MoltenBreastplate,
                ItemID.MoltenGreaves,
                ItemID.MoltenPickaxe,
                ItemID.FieryGreatsword,

                // --- Hooks ---
                ItemID.GrapplingHook,
                ItemID.AmethystHook,
                ItemID.TopazHook,
                ItemID.SapphireHook,
                ItemID.EmeraldHook,
                ItemID.RubyHook,
                ItemID.DiamondHook,

                // --- Demonite ---
                ItemID.DemoniteOre,
                ItemID.DemoniteBar,
                ItemID.ShadowScale,
                ItemID.NightmarePickaxe,
                ItemID.LightsBane,
                ItemID.ShadowHelmet,
                ItemID.ShadowScalemail,
                ItemID.ShadowGreaves,
                ItemID.DemonBow,

                // --- Crimtane ---
                ItemID.CrimtaneOre,
                ItemID.CrimtaneBar,
                ItemID.TissueSample,
                ItemID.DeathbringerPickaxe,
                ItemID.BloodButcherer,
                ItemID.CrimsonHelmet,
                ItemID.CrimsonScalemail,
                ItemID.CrimsonGreaves,
                ItemID.TheUndertaker,
                ItemID.FleshGrinder,

            };

            foreach (var recipe in Main.recipe)
            {
                if (recipe == null || recipe.createItem == null)
                    continue;

                if (!allowedItems.Contains(recipe.createItem.type))
                {
                    recipe.DisableRecipe();
#if DEBUG
                    ModContent.GetInstance<RL_API>().Logger.Info($"[Blocked Recipe] {recipe.createItem.Name}");
#endif
                }
            }
        }
    }
}
