namespace The_World.GameData.Items;

/// <summary>
/// A story-critical item. Weighs almost nothing, can't be sold,
/// and merchants politely refuse to touch it.
/// </summary>
public record QuestItem(
    string Name,
    string Description,
    double Weight = 0.1)
    : Item(Name, Description, Weight, 0)
{
    public override string Look()
        => $"{Name}  [Quest Item]{Environment.NewLine}{Description}";
}
