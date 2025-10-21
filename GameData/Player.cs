namespace The_World.GameData;

/// <summary>
/// Represents a player in the game.
/// </summary>
public record Player(
    string Name,
    string Class, // mmmm this one smells funny... TODO: Research - Player Classes and how to implement them
    PlayerLevel Level,  // Player Level!
    StatChart Stats)
{
    /// <summary>
    /// The Player's level needs to be mutable!
    /// But ONLY within this class.
    /// </summary>
    public PlayerLevel Level { get; private set; } = Level;
    
    /// <summary>
    /// Encapsulate the ability to Add experience to the Player's level.
    /// </summary>
    /// <param name="exp">Experience gained</param>
    public void AddExperience(double exp) =>
        Level += exp;
    
    /*
     * TODO:  Stats management (level up increases stats?)
     * the Stats property is currently immutable from outside,
     * but we may want to add methods to modify stats as the player levels up.
     * So, treat it in the same way as Level.
     */
    
    
    /// <summary>
    /// Factory method to create a new player with default starting values.
    /// </summary>
    /// <param name="name">The Player's Name</param>
    /// <param name="className">The Player's Class</param>
    /// <returns>a new Player instance</returns>
    public static Player CreateNewPlayer(string name, string className)
        => new Player(
            name?.Trim() switch          // switch on the provided `name`
            {
                null or "" => "Unknown Hero", // if name is null or empty, use "Unknown Hero"
                _ => name.Trim()              // otherwise, use the trimmed name
            },
            className?.Trim() switch     // switch on the provided `className`
            {
                null or "" => "Warrior",     // if className is null or empty, use "Warrior"
                _ => className.Trim()        // otherwise, use the trimmed className
            },
            1, // Start at Level 1 (experience gets calculated automagically)
            new(100, 50)); // Starting mana
}

/// <summary>
/// PlayerLevel represents the level and experience of a player.
/// </summary>
/// <param name="Value">What integer level is the player?</param>
/// <param name="Experience">How much Experience have they accumulated</param>
public record PlayerLevel(int Value = 1, double Experience = 0.0)
{
    /*
     * TODO: Research - Leveling Curve Formula
     * We need a formula to determine how much experience is needed for each level.
     * A common approach is to use an exponential or polynomial curve.
     * For example, we could use:
     * ExperienceNeeded = BaseExperience * (Level ^ Exponent)
     * Where BaseExperience is a constant (e.g., 500) and Exponent could be 2 or 3.
     * This would mean that as the player levels up, the experience required increases significantly.
     *
     * TODO: Research - How do we alert the Player that they have leveled up?
     * We need to consider how the game will notify the player when they reach a new level.
     * This should be an EVENT that other parts of the game can listen to.
     * For example, we could implement an event system where the PlayerLevel class
     * raises a LevelUp event whenever the experience crosses the threshold for the next level.
     */
    
    
    /// <summary>
    /// the Player's Level!
    /// </summary>
    public int Value { get; } = Value switch
    {
        < 1 => 1, // Ensure the level is at least 1
        > 100 => 100, // Cap the level at 100
        _ => Value // Otherwise, use the provided value
    };
    /// <summary>
    /// 
    /// </summary>
    public double Experience { get; } = Experience switch
    {
        < 0 => 0, // Ensure experience is not negative
        _ => Experience // Otherwise, use the provided experience
    };
    
    /// <summary>
    /// Implicitly get the Level Value when used as an integer
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static implicit operator int(PlayerLevel level) => level.Value;
    /// <summary>
    /// Implicitly get the Experience when used as a Double
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static implicit operator double(PlayerLevel level) => level.Experience;
    /// <summary>
    /// Create using only the integer Level - Calculates Experience
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static implicit operator PlayerLevel(int level) => 
        new(level, 500 * Math.Exp(level / 30.0));
    /// <summary>
    /// Create using only Experience - Calculates Level
    /// </summary>
    /// <param name="experience"></param>
    /// <returns></returns>
    public static implicit operator PlayerLevel(double experience) => 
        new((int)(30*Math.Log(experience/500)), experience);

    /// <summary>
    /// Add experience to a Player's level using the + operator!
    /// </summary>
    /// <param name="level"></param>
    /// <param name="experiencePoints"></param>
    /// <returns></returns>
    public static PlayerLevel operator +(PlayerLevel level, double experiencePoints)
        => level.Experience + experiencePoints; 
}



