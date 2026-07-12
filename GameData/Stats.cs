using The_World.GameData.GameMechanics;

namespace The_World.GameData;

/// <summary>
/// The health, mana, and ability scores of an entity (player or creature).
///
/// Maximums and ability scores are set at construction; the *current*
/// Health and Mana are mutable, but only through the methods on this record
/// (TakeDamage, Heal, SpendMana, ...) - the same encapsulation pattern
/// used by Player.Level.
/// </summary>
/// <param name="MaxHealth">Hit points when fully healed.</param>
/// <param name="MaxMana">Mana when fully rested.</param>
/// <param name="Strength">Muscle. Drives melee damage and carry weight.</param>
/// <param name="Dexterity">Agility. Drives defense, initiative, and fleeing.</param>
/// <param name="Intelligence">Wits. Drives spell power and noticing things.</param>
public record StatChart(
    int MaxHealth,
    int MaxMana,
    int Strength = 10,
    int Dexterity = 10,
    int Intelligence = 10)
{
    public int MaxHealth { get; private set; } = Math.Max(1, MaxHealth);
    public int MaxMana { get; private set; } = Math.Max(0, MaxMana);

    /// <summary>Current hit points. Starts full.</summary>
    public int Health { get; private set; } = Math.Max(1, MaxHealth);

    /// <summary>Current mana. Starts full.</summary>
    public int Mana { get; private set; } = Math.Max(0, MaxMana);

    public int Strength { get; private set; } = Math.Clamp(Strength, 1, 30);
    public int Dexterity { get; private set; } = Math.Clamp(Dexterity, 1, 30);
    public int Intelligence { get; private set; } = Math.Clamp(Intelligence, 1, 30);

    public bool IsAlive => Health > 0;

    // Derived combat numbers (equipment bonuses are layered on elsewhere).
    public int StrengthModifier => GameMath.AbilityModifier(Strength);
    public int DexterityModifier => GameMath.AbilityModifier(Dexterity);
    public int IntelligenceModifier => GameMath.AbilityModifier(Intelligence);

    /// <summary>
    /// Reduce current health, never below zero.
    /// </summary>
    /// <returns>The damage actually applied.</returns>
    public int TakeDamage(int amount)
    {
        var applied = Math.Clamp(amount, 0, Health);
        Health -= applied;
        return applied;
    }

    /// <summary>
    /// Restore current health, never above MaxHealth.
    /// </summary>
    /// <returns>The healing actually applied.</returns>
    public int Heal(int amount)
    {
        var applied = Math.Clamp(amount, 0, MaxHealth - Health);
        Health += applied;
        return applied;
    }

    /// <summary>
    /// Try to pay a mana cost. Returns false (and spends nothing) if
    /// there isn't enough mana.
    /// </summary>
    public bool SpendMana(int cost)
    {
        if (cost < 0 || cost > Mana)
            return false;
        Mana -= cost;
        return true;
    }

    /// <summary>
    /// Restore current mana, never above MaxMana.
    /// </summary>
    /// <returns>The mana actually restored.</returns>
    public int RestoreMana(int amount)
    {
        var applied = Math.Clamp(amount, 0, MaxMana - Mana);
        Mana += applied;
        return applied;
    }

    /// <summary>
    /// Fully restore health and mana (a good night's sleep, or a level up).
    /// </summary>
    public void FullRestore()
    {
        Health = MaxHealth;
        Mana = MaxMana;
    }

    /// <summary>
    /// Permanently raise maximums and ability scores (level up!).
    /// Current health/mana rise by the same amount so the gain is felt immediately.
    /// </summary>
    public void Improve(int health = 0, int mana = 0, int strength = 0, int dexterity = 0, int intelligence = 0)
    {
        MaxHealth += Math.Max(0, health);
        MaxMana += Math.Max(0, mana);
        Health = Math.Min(Health + Math.Max(0, health), MaxHealth);
        Mana = Math.Min(Mana + Math.Max(0, mana), MaxMana);
        Strength = Math.Clamp(Strength + strength, 1, 30);
        Dexterity = Math.Clamp(Dexterity + dexterity, 1, 30);
        Intelligence = Math.Clamp(Intelligence + intelligence, 1, 30);
    }

    public override string ToString() =>
        $"HP {Health}/{MaxHealth}  MP {Mana}/{MaxMana}  STR {Strength}  DEX {Dexterity}  INT {Intelligence}";
}
