using The_World.GameData.Abilities;
using The_World.GameData.Creatures;

namespace The_World.GameData.GameMechanics;

/// <summary>
/// Creates common Creature archetypes for reuse.
/// This helps avoid duplication of code when creating
/// similar creatures in multiple areas or scenarios.
/// </summary>
public static class CreatureFactory
{
    /// <summary>
    /// Helper method to build a Goblin creature archetype.
    /// You might use this in multiple areas.
    /// </summary>
    public static Creature BuildGoblinArchetype(
        string name = "Goblin",
        string description = "A small, green humanoid creature with sharp teeth and a mischievous grin.",
        int level = 1)
        => new(name, description,
            new StatChart(8 + 4 * level, 0, Strength: 8, Dexterity: 13, Intelligence: 8),
            level,
            XP: 4 + level)
        {
            DamageDice = new Dice(1, 4),
            Gold = level * 3,
            AttackVerb = "jabs a crude spear at",
            Art = AsciiArt.Creatures.Goblin
        };

    public static Creature Wolf(string name = "Grey Wolf") => new(
        name, "Lean and yellow-eyed, it circles you with unsettling patience.",
        new StatChart(14, 0, Strength: 12, Dexterity: 14, Intelligence: 4),
        Level: 2, XP: 7)
    {
        DamageDice = new Dice(1, 6),
        AttackVerb = "snaps its jaws at",
        Loot = [ItemFactory.WolfPelt()],
        Art = AsciiArt.Creatures.Wolf
    };

    public static Creature DireWolf() => new(
        "Dire Wolf", "A wolf the size of a pony, with scars that suggest it usually wins.",
        new StatChart(30, 0, Strength: 15, Dexterity: 14, Intelligence: 5),
        Level: 4, XP: 12)
    {
        DamageDice = new Dice(1, 8, 1),
        AttackVerb = "lunges, jaws wide, at",
        Loot = [ItemFactory.WolfPelt()],
        Art = AsciiArt.Creatures.Wolf
    };

    public static Creature GoblinWarrior(string name = "Goblin Warrior") => new(
        name, "A fierce goblin clad in makeshift armor, wielding a crude but well-used blade.",
        new StatChart(18, 0, Strength: 12, Dexterity: 12, Intelligence: 8),
        Level: 2, XP: 8)
    {
        DamageDice = new Dice(1, 6),
        NaturalArmor = 1,
        Gold = 8,
        AttackVerb = "slashes at",
        Art = AsciiArt.Creatures.Goblin
    };

    public static Creature GoblinChief() => new(
        "Goblin Chief Grubnash", "Twice the size of his kin, draped in stolen finery and radiating bad temper. A heavy iron key hangs from his belt.",
        new StatChart(40, 10, Strength: 15, Dexterity: 12, Intelligence: 9),
        Level: 5, XP: 20)
    {
        DamageDice = new Dice(1, 8, 2),
        NaturalArmor = 2,
        Gold = 30,
        AttackVerb = "swings a spiked club at",
        Loot = [ItemFactory.BarrowKey(), ItemFactory.SilverLocket()],
        Art = AsciiArt.Creatures.GoblinChief,
        Special = new Ability("Savage Howl", "A frenzied flurry of blows.",
            5, new Dice(2, 6, 2), AbilityKind.Damage, AbilityScaling.Strength,
            "Grubnash howls with rage and rains blows down on {0}!")
    };

    public static Creature Bandit() => new(
        "Bandit", "A ragged highwayman with a rusty cutlass and poor career prospects.",
        new StatChart(24, 0, Strength: 13, Dexterity: 13, Intelligence: 10),
        Level: 3, XP: 10)
    {
        DamageDice = new Dice(1, 6, 1),
        Gold = 15,
        AttackVerb = "slashes wildly at",
        Art = AsciiArt.Creatures.Bandit
    };

    public static Creature GiantSpider() => new(
        "Giant Spider", "Eight eyes, eight legs, and far too many teeth for anyone's comfort.",
        new StatChart(20, 0, Strength: 11, Dexterity: 15, Intelligence: 3),
        Level: 3, XP: 10)
    {
        DamageDice = new Dice(1, 8),
        AttackVerb = "sinks venomous fangs toward",
        Art = AsciiArt.Creatures.Spider
    };

    public static Creature Skeleton(string name = "Skeleton") => new(
        name, "Yellowed bones held together by spite and old magic. Its jaw hangs at a jaunty angle.",
        new StatChart(22, 0, Strength: 11, Dexterity: 9, Intelligence: 6),
        Level: 4, XP: 12)
    {
        DamageDice = new Dice(1, 6),
        NaturalArmor = 2,
        AttackVerb = "swings a notched blade at",
        Art = AsciiArt.Creatures.Skeleton
    };

    public static Creature StoneGolem() => new(
        "Stone Golem", "A hulking statue that has decided, ponderously, that you should not be here.",
        new StatChart(50, 0, Strength: 18, Dexterity: 6, Intelligence: 3),
        Level: 6, XP: 18)
    {
        DamageDice = new Dice(2, 6),
        NaturalArmor = 4,
        AttackVerb = "brings a granite fist down on",
        Loot = [ItemFactory.AmberRing()],
        Art = AsciiArt.Creatures.Golem
    };

    public static Creature CryptWight() => new(
        "Crypt Wight", "A gaunt figure in grave-clothes, eyes burning with cold blue fire.",
        new StatChart(45, 20, Strength: 14, Dexterity: 11, Intelligence: 14),
        Level: 7, XP: 25)
    {
        DamageDice = new Dice(1, 8, 1),
        NaturalArmor = 2,
        Gold = 25,
        AttackVerb = "rakes grave-cold claws at",
        Loot = [ItemFactory.GreaterHealingPotion()],
        Art = AsciiArt.Creatures.Wight,
        Special = new Ability("Chilling Touch", "A grasp that drinks warmth and life.",
            6, new Dice(2, 6), AbilityKind.Damage, AbilityScaling.Intelligence,
            "The wight seizes {0} with fingers of ice - your very warmth drains away!")
    };

    /// <summary>
    /// The final boss. Slaying him wins the game.
    /// </summary>
    public static Creature LichMalakhar() => new(
        "Lich Malakhar", "Once the kingdom's greatest scholar; now a crowned corpse wreathed in green sorcery. His grin has outlasted his lips.",
        new StatChart(85, 60, Strength: 10, Dexterity: 10, Intelligence: 18),
        Level: 10, XP: 50)
    {
        DamageDice = new Dice(1, 10, 2),
        NaturalArmor = 3,
        Gold = 150,
        AttackVerb = "rakes skeletal claws at",
        IsFinalBoss = true,
        Loot = [ItemFactory.Phylactery()],
        Art = AsciiArt.Creatures.Lich,
        Special = new Ability("Soul Drain", "Tears at the seam between body and spirit.",
            8, new Dice(3, 8), AbilityKind.Damage, AbilityScaling.Intelligence,
            "Malakhar speaks a word that predates mercy - your soul is wrenched toward his outstretched hand!")
    };

    /// <summary>
    /// A peaceful creature - can't be fought, only startled.
    /// </summary>
    public static Creature Deer() => new(
        "Deer", "A doe grazing at the clearing's edge, ears twitching at your every step.",
        new StatChart(8, 0, Strength: 6, Dexterity: 16, Intelligence: 4),
        Level: 1, XP: 0)
    {
        IsHostile = false,
        AttackVerb = "stares reproachfully at",
        Art = AsciiArt.Creatures.Deer
    };
}
