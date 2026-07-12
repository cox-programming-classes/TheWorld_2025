using The_World.GameData.Abilities;
using The_World.GameData.GameMechanics;
using The_World.GameData.Items;

namespace The_World.GameData.Creatures;

/// <summary>
/// Represents a creature in the game.
/// This could be an enemy, a friendly NPC (see the derived Npc record),
/// or any other entity that interacts with the player.
/// </summary>
/// <param name="Name">The creature's name</param>
/// <param name="Description">How the creature appears</param>
/// <param name="Stats">Health, mana, and ability scores</param>
/// <param name="Level">Creature's level</param>
/// <param name="XP">How much Experience the Creature grants when slain</param>
public record Creature(
    string Name,
    string Description,
    StatChart Stats,
    int Level,
    double XP)
{
    /// <summary>Dice rolled when this creature lands a hit.</summary>
    public Dice DamageDice { get; init; } = new(1, 4);

    /// <summary>Attack bonus beyond what Strength provides (training, claws, malice).</summary>
    public int AttackBonus { get; init; }

    /// <summary>Hide, scales, or bone that makes this thing harder to hurt.</summary>
    public int NaturalArmor { get; init; }

    /// <summary>Gold coins carried (dropped on defeat).</summary>
    public int Gold { get; init; }

    /// <summary>Items dropped on defeat.</summary>
    public IReadOnlyList<Item> Loot { get; init; } = [];

    /// <summary>An optional special attack or spell (used in combat sometimes).</summary>
    public Ability? Special { get; init; }

    /// <summary>Hostile creatures can be attacked and fight back. NPCs are not.</summary>
    public bool IsHostile { get; init; } = true;

    /// <summary>Slaying this creature wins the game!</summary>
    public bool IsFinalBoss { get; init; }

    /// <summary>Flavor verb for attack messages: "The wolf {snaps} at you".</summary>
    public string AttackVerb { get; init; } = "attacks";

    /// <summary>Total attack bonus: muscle plus training.</summary>
    public int TotalAttackBonus => Stats.StrengthModifier + AttackBonus;

    /// <summary>How hard this creature is to hit.</summary>
    public int DefenseValue => 10 + Stats.DexterityModifier + NaturalArmor;

    /// <summary>
    /// When you look at the creature, this is what you see:
    /// name, level, description, and how beaten-up it currently looks.
    /// </summary>
    public string Look()
        => $"{Name} [Level {Level}]{Environment.NewLine}{Description}{Environment.NewLine}{HealthDescription()}";

    /// <summary>
    /// A rough read on the creature's condition - no floating health bars
    /// in *this* world.
    /// </summary>
    public string HealthDescription()
    {
        var fraction = Stats.MaxHealth == 0 ? 0 : (double)Stats.Health / Stats.MaxHealth;
        var condition = fraction switch
        {
            >= 1.0 => "It looks unharmed.",
            > 0.7 => "It is lightly wounded.",
            > 0.4 => "It is bloodied and angry.",
            > 0.15 => "It is badly hurt.",
            > 0 => "It is barely standing.",
            _ => "It is dead."
        };
        return condition;
    }

    public static Creature CreateNewCreature(
        string name,
        string description,
        StatChart stats,
        int level,
        double xp)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Creature name cannot be null or empty.");

        return new Creature(
            name.Trim(),
            description.Trim(),
            stats,
            level,
            xp);
    }
}
