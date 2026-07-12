using The_World.GameData.Abilities;
using The_World.GameData.GameMechanics;
using The_World.GameData.Items;

namespace The_World.GameData;

/// <summary>
/// Represents a player in the game.
/// </summary>
public record Player(
    string Name,
    PlayerClass Class,
    PlayerLevel Level,
    StatChart Stats)
{
    /// <summary>
    /// The Player's level needs to be mutable!
    /// But ONLY within this class.
    /// </summary>
    public PlayerLevel Level { get; private set; } = Level;

    /// <summary>Walking-around money.</summary>
    public int Gold { get; private set; }

    private Inventory? _inventory;

    /// <summary>
    /// The player's pack. Capacity tracks current Strength,
    /// so a Strength boost immediately lets you carry more loot.
    /// </summary>
    public Inventory Inventory =>
        _inventory ??= new Inventory(() => GameMath.CarryCapacity(Stats.Strength));

    /// <summary>Currently equipped weapon and armor.</summary>
    public Equipment Equipment { get; } = new();

    /// <summary>
    /// Raised when the player gains a level: (player, oldLevel, newLevel).
    /// The engine listens to this to print the fanfare.
    /// </summary>
    public event Action<Player, int, int>? LeveledUp;

    // --- Derived combat numbers -------------------------------------------

    /// <summary>
    /// Attack bonus for weapon swings. Finesse weapons let a nimble
    /// character use Dexterity instead of Strength.
    /// </summary>
    public int AttackBonus => MeleeModifier + Equipment.AttackBonus;

    /// <summary>Flat bonus added to weapon damage rolls.</summary>
    public int DamageBonus => MeleeModifier;

    /// <summary>How hard this player is to hit: 10 + DEX modifier + armor.</summary>
    public int DefenseValue => 10 + Stats.DexterityModifier + Equipment.DefenseBonus;

    /// <summary>Damage dice of the equipped weapon (or bare fists).</summary>
    public Dice DamageDice => Equipment.DamageDice;

    private int MeleeModifier => Equipment.WeaponIsFinesse
        ? Math.Max(Stats.StrengthModifier, Stats.DexterityModifier)
        : Stats.StrengthModifier;

    /// <summary>Experience still needed to reach the next level.</summary>
    public double ExperienceToNextLevel =>
        Math.Max(0, GameMath.ExperienceForLevel(Level.Value + 1) - Level.Experience);

    // --- Behavior ----------------------------------------------------------

    /// <summary>
    /// Add experience to the Player's level. If that crosses one or more
    /// level thresholds, stats improve per the player's class, health and
    /// mana are fully restored, and the LeveledUp event fires.
    /// </summary>
    /// <param name="exp">Experience gained</param>
    public void AddExperience(double exp)
    {
        int before = Level.Value;
        Level += exp;
        int after = Level.Value;
        if (after <= before)
            return;

        for (var lvl = before; lvl < after; lvl++)
        {
            Stats.Improve(
                health: Class.HealthPerLevel,
                mana: Class.ManaPerLevel,
                strength: Class.PrimaryStat == AbilityScaling.Strength ? 1 : 0,
                dexterity: Class.PrimaryStat == AbilityScaling.Dexterity ? 1 : 0,
                intelligence: Class.PrimaryStat == AbilityScaling.Intelligence ? 1 : 0);
        }
        Stats.FullRestore();
        LeveledUp?.Invoke(this, before, after);
    }

    public void AddGold(int amount) => Gold += Math.Max(0, amount);

    /// <summary>
    /// Try to pay for something. Returns false (spending nothing) if the
    /// player can't afford it.
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (amount < 0 || amount > Gold)
            return false;
        Gold -= amount;
        return true;
    }

    /// <summary>
    /// A full character sheet, for the 'stats' command.
    /// </summary>
    public string CharacterSheet() => $"""
        ─── {Name}, Level {Level.Value} {Class.Name} ───
        {Stats}
        Attack: +{AttackBonus} to hit, {DamageDice}{(DamageBonus != 0 ? $"{(DamageBonus > 0 ? "+" : "")}{DamageBonus}" : "")} damage
        Defense: {DefenseValue}
        Gold: {Gold}
        Experience: {Level.Experience:0} ({ExperienceToNextLevel:0} to next level)
        {Equipment.Describe()}
        """;

    /// <summary>
    /// Factory method to create a new player with rolled stats and a class.
    /// </summary>
    /// <param name="name">The Player's Name</param>
    /// <param name="playerClass">The Player's Class</param>
    /// <param name="stats">The (freshly dice-rolled) starting stats</param>
    /// <returns>a new Player instance</returns>
    public static Player CreateNewPlayer(string? name, PlayerClass playerClass, StatChart stats)
        => new(
            name?.Trim() switch
            {
                null or "" => "Unknown Hero",
                _ => name.Trim()
            },
            playerClass,
            1, // Start at Level 1 (experience gets calculated automagically)
            stats);
}

/// <summary>
/// PlayerLevel represents the level and experience of a player.
/// The leveling curve is exponential: Experience = 500 * e^(level/30).
/// </summary>
/// <param name="Value">What integer level is the player?</param>
/// <param name="Experience">How much Experience have they accumulated</param>
public record PlayerLevel(int Value = 1, double Experience = 0.0)
{
    /// <summary>
    /// the Player's Level!
    /// </summary>
    public int Value { get; } = Value switch
    {
        < 1 => 1, // Ensure the level is at least 1
        > 100 => 100, // Cap the level at 100
        _ => Value // Otherwise, use the provided value
    };

    /// <summary>
    /// Total experience accumulated.
    /// </summary>
    public double Experience { get; } = Experience switch
    {
        < 0 => 0, // Ensure experience is not negative
        _ => Experience // Otherwise, use the provided experience
    };

    /// <summary>
    /// Implicitly get the Level Value when used as an integer
    /// </summary>
    public static implicit operator int(PlayerLevel level) => level.Value;

    /// <summary>
    /// Implicitly get the Experience when used as a Double
    /// </summary>
    public static implicit operator double(PlayerLevel level) => level.Experience;

    /// <summary>
    /// Create using only the integer Level - Calculates Experience
    /// </summary>
    public static implicit operator PlayerLevel(int level) =>
        new(level, 500 * Math.Exp(level / 30.0));

    /// <summary>
    /// Create using only Experience - Calculates Level.
    /// (The tiny epsilon keeps exact thresholds from truncating down:
    /// 30*ln(e^(5/30)) can compute as 4.9999999..., which is level 5, not 4.)
    /// </summary>
    public static implicit operator PlayerLevel(double experience) =>
        new((int)Math.Floor(30 * Math.Log(experience / 500) + 1e-9), experience);

    /// <summary>
    /// Add experience to a Player's level using the + operator!
    /// </summary>
    public static PlayerLevel operator +(PlayerLevel level, double experiencePoints)
        => level.Experience + experiencePoints;
}
