namespace The_World.GameData.Items;

/// <summary>
/// An item you wear so things hit you less.
/// </summary>
/// <param name="DefenseBonus">Added to the wearer's defense value.</param>
public record Armor(
    string Name,
    string Description,
    double Weight,
    int Value,
    int DefenseBonus)
    : Item(Name, Description, Weight, Value)
{
    public override string Look()
        => $"{ArtBlock}{Name}  [Armor: +{DefenseBonus} defense]  ({Weight:0.#} lbs, {Value} gold){Environment.NewLine}{Description}";
}
