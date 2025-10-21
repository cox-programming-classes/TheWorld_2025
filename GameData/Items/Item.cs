namespace The_World.GameData.Items;

/// <summary>
/// A Generic Item in the game world.
/// This could be anything from a weapon, a piece of armor,
/// a potion, or any other object that the player can interact with.
///
/// TODO: Expand this by creating derived item types
/// such as Weapon, Armor, Consumable, etc.
///
/// TODO: Research - Polymorphism for Item Types.  In Particular, look for the key word "Discriminated Unions".
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
/// <param name="weight"></param>
public record Item(string name, string description, double weight)
{
    public string Look()
        => $"{Name} (Weight: {Weight} lbs){Environment.NewLine}{Description}";
    
    public string Name { get; private set; } = name?.Trim() switch
    {
        null or "" => "Unknown Item",
        _ => name.Trim()
    };

    public string Description { get; private set; } = description?.Trim() switch
    {
        null or "" => "No description available.",
        _ => description.Trim()
    };

    public double Weight { get; private set; } = weight switch
    {
        < 0 => 0,
        _ => weight
    };
}