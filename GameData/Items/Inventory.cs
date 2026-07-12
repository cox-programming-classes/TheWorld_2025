namespace The_World.GameData.Items;

/// <summary>
/// A weight-limited bag of items. Capacity is provided by a delegate so it
/// can track the owner's (possibly changing) Strength without the inventory
/// needing to know who owns it.
/// </summary>
public class Inventory(Func<double> capacityProvider)
{
    private readonly List<Item> _items = [];

    public IReadOnlyList<Item> Items => _items;

    public double Capacity => capacityProvider();

    public double TotalWeight => _items.Sum(i => i.Weight);

    public int Count => _items.Count;

    public bool CanCarry(Item item) => TotalWeight + item.Weight <= Capacity;

    /// <summary>
    /// Add an item if there's weight allowance for it.
    /// </summary>
    /// <returns>false if the item is too heavy to carry.</returns>
    public bool TryAdd(Item item)
    {
        if (!CanCarry(item))
            return false;
        _items.Add(item);
        return true;
    }

    public bool Remove(Item item) => _items.Remove(item);

    /// <summary>
    /// Find an item by (partial, case-insensitive) name.
    /// "pot" will find a "Healing Potion" - exact matches win first.
    /// </summary>
    public Item? Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return null;
        query = query.Trim();

        return _items.FirstOrDefault(i => i.Name.Equals(query, StringComparison.OrdinalIgnoreCase))
            ?? _items.FirstOrDefault(i => i.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            ?? _items.FirstOrDefault(i => i.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    public string Describe() => _items.Count == 0
        ? $"Your pack is empty.  (capacity {Capacity:0.#} lbs)"
        : string.Join(Environment.NewLine, _items
              .Select(i => $"  {i.Name,-24} {i.Weight,5:0.#} lbs"))
          + Environment.NewLine
          + $"  Total: {TotalWeight:0.#} / {Capacity:0.#} lbs";
}
