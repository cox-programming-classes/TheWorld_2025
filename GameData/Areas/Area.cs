using The_World.GameData.Creatures;
using The_World.GameData.Items;

namespace The_World.GameData.Areas;

/// <summary>
/// A locked exit: which item opens it, and what the player sees while it's shut.
/// </summary>
/// <param name="RequiredItemName">Item (by name) the player must carry to pass.</param>
/// <param name="LockedMessage">Shown when the player tries the exit without it.</param>
public record ExitLock(string RequiredItemName, string LockedMessage);

public record Area(
    string Name,
    string Description,
    Dictionary<string, Item> Items,
    Dictionary<string, Creature> Creatures,
    Dictionary<string, Area> ConnectedAreas)
{
    /// <summary>
    /// Safe areas allow uninterrupted rest and never spawn ambushes.
    /// </summary>
    public bool IsSafe { get; init; }

    /// <summary>
    /// Items that don't show up in LookAround - the 'search' command
    /// (an Intelligence check) moves them into Items.
    /// </summary>
    public Dictionary<string, Item> HiddenItems { get; init; } = [];

    /// <summary>
    /// Exits that need a key item. Keyed by the same exit key as ConnectedAreas.
    /// Unlocking removes the entry.
    /// </summary>
    public Dictionary<string, ExitLock> LockedExits { get; init; } = [];

    /// <summary>
    /// Optional ASCII depiction shown when the area is inspected or entered.
    /// </summary>
    public string Art { get; init; } = "";

    /// <summary>
    /// Look around the Area.
    /// </summary>
    public string Look() => $"""
                             {ArtBlock}
                             ── {Name} ──
                             {Description}
                             """;

    private string ArtBlock =>
        string.IsNullOrWhiteSpace(Art) ? "" : Art.TrimEnd() + Environment.NewLine;

    public string LookAround() => $"""
                                    You look around the {Name}.{(IsSafe ? " It feels safe here." : "")}

                                    Items here:
                                    {(Items.Count == 0 ? "  None" : string.Join(Environment.NewLine, Items.Values.Select(i => $"  {i.Name}")))}

                                    Creatures here:
                                    {(Creatures.Count == 0 ? "  None" : string.Join(Environment.NewLine, Creatures.Values.Select(c => $"  {c.Name}{(c.IsHostile ? " (hostile)" : "")}")))}

                                    Exits:
                                    {(ConnectedAreas.Count == 0 ? "  None" : string.Join(Environment.NewLine, ConnectedAreas.Select(kv => $"  {kv.Key} -> {kv.Value.Name}{LockHint(kv.Key)}")))}
                                    """;

    private string LockHint(string exitKey) =>
        LockedExits.ContainsKey(exitKey) ? "  [locked]" : "";

    /// <summary>
    /// Look at a specific target in the Area.
    /// This could be an Item, Creature, or Connected Area.
    /// </summary>
    /// <param name="target">The name of the target to look at.</param>
    public string LookAt(string target)
    {
        if (Items.TryGetValue(target, out var item))
            return item.Look();
        if (Creatures.TryGetValue(target, out var creature))
            return creature.Look();
        if (ConnectedAreas.TryGetValue(target, out var area))
            return area.Look();

        return $"There is nothing notable about '{target}' here.";
    }
}
