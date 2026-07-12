using The_World.GameData.Items;

namespace The_World.GameData.Creatures;

/// <summary>
/// A non-player character: a Creature you talk to instead of fight.
/// NPCs carry a dialogue tree and, if they're the merchant sort, wares.
/// </summary>
/// <param name="Dialogue">The conversation graph shown when the player talks to them.</param>
public record Npc(
    string Name,
    string Description,
    StatChart Stats,
    int Level,
    DialogueTree Dialogue)
    : Creature(Name, Description, Stats, Level, XP: 0)
{
    /// <summary>
    /// Items this NPC will sell. Mutable on purpose: buying removes from
    /// the list, selling adds to it - a tiny living economy.
    /// </summary>
    public List<Item> Wares { get; init; } = [];

    /// <summary>
    /// Create an NPC (peaceable by default - IsHostile comes from Creature
    /// and must be turned off here).
    /// </summary>
    public static Npc CreateNpc(
        string name,
        string description,
        int level,
        DialogueTree dialogue,
        List<Item>? wares = null)
        => new(name, description, new StatChart(20 + 5 * level, 10), level, dialogue)
        {
            IsHostile = false,
            Wares = wares ?? []
        };
}
