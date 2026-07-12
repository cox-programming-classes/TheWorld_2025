using The_World.GameData.Abilities;
using The_World.GameData.GameMechanics;

namespace The_World.GameData;

/// <summary>
/// A character archetype: base health/mana, which ability score matters most,
/// what you gain each level, and the special abilities you start with.
///
/// (This resolves the old "string Class smells funny" TODO - the class is
/// now a real type instead of a magic string.)
/// </summary>
/// <param name="Name">Warrior, Mage, Rogue...</param>
/// <param name="Description">Shown during character creation.</param>
/// <param name="BaseHealth">Health before Strength adjustments.</param>
/// <param name="BaseMana">Mana before Intelligence adjustments.</param>
/// <param name="PrimaryStat">The score this class leans on; +2 at creation, +1 per level.</param>
/// <param name="HealthPerLevel">Max health gained per level.</param>
/// <param name="ManaPerLevel">Max mana gained per level.</param>
/// <param name="StartingWeaponKey">ItemFactory key for the starting weapon.</param>
/// <param name="StartingArmorKey">ItemFactory key for the starting armor.</param>
/// <param name="Abilities">Special moves known from level 1.</param>
public record PlayerClass(
    string Name,
    string Description,
    int BaseHealth,
    int BaseMana,
    AbilityScaling PrimaryStat,
    int HealthPerLevel,
    int ManaPerLevel,
    string StartingWeaponKey,
    string StartingArmorKey,
    IReadOnlyList<Ability> Abilities)
{
    public static readonly PlayerClass Warrior = new(
        "Warrior",
        "A wall of muscle and stubbornness. Hits hard, gets hit harder, keeps standing.",
        BaseHealth: 30,
        BaseMana: 10,
        PrimaryStat: AbilityScaling.Strength,
        HealthPerLevel: 8,
        ManaPerLevel: 2,
        StartingWeaponKey: "shortsword",
        StartingArmorKey: "leather_armor",
        Abilities:
        [
            new Ability("Power Strike", "An overhead blow that ignores subtlety entirely.",
                5, new Dice(2, 8), AbilityKind.Damage, AbilityScaling.Strength,
                "You put your whole body behind a crushing strike at {0}!"),
            new Ability("Second Wind", "Grit your teeth and shake off your wounds.",
                6, new Dice(2, 8), AbilityKind.Heal, AbilityScaling.Strength,
                "You plant your feet, breathe deep, and refuse to fall.")
        ]);

    public static readonly PlayerClass Mage = new(
        "Mage",
        "A student of the arcane. Physically unimpressive, but why punch when you can immolate?",
        BaseHealth: 18,
        BaseMana: 30,
        PrimaryStat: AbilityScaling.Intelligence,
        HealthPerLevel: 4,
        ManaPerLevel: 8,
        StartingWeaponKey: "gnarled_staff",
        StartingArmorKey: "apprentice_robes",
        Abilities:
        [
            new Ability("Firebolt", "A dart of crackling flame.",
                4, new Dice(2, 6), AbilityKind.Damage, AbilityScaling.Intelligence,
                "A bolt of fire leaps from your fingertips at {0}!"),
            new Ability("Frost Lance", "A spear of ice for problems that need serious solving.",
                10, new Dice(3, 8), AbilityKind.Damage, AbilityScaling.Intelligence,
                "The air freezes into a glittering lance that hurtles toward {0}!"),
            new Ability("Mend", "Knit flesh with a whispered word.",
                6, new Dice(2, 8), AbilityKind.Heal, AbilityScaling.Intelligence,
                "Soft blue light seals your wounds.")
        ]);

    public static readonly PlayerClass Rogue = new(
        "Rogue",
        "Quick hands, quicker feet, questionable morals. Strikes where it hurts and is gone before the scream.",
        BaseHealth: 24,
        BaseMana: 16,
        PrimaryStat: AbilityScaling.Dexterity,
        HealthPerLevel: 6,
        ManaPerLevel: 4,
        StartingWeaponKey: "dagger",
        StartingArmorKey: "leather_armor",
        Abilities:
        [
            new Ability("Backstab", "Find the gap in the armor. There's always a gap.",
                5, new Dice(3, 6), AbilityKind.Damage, AbilityScaling.Dexterity,
                "You slip past {0}'s guard and strike a vital spot!"),
            new Ability("Shadow Mend", "Field medicine learned in unsavory places.",
                5, new Dice(1, 8), AbilityKind.Heal, AbilityScaling.Dexterity,
                "You bind your wounds with practiced speed.")
        ]);

    /// <summary>All playable classes, in character-creation menu order.</summary>
    public static readonly IReadOnlyList<PlayerClass> All = [Warrior, Mage, Rogue];

    /// <summary>Find a class by name (case-insensitive), or null.</summary>
    public static PlayerClass? FindByName(string? name) =>
        All.FirstOrDefault(c => c.Name.Equals(name?.Trim(), StringComparison.OrdinalIgnoreCase));
}
