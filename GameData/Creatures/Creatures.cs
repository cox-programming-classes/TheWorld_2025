namespace The_World.GameData.Creatures;

/// <summary>
/// Represents a creature in the game.
/// This could be an enemy, a friendly NPC,
/// or any other entity that interacts with the player.
/// </summary>
/// <param name="Name">The creature's name</param>
/// <param name="Description">How the creature appears</param>
/// <param name="Health">HP for fighting</param>
/// <param name="Mana">Magic level</param>
/// <param name="Level">Creature's level</param>
/// <param name="XP">How much Experience the Creature grants</param>
public record Creature(
    string Name,
    string Description,
    StatChart Stats,
    int Level,
    double XP)
{
    /// <summary>
    /// When you look at the creature, this is what you see.
    /// It includes the name, level, and description of the creature.
    /// </summary>
    /// <returns></returns>
    public string Look()
        => $"{Name} [{Level}]{Environment.NewLine}{Description}";

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