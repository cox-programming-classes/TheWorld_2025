namespace The_World.GameData;

/// <summary>
/// Represents the health and mana statistics of an entity.
/// TODO: Expand this to include more stats as needed.
/// TODO: Research - Structs vs Records for lightweight data containers.
/// </summary>
public record StatChart(int Health, int Mana)
{
    public int Health { get; private set; } = Health switch
    {
        < 0 => 0,
        _ => Health
    };

    public int Mana { get; private set; } = Mana switch
    {
        < 0 => 0,
        _ => Mana
    };
    
    // TODO: Use Dice rolls to determine starting stats based on Player Class or Creature Type?
    // TODO: add Properties which are Dice for things like "Attack Power", "Defense", etc.
}