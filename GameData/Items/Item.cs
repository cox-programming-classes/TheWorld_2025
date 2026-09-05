namespace The_World.GameData.Items;

/// <summary>
/// A generic Item in the game world - the base of the item hierarchy.
///
/// Derived record types (Weapon, Armor, Consumable, QuestItem) act as a
/// closed set of item shapes: pattern matching over them
/// (e.g. `item switch { Weapon w => ..., Armor a => ... }`) gives us the
/// "discriminated union" style the design notes asked about.
/// </summary>
/// <param name="Name">Display name.</param>
/// <param name="Description">Flavor text shown when looked at.</param>
/// <param name="Weight">Weight in pounds - inventories have limits!</param>
/// <param name="Value">Worth in gold, for buying and selling.</param>
public record Item(string Name, string Description, double Weight, int Value = 0)
{
    /// <summary>
    /// Optional ASCII depiction shown when the item is inspected.
    /// </summary>
    public string Art { get; init; } = "";

    /// <summary>
    /// When you look at the item, this is what you see.
    /// Derived types add their own details (damage dice, defense, etc.).
    /// </summary>
    public virtual string Look()
        => $"{ArtBlock}{Name}  ({Weight:0.#} lbs, {Value} gold){Environment.NewLine}{Description}";

    protected string ArtBlock =>
        string.IsNullOrWhiteSpace(Art) ? "" : Art.TrimEnd() + Environment.NewLine;

    public string Name { get; } = Name?.Trim() switch
    {
        null or "" => "Unknown Item",
        _ => Name.Trim()
    };

    public string Description { get; } = Description?.Trim() switch
    {
        null or "" => "No description available.",
        _ => Description.Trim()
    };

    public double Weight { get; } = Weight switch
    {
        < 0 => 0,
        _ => Weight
    };

    public int Value { get; } = Value switch
    {
        < 0 => 0,
        _ => Value
    };
}
