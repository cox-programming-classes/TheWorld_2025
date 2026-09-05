using The_World.GameData.Areas;

namespace The_World.GameData.GameMechanics;

// Import factory methods for easier access!
using static CreatureFactory;

/// <summary>
/// Builds the entire world: every area, creature, NPC, and item,
/// wired together into one map.
///
///                        [Lich's Sanctum]*
///                              | (locked: Barrow Key)
///   [Hermit's Hut]       [Crypt Depths]
///        |                     |
///   [Sunny Clearing]     [Barrow Crypt]
///        |                     |
///   [Dark Forest]------[Ancient Ruins]
///     |   |   \                |
///     |   |  [Old Mill]  [Mountain Foothills]
///     |   |                    |
///     | [Goblin Cave]          |
///     |      |                 |
///     |  [Chief's Den]         |
///   [Willowbrook Village]------+
///        |
///   [Rusty Flagon Tavern]
/// </summary>
public static class WorldBuilder
{
    /// <summary>
    /// Initialize the WHOLE WORLD here.
    /// </summary>
    /// <returns>The starting area (Willowbrook Village).</returns>
    public static Area BuildWorld()
    {
        // ── The safe places ────────────────────────────────────────────────

        var village = AreaBuilder
            .FromName("Willowbrook Village")
            .WithArt(AsciiArt.Areas.Village)
            .WithDescription(
                "A cluster of thatched roofs around a mossy well. Chickens patrol the lanes with an air of authority. " +
                "The moot hall steps hold Elder Maera; Bram's Provisions leans companionably against the tavern.")
            .AsSafeZone()
            .WithCreature("elder_maera", NpcFactory.ElderMaera())
            .WithCreature("merchant_bram", NpcFactory.MerchantBram())
            .Build();

        var tavern = AreaBuilder
            .FromName("Rusty Flagon Tavern")
            .WithArt(AsciiArt.Areas.Tavern)
            .WithDescription(
                "Low beams, a roaring hearth, and the competing smells of stew and spilled ale. " +
                "A gambler runs dice at the corner table while the barkeep keeps order with a look.")
            .AsSafeZone()
            .WithCreature("barkeep_hulda", NpcFactory.BarkeepHulda())
            .WithCreature("finn", NpcFactory.FinnTheGambler())
            .Build();

        var clearing = AreaBuilder
            .FromName("Sunny Clearing")
            .WithArt(AsciiArt.Areas.Clearing)
            .WithDescription(
                "A bright clearing bathed in sunlight, with soft grass and colorful flowers. " +
                "After the gloom of the forest, it feels like surfacing for air.")
            .AsSafeZone()
            .WithItem("healing_herb", ItemFactory.HealingHerb())
            .WithItem("healing_herb_2", ItemFactory.HealingHerb())
            .WithCreature("deer", Deer())
            .WithHiddenItem("moonpetal", ItemFactory.MoonpetalBlossom())
            .Build();

        var hut = AreaBuilder
            .FromName("Hermit's Hut")
            .WithArt(AsciiArt.Areas.Hut)
            .WithDescription(
                "A crooked little dwelling of driftwood and river stone, herbs drying under the eaves. " +
                "It smells of woodsmoke, sage, and long silences.")
            .AsSafeZone()
            .WithCreature("hermit_odo", NpcFactory.HermitOdo())
            .Build();

        // ── The wilds ──────────────────────────────────────────────────────

        var forest = AreaBuilder
            .FromName("Dark Forest")
            .WithArt(AsciiArt.Areas.Forest)
            .WithDescription(
                "A gloomy forest filled with towering trees and eerie sounds. The canopy swallows the sun; " +
                "something small and green snickers in the undergrowth.")
            .WithCreature("goblin_1", BuildGoblinArchetype())
            .WithCreature("goblin_2", BuildGoblinArchetype(
                "Goblin Scout", "A nimble goblin with keen eyes, always on the lookout for intruders."))
            .WithCreature("wolf", Wolf())
            .WithItem("rusty_sword", ItemFactory.RustySword())
            .Build();

        var mill = AreaBuilder
            .FromName("Old Mill")
            .WithArt(AsciiArt.Areas.Mill)
            .WithDescription(
                "A broken waterwheel groans in the current beside a sagging mill house. " +
                "Someone has been living here - and judging by the bones by the fire pit, not politely.")
            .WithCreature("bandit", Bandit())
            .WithCreature("spider", GiantSpider())
            .WithItem("silver_locket", ItemFactory.SilverLocket())
            .WithHiddenItem("stashed_potion", ItemFactory.HealingPotion())
            .Build();

        var goblinCave = AreaBuilder
            .FromName("Goblin Cave")
            .WithArt(AsciiArt.Areas.Cave)
            .WithDescription(
                "A reeking cave mouth fringed with gnawed bones and crude fetishes. Guttural voices " +
                "echo from deeper in, arguing over something. Probably dinner.")
            .WithCreature("warrior_1", GoblinWarrior())
            .WithCreature("warrior_2", GoblinWarrior("Goblin Guard"))
            .Build();

        var chiefsDen = AreaBuilder
            .FromName("Chief's Den")
            .WithArt(AsciiArt.Areas.ChiefsDen)
            .WithDescription(
                "The heart of the goblin warren, lit by a guttering fire. Trophies of a dozen raids hang " +
                "from the walls - and there, atop a heap of plunder, sits a strongbox stamped 'BRAM'S PROVISIONS'.")
            .WithCreature("goblin_chief", GoblinChief())
            .WithItem("stolen_goods", ItemFactory.StolenGoods())
            .WithItem("healing_potion", ItemFactory.HealingPotion())
            .Build();

        var foothills = AreaBuilder
            .FromName("Mountain Foothills")
            .WithArt(AsciiArt.Areas.Foothills)
            .WithDescription(
                "Windswept slopes of heather and scree climbing toward grey peaks. A cairn-marked trail " +
                "winds north; wolf tracks stitch back and forth across it.")
            .WithCreature("dire_wolf", DireWolf())
            .Build();

        var ruins = AreaBuilder
            .FromName("Ancient Ruins")
            .WithArt(AsciiArt.Areas.Ruins)
            .WithDescription(
                "Shattered columns and fallen archways of some elder civilization, half-swallowed by moss. " +
                "In the courtyard's center, an enormous stone figure stands too still to be a statue.")
            .WithCreature("stone_golem", StoneGolem())
            .WithHiddenItem("runed_warstaff", ItemFactory.RunedWarstaff())
            .WithHiddenItem("ancient_tome", ItemFactory.AncientTome())
            .Build();

        // ── The Barrow ─────────────────────────────────────────────────────

        var crypt = AreaBuilder
            .FromName("Barrow Crypt")
            .WithArt(AsciiArt.Areas.Crypt)
            .WithDescription(
                "Beneath the barrow mound, cold air breathes up a stairway of black stone. Niches line the " +
                "walls, their occupants long-since risen and not at all happy about visitors.")
            .WithCreature("skeleton_1", Skeleton())
            .WithCreature("skeleton_2", Skeleton("Skeletal Guardian"))
            .Build();

        var depths = AreaBuilder
            .FromName("Crypt Depths")
            .WithArt(AsciiArt.Areas.Depths)
            .WithDescription(
                "The stair ends in a vaulted hall of tombs. Frost furs every surface despite the airless " +
                "still. At the far end looms an iron gate wrought with warding sigils, and before it drifts a gaunt shape.")
            .WithCreature("crypt_wight", CryptWight())
            .WithItem("chain_mail", ItemFactory.ChainMail())
            .WithLockedExit(
                "gate",
                "Barrow Key",
                "The iron gate is sealed fast. A keyhole of antique design sits at its center - the old Barrow Key would fit it.")
            .Build();

        var sanctum = AreaBuilder
            .FromName("Lich's Sanctum")
            .WithArt(AsciiArt.Areas.Sanctum)
            .WithDescription(
                "A round chamber below the roots of the hill, lit by candles that burn green and cast no heat. " +
                "Ranks of ancient books rot on the shelves. Upon a throne of grave-goods sits the crowned corpse of Malakhar, " +
                "and his empty eyes are already on you.")
            .WithCreature("lich", LichMalakhar())
            .Build();

        // ── Wire it all together (every passage goes both ways) ────────────

        AreaBuilder.Connect(village, "tavern", tavern, "outside");
        AreaBuilder.Connect(village, "east", forest, "west");
        AreaBuilder.Connect(village, "north", foothills, "south");
        AreaBuilder.Connect(forest, "east", clearing, "west");
        AreaBuilder.Connect(forest, "north", goblinCave, "out");
        AreaBuilder.Connect(forest, "south", mill, "north");
        AreaBuilder.Connect(clearing, "east", hut, "west");
        AreaBuilder.Connect(goblinCave, "deeper", chiefsDen, "out");
        AreaBuilder.Connect(foothills, "north", ruins, "south");
        AreaBuilder.Connect(ruins, "east", crypt, "west");
        AreaBuilder.Connect(crypt, "down", depths, "up");
        AreaBuilder.Connect(depths, "gate", sanctum, "gate");

        return village;
    }
}
