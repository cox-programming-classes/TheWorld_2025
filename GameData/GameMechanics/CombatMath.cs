namespace The_World.GameData.GameMechanics;

/// <summary>
/// The result of one attack: the raw d20 roll, whether it hit,
/// and how much damage got through.
/// </summary>
/// <param name="AttackRoll">The detailed d20 roll (for flavor text).</param>
/// <param name="AttackTotal">d20 + attack bonus.</param>
/// <param name="Hit">Did it connect?</param>
/// <param name="Critical">Natural 20 - double damage dice!</param>
/// <param name="Fumble">Natural 1 - always a miss, always embarrassing.</param>
/// <param name="Damage">Damage dealt (0 on a miss).</param>
public record AttackOutcome(
    DiceResult AttackRoll,
    int AttackTotal,
    bool Hit,
    bool Critical,
    bool Fumble,
    int Damage);

/// <summary>
/// Core d20 combat resolution, kept free of any I/O or game-state
/// dependencies so it can be unit tested with a seeded Random.
/// </summary>
public static class CombatMath
{
    /// <summary>
    /// Resolve a single attack:
    /// roll d20 + attackBonus vs. the target's defense value.
    /// Natural 20 always hits and doubles the damage dice;
    /// natural 1 always misses. Damage on a hit is at least 1 -
    /// if you connect, it stings.
    /// </summary>
    public static AttackOutcome ResolveAttack(
        int attackBonus,
        Dice damageDice,
        int damageBonus,
        int targetDefense,
        Random? rng = null)
    {
        var roll = Dice.D20.RollDetailed(rng);
        var total = roll.Total + attackBonus;
        var critical = roll.IsCriticalSuccess;
        var fumble = roll.IsCriticalFailure;
        var hit = !fumble && (critical || total >= targetDefense);

        var damage = 0;
        if (hit)
        {
            damage = damageDice.Roll(rng) + damageBonus;
            if (critical)
                damage += damageDice.Roll(rng);
            damage = Math.Max(1, damage);
        }

        return new AttackOutcome(roll, total, hit, critical, fumble, damage);
    }

    /// <summary>
    /// Initiative: d20 + Dexterity modifier. Ties go to the defender,
    /// so roll for the attacker first and compare with >=.
    /// </summary>
    public static int RollInitiative(int dexterityModifier, Random? rng = null)
        => Dice.D20.Roll(rng) + dexterityModifier;

    /// <summary>
    /// An escape attempt: the runner's DEX check against 10 + the chaser's
    /// DEX modifier. Failing means the chaser gets a free swipe at you.
    /// </summary>
    public static bool ResolveFlee(int runnerDexterity, int chaserDexterityModifier, Random? rng = null)
        => GameMath.Check(runnerDexterity, 10 + chaserDexterityModifier, rng);
}
