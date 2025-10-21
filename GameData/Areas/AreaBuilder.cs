using The_World.GameData.Creatures;
using The_World.GameData.Items;

namespace The_World.GameData.Areas;

public class AreaBuilder
{
    private Area _area;
    
    #region From Methods
    /// <summary>
    /// Creates a new AreaBuilder with an empty Area with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static AreaBuilder FromName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Area name cannot be null or empty.");
        var builder = new AreaBuilder
        {
            _area = new Area(
                name.Trim(), 
                "", 
                [],
                [], 
                [])
        };
        return builder;
    }
    
    /// <summary>
    /// Creates a new AreaBuilder from an existing Area.
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static AreaBuilder FromArea(Area area)
    {
        var builder = new AreaBuilder { _area = area };
        return builder;
    }
    #endregion

    #region With Methods
    /// <summary>
    /// Add a description to the Area.
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public AreaBuilder WithDescription(string description)
    {
        _area = _area with { Description = description?.Trim() ?? "" };
        return this;
    }
    
    /// <summary>
    /// Adds an Item to the Area.
    /// </summary>
    /// <param name="key">The key to identify the item in the area.</param>
    /// <param name="item">The Item to add.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public AreaBuilder WithItem(string key, Item item)
    {
        // TODO: Validate Key Naming Rules!
        if(string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key), "Item key cannot be null or empty.");
        
        if(item is null)
            throw new ArgumentNullException(nameof(item), "Item cannot be null.");
        
        if(!_area.Items.TryAdd(key, item))
            throw new ArgumentException($"An item with the key '{key}' already exists in the area.");
        
        return this;
    }

    /// <summary>
    /// Adds a Creature to the Area.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="creature"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public AreaBuilder WithCreature(string key, Creature creature)
    {
        // TODO: Validate Key Naming Rules!
        if(string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key), "Creature key cannot be null or empty.");
        
        if(creature is null)
            throw new ArgumentNullException(nameof(creature), "Creature cannot be null.");
        
        if(!_area.Creatures.TryAdd(key, creature))
            throw new ArgumentException($"A creature with the key '{key}' already exists in the area");
        
        return this;
    }

    /// <summary>
    /// Adds a Connected Area to the Area.
    /// TODO: Add a reciprocal connection option? Like .WithConnectedArea("north", areaB, reciprocalKey: "south")
    /// This would automatically add areaA to areaB's connected areas.  You need this to be able to navigate back and forth.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="area"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public AreaBuilder WithConnectedArea(string key, Area area)
    {
        // TODO: Validate Key Naming Rules!
        if(string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key), "Connected Area key cannot be null or empty.");
        if(area is null)
            throw new ArgumentNullException(nameof(area), "Connected Area cannot be null.");
        if(!_area.ConnectedAreas.TryAdd(key, area))
            throw new ArgumentException($"A connected area with the key '{key}' already exists in the area");
        return this;
    }

    #endregion
    
    /// <summary>
    /// TODO: There is actually still validation to do here.
    /// Currently, all validation is done in the With methods, but
    /// some cross-field validation might be necessary.
    /// Such as ensuring no duplicate keys across Items, Creatures, and Connected Areas.
    /// </summary>
    /// <returns></returns>
    public Area Build() => _area;
}