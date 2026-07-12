namespace The_World.GameData.GameMechanics;

/// <summary>
/// Common game math functions: ability modifiers, experience scaling,
/// difficulty checks, and carry weight.
/// </summary>
public static class GameMath
{
    /// <summary>
    /// Classic tabletop ability modifier: 10 is average (+0), every 2 points
    /// above or below shifts the modifier by 1. STR 14 => +2, DEX 7 => -2.
    /// </summary>
    public static int AbilityModifier(int score) =>
        (int)Math.Floor((score - 10) / 2.0);

    /// <summary>
    /// Total experience required to reach a given level.
    /// Mirrors the PlayerLevel curve: Experience = 500 * e^(level/30).
    /// </summary>
    public static double ExperienceForLevel(int level) =>
        500 * Math.Exp(Math.Max(1, level) / 30.0);

    /// <summary>
    /// How much experience a slain creature is worth, scaled by the
    /// difference between its level and the player's. Punching down pays less.
    /// </summary>
    public static double ScaledExperience(double baseXp, int creatureLevel, int playerLevel)
    {
        var levelGap = creatureLevel - playerLevel;
        var multiplier = Math.Clamp(1.0 + levelGap * 0.25, 0.25, 2.0);
        return Math.Round(baseXp * multiplier, 1);
    }

    /// <summary>
    /// A skill/ability check: roll a d20, add the relevant ability modifier,
    /// and compare against a difficulty class (DC).
    /// </summary>
    public static bool Check(int abilityScore, int difficulty, Random? rng = null) =>
        Dice.D20.Roll(rng) + AbilityModifier(abilityScore) >= difficulty;

    /// <summary>
    /// How much weight (in lbs) a character can lug around, based on Strength.
    /// </summary>
    public static double CarryCapacity(int strength) => strength * 5.0;
}
