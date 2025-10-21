using The_World.GameData.Creatures;
using The_World.GameData.Items;

namespace The_World.GameData.Areas;

public record Area(
    string Name,
    string Description,
    Dictionary<string, Item> Items,
    Dictionary<string, Creature> Creatures,
    Dictionary<string, Area> ConnectedAreas)
{
    /// <summary>
    /// Look around the Area.
    /// </summary>
    /// <returns></returns>
    public string Look() => $"""
                             {Name}
                             {Description}
                             """;
    
    public string LookAround() => $"""
                                    You look around the {Name}.

                                    Items here:
                                    { (Items.Count == 0 ? "  None" : string.Join(Environment.NewLine, Items.Keys.Select(i => $"  {i}"))) }

                                    Creatures here:
                                    { (Creatures.Count == 0 ? "  None" : string.Join(Environment.NewLine, Creatures.Keys.Select(c => $"  {c}"))) }

                                    Connected Areas:
                                    { (ConnectedAreas.Count == 0 ? "  None" : string.Join(Environment.NewLine, ConnectedAreas.Keys.Select(a => $"  {a}"))) }
                                    """;

    /// <summary>
    /// Look at a specific target in the Area.
    /// This could be an Item, Creature, or Connected Area.
    /// </summary>
    /// <param name="target">The name of the target to look at.</param>
    /// <returns></returns>
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