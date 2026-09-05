using The_World.GameData.Creatures;
using The_World.GameData.Items;

namespace The_World.GameData.Areas;

public class AreaBuilder
{
    private Area _area = null!; // always set by the From* factory methods
    
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
    /// Adds an ASCII depiction shown when the area is inspected.
    /// </summary>
    public AreaBuilder WithArt(string art)
    {
        _area = _area with { Art = art?.TrimEnd() ?? "" };
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
    /// Marks the Area as a safe zone: no ambushes, restful rest.
    /// </summary>
    public AreaBuilder AsSafeZone()
    {
        _area = _area with { IsSafe = true };
        return this;
    }

    /// <summary>
    /// Adds a hidden Item - invisible to 'look', revealed by a successful
    /// 'search' (Intelligence check).
    /// </summary>
    public AreaBuilder WithHiddenItem(string key, Item item)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key), "Item key cannot be null or empty.");
        if (item is null)
            throw new ArgumentNullException(nameof(item), "Item cannot be null.");
        if (!_area.HiddenItems.TryAdd(key, item))
            throw new ArgumentException($"A hidden item with the key '{key}' already exists in the area.");
        return this;
    }

    /// <summary>
    /// Locks an exit behind a key item. The exit itself may be wired up later
    /// (see Connect) - the lock is checked whenever the player tries to leave
    /// through that exit key.
    /// </summary>
    /// <param name="exitKey">The ConnectedAreas key this lock guards.</param>
    /// <param name="requiredItemName">Item (by Name) the player must carry.</param>
    /// <param name="lockedMessage">Shown when the way is shut.</param>
    public AreaBuilder WithLockedExit(string exitKey, string requiredItemName, string lockedMessage)
    {
        if (string.IsNullOrWhiteSpace(exitKey))
            throw new ArgumentNullException(nameof(exitKey), "Exit key cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(requiredItemName))
            throw new ArgumentNullException(nameof(requiredItemName), "Required item name cannot be null or empty.");
        if (!_area.LockedExits.TryAdd(exitKey, new ExitLock(requiredItemName, lockedMessage ?? "The way is shut.")))
            throw new ArgumentException($"Exit '{exitKey}' is already locked.");
        return this;
    }

    /// <summary>
    /// Connects two already-built areas in both directions - the reciprocal
    /// connection the one-way WithConnectedArea always needed.
    /// (Areas are records, but their dictionaries are mutable, so wiring the
    /// world graph after building each node is straightforward.)
    /// </summary>
    public static void Connect(Area a, string exitFromA, Area b, string exitFromB)
    {
        if (a is null || b is null)
            throw new ArgumentNullException(a is null ? nameof(a) : nameof(b));
        if (!a.ConnectedAreas.TryAdd(exitFromA, b))
            throw new ArgumentException($"'{a.Name}' already has an exit '{exitFromA}'.");
        if (!b.ConnectedAreas.TryAdd(exitFromB, a))
            throw new ArgumentException($"'{b.Name}' already has an exit '{exitFromB}'.");
    }

    /// <summary>
    /// Adds a Connected Area to the Area (one direction only - prefer the
    /// static Connect helper for two-way passages).
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
