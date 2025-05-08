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
                ItemID.SnowBlock,

                // --- Walls ---
                ItemID.StoneWall,
                ItemID.DirtWall,
                ItemID.WoodWall,
                ItemID.StoneWall,
                ItemID.GlassWall,

                // --- Wood ---
                ItemID.Wood,
                ItemID.Torch,
                ItemID.Campfire,
                ItemID.WoodPlatform,
                ItemID.WoodenChair,
                ItemID.WoodenTable,
                ItemID.WoodenDoor,
                ItemID.WoodenBow,
                ItemID.WoodenSword,
                ItemID.WoodenHammer,
                ItemID.WoodenBoomerang,
                ItemID.WoodBreastplate,
                ItemID.WoodHelmet,
                ItemID.WoodGreaves,

                // --- Copper ---
                ItemID.CopperOre,
                ItemID.CopperBar,
                ItemID.CopperPickaxe,
                ItemID.CopperAxe,
                ItemID.CopperHammer,
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
                ItemID.TinHammer,
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
                ItemID.IronHammer,
                ItemID.IronShortsword,
                ItemID.IronBroadsword,
                ItemID.IronHelmet,
                ItemID.IronChainmail,
                ItemID.IronGreaves,
                ItemID.IronAnvil,
                ItemID.EmptyBucket,
                ItemID.Chain,

                // --- Crafting Stations ---
                ItemID.WorkBench,
                ItemID.Furnace,
                ItemID.IronAnvil,
                ItemID.LeadAnvil,
                ItemID.Loom,
                ItemID.SharpeningStation,
                ItemID.Sawmill,

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
                ItemID.ThornsPotion,
                ItemID.EndurancePotion,
                ItemID.NightOwlPotion,
                ItemID.ArcheryPotion,
                ItemID.HunterPotion,
                ItemID.RecallPotion,
                ItemID.FeatherfallPotion,
                ItemID.ObsidianSkinPotion,
                ItemID.MagicPowerPotion,
                ItemID.MiningPotion,
                ItemID.SpelunkerPotion,
                ItemID.GillsPotion,
                ItemID.SonarPotion,
                ItemID.InvisibilityPotion,

                // --- Potion Materials ---
                ItemID.Mushroom,
                ItemID.Daybloom,
                ItemID.Moonglow,
                ItemID.Blinkroot,
                ItemID.Gel,
                ItemID.BottledWater,
                ItemID.BottledHoney,
                ItemID.Bottle,
                ItemID.Deathweed,
                ItemID.Waterleaf,
                ItemID.Fireblossom,
                ItemID.Cactus,
                ItemID.BottledHoney,
                ItemID.Stinger,
                ItemID.Vine,
                ItemID.JungleSpores,

                // --- Lead ---
                ItemID.LeadOre,
                ItemID.LeadBar,
                ItemID.LeadPickaxe,
                ItemID.LeadAxe,
                ItemID.LeadHammer,
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
                ItemID.SilverHammer,
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
                ItemID.TungstenHammer,
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
                ItemID.GoldHammer,
                ItemID.GoldShortsword,
                ItemID.GoldBroadsword,
                ItemID.GoldHelmet,
                ItemID.GoldChainmail,
                ItemID.GoldGreaves,
                ItemID.GoldWatch,

                // --- Platinum ---
                ItemID.PlatinumOre,
                ItemID.PlatinumBar,
                ItemID.PlatinumPickaxe,
                ItemID.PlatinumAxe,
                ItemID.PlatinumHammer,
                ItemID.PlatinumShortsword,
                ItemID.PlatinumBroadsword,
                ItemID.PlatinumHelmet,
                ItemID.PlatinumChainmail,
                ItemID.PlatinumGreaves,
                ItemID.PlatinumWatch,

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
                ItemID.GlowingMushroom,        

                // --- Meteorite ---
                ItemID.Meteorite,
                ItemID.MeteoriteBar,
                ItemID.MeteorHelmet,
                ItemID.MeteorSuit,
                ItemID.MeteorLeggings,
                ItemID.SpaceGun,
                ItemID.MeteorShot,
                ItemID.MeteorStaff,
                ItemID.MeteorHamaxe,
                ItemID.RedPhaseblade,
                ItemID.BluePhaseblade,
                ItemID.GreenPhaseblade,
                ItemID.YellowPhaseblade,
                ItemID.PurplePhaseblade,
                ItemID.WhitePhaseblade,
                ItemID.OrangePhaseblade,

                // --- Hellstone ---
                ItemID.Hellstone,
                ItemID.HellstoneBar,
                ItemID.MoltenHelmet,
                ItemID.MoltenBreastplate,
                ItemID.MoltenGreaves,
                ItemID.MoltenPickaxe,
                ItemID.FieryGreatsword,
                ItemID.MoltenFury,
                ItemID.MoltenHamaxe,
                ItemID.MoltenCharm,
                ItemID.Flamarang,
                ItemID.Hellforge,
                ItemID.HellfireArrow,
                ItemID.ImpStaff,
                ItemID.PhoenixBlaster,
                ItemID.Sunfury,

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
                ItemID.WarAxeoftheNight,
                ItemID.TheBreaker,
                ItemID.BallOHurt,

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
                ItemID.TheRottedFork,

                // --- Food ---
                ItemID.PumpkinPie,
                ItemID.BowlofSoup,
                ItemID.CookedFish,

                // --- Misc ---
                ItemID.Cobweb,
                ItemID.Silk,
                ItemID.Bone,
                ItemID.CookingPot,
                ItemID.Bowl,
                ItemID.Bed,
                ItemID.ManaCrystal,
                ItemID.EnchantedBoomerang,
                ItemID.EnchantedSword,

                // --- Obsidian ---
                ItemID.Obsidian,
                ItemID.ObsidianShield,
                ItemID.ObsidianSkull,
                ItemID.ObsidianPants,
                ItemID.ObsidianHelm,
                ItemID.ObsidianChest,

                // --- Jungle ---
                ItemID.JungleSpores,
                ItemID.Stinger,
                ItemID.BladeofGrass,
                ItemID.JungleHat,
                ItemID.JungleShirt,
                ItemID.JunglePants,
                ItemID.IvyWhip,
                ItemID.ThornChakram,

                // --- Altars ---
                ItemID.BloodySpine,
                ItemID.SuspiciousLookingEye,
                ItemID.SlimeCrown,
                ItemID.WormFood,
                ItemID.NightsEdge,

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
